using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Web_barkery1.Models;
using Web_barkery1.data;

namespace Web_barkery1.Controllers;
public class ShopInputException(string message) : Exception(message);
public class ShopErrorsAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext c)
    {
        if (c.Exception is ShopInputException or DbUpdateException)
        {
            c.Result = new ObjectResult(new { success = false, message = c.Exception is ShopInputException ? c.Exception.Message : "Dữ liệu đã thay đổi hoặc đang được sử dụng. Hãy tải lại và thử lại." })
            {
                StatusCode = c.Exception is ShopInputException ? 400 : 409
            };
            c.ExceptionHandled = true;
        }
    }
}

[ApiController, Route("api"), ShopErrors]
public class ShopApiController(BakeryDbContext db, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn) : ControllerBase
{
    string Uid => users.GetUserId(User) ?? throw new ShopInputException("Bạn cần đăng nhập.");

    bool Operator => User.IsInRole("ADMIN");

    IActionResult Good(object? data = null) => Ok(new { success = true, data });
    static string Text(JsonElement j, string key, int max = 300, bool required = true)
    {
        var s = j.ValueKind == JsonValueKind.Object && j.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString()?.Trim() ?? "" : "";
        if ((required && s.Length == 0) || s.Length > max)
            throw new ShopInputException($"Thông tin {key} còn thiếu hoặc quá dài (tối đa {max}).");
        return s;
    }

    static string Password(JsonElement j)
    {
        if (j.ValueKind != JsonValueKind.Object || !j.TryGetProperty("password", out var v) || v.ValueKind != JsonValueKind.String || string.IsNullOrEmpty(v.GetString()) || v.GetString()!.Length > 200)
            throw new ShopInputException("Vui lòng nhập mật khẩu (tối đa 200 ký tự).");
        return v.GetString()!;
    }

    static int Int(JsonElement j, string key, int min = 1, int max = 999)
    {
        if (j.ValueKind != JsonValueKind.Object || !j.TryGetProperty(key, out var v) || v.ValueKind != JsonValueKind.Number || !v.TryGetInt32(out var n) || n < min || n > max)
            throw new ShopInputException($"{key} phải là số nguyên từ {min} đến {max}.");
        return n;
    }

    static decimal Money(JsonElement j, string key, decimal min = 0, decimal max = 999999999)
    {
        if (j.ValueKind != JsonValueKind.Object || !j.TryGetProperty(key, out var v) || v.ValueKind != JsonValueKind.Number || !v.TryGetDecimal(out var n) || n < min || n > max)
            throw new ShopInputException($"Giá trị {key} không hợp lệ.");
        return decimal.Round(n, 2);
    }

    static string Image(JsonElement j)
    {
        var s = Text(j, "image", 500, false);
        if (s.Length > 0 && !(s.StartsWith("/images/") && !s.Contains("..") && !s.Contains('\\')) && !(Uri.TryCreate(s, UriKind.Absolute, out var u) && (u.Scheme == "http" || u.Scheme == "https")))
            throw new ShopInputException("Ảnh phải là URL http/https hoặc đường dẫn /images/.");
        return s;
    }

    async Task<object> PublicUser(ApplicationUser u)
    {
        var roles = await users.GetRolesAsync(u);
        return new
        {
            id = u.Id,
            full_name = u.FullName,
            email = u.Email,
            phone = u.PhoneNumber,
            address = u.Address,
            role = roles.Contains("ADMIN") ? "ADMIN" : "CUSTOMER"
        };
    }

    static void Check(IdentityResult r)
    {
        if (!r.Succeeded)
            throw new ShopInputException(string.Join("; ", r.Errors.Select(x => x.Description)));
    }

    // ===== Auth =====
[HttpPost("auth/login")]
    public async Task<IActionResult> Login(JsonElement j)
    {
        var identifier = Text(j, "identifier", 256);
        var password = Password(j);
        var u = await users.FindByEmailAsync(identifier) ?? await users.Users.FirstOrDefaultAsync(x => x.PhoneNumber == identifier);
        if (u == null)
            return Unauthorized(new { message = "Thông tin đăng nhập không đúng." });
        var r = await signIn.PasswordSignInAsync(u, password, false, true);
        if (!r.Succeeded)
            return Unauthorized(new { message = r.IsLockedOut ? "Tài khoản bị khóa. Vui lòng thử lại sau hoặc liên hệ chủ quầy." : "Thông tin đăng nhập không đúng." });
        return Good(new { user = await PublicUser(u) });
    }

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register(JsonElement j)
    {
        var email = Text(j, "email", 256);
        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            throw new ShopInputException("Email không hợp lệ.");
        var u = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = Text(j, "full_name", 100),
            PhoneNumber = Text(j, "phone", 15, false),
            Address = Text(j, "address", 300, false)
        };
        if (!string.IsNullOrEmpty(u.PhoneNumber) && (!Regex.IsMatch(u.PhoneNumber, @"^[+0-9 ]{9,15}$") || await users.Users.AnyAsync(x => x.PhoneNumber == u.PhoneNumber)))
            throw new ShopInputException("Số điện thoại không hợp lệ hoặc đã được sử dụng.");
        await using var tx = await db.Database.BeginTransactionAsync();
        Check(await users.CreateAsync(u, Password(j)));
        Check(await users.AddToRoleAsync(u, "CUSTOMER"));
        await tx.CommitAsync();
        await signIn.SignInAsync(u, false);
        return Good(new { user = await PublicUser(u) });
    }

    [Authorize, HttpPost("auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return Good();
    }

    [Authorize, HttpGet("auth/me")]
    public async Task<IActionResult> Me() => Good(await PublicUser((await users.GetUserAsync(User))!));
    [Authorize, HttpPut("auth/me")]
    public async Task<IActionResult> Profile(JsonElement j)
    {
        var u = (await users.GetUserAsync(User))!;
        u.FullName = Text(j, "full_name", 100);
        u.Address = Text(j, "address", 300, false);
        var phone = Text(j, "phone", 15, false);
        if (phone != "" && (!Regex.IsMatch(phone, @"^[+0-9 ]{9,15}$") || await users.Users.AnyAsync(x => x.Id != u.Id && x.PhoneNumber == phone)))
            throw new ShopInputException("Số điện thoại không hợp lệ hoặc đã được dùng.");
        u.PhoneNumber = phone;
        Check(await users.UpdateAsync(u));
        return Good(await PublicUser(u));
    }
    // ===== Cart =====
[Authorize, HttpGet("cart")]
    public async Task<IActionResult> Cart() => Good(await (
        from c in db.ShopCartItems
        join p in db.Products on c.ProductId equals p.Id
        where c.UserId == Uid
        select new { id = c.Id, product_id = p.Id, name = p.Name, price = p.Price, image = p.ImageUrl ?? "/images/cake-placeholder.svg", quantity = c.Quantity }).ToListAsync());
    [Authorize, HttpPost("cart")]
    public async Task<IActionResult> AddCart(JsonElement j)
    {
        var id = Int(j, "product_id", 1, int.MaxValue);
        var n = Int(j, "quantity");
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var p = await db.Products.FindAsync(id);
        if (p == null)
            return NotFound();
        var c = await db.ShopCartItems.FirstOrDefaultAsync(x => x.UserId == Uid && x.ProductId == id);
        var sellable = p.Stock;
        if (n + (c?.Quantity ?? 0) > 999 || n + (c?.Quantity ?? 0) > sellable)
            throw new ShopInputException("Số lượng vượt tồn kho hoặc giới hạn 999.");
        if (c == null)
        {
            c = new ShopCartItem
            {
                UserId = Uid,
                ProductId = id
            };
            db.ShopCartItems.Add(c);
        }

        c.Quantity += n;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Good();
    }

    [Authorize, HttpPut("cart/{id:int}")]
    public async Task<IActionResult> ChangeCart(int id, JsonElement j)
    {
        var n = Int(j, "quantity", 0);
        var c = await db.ShopCartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid);
        if (c == null)
            return NotFound();
        if (n == 0)
            db.Remove(c);
        else
        {
            var p = await db.Products.FindAsync(c.ProductId);
            var sellable = p?.Stock ?? 0;
            if (p == null || sellable < n)
                throw new ShopInputException("Không đủ tồn kho.");
            c.Quantity = n;
        }

        await db.SaveChangesAsync();
        return Good();
    }

    [Authorize, HttpDelete("cart"), HttpDelete("cart/{id:int}")]
    public async Task<IActionResult> DeleteCart(int? id = null)
    {
        await db.ShopCartItems.Where(x => x.UserId == Uid && (!id.HasValue || x.Id == id)).ExecuteDeleteAsync();
        return Good();
    }
    // ===== Catalog =====
object ProductDto(Product p) => new
    {
        id = p.Id,
        name = p.Name,
        description = p.Description,
        price = p.Price,
        old_price = p.OldPrice,
        rating = p.Rating,
        image = string.IsNullOrEmpty(p.ImageUrl) ? "/images/cake-placeholder.svg" : p.ImageUrl,
        stock = p.Stock,
        category_id = p.CategoryId,
        category = p.Category?.Name,
        status = p.Stock == 0 ? "Hết hàng" : p.Stock <= 5 ? "Sắp hết" : "Còn hàng"
    };
    // API này trả JSON cho products.js; tìm kiếm và lọc dựa trên SQL Server.
    [HttpGet("products")]
    public async Task<IActionResult> Products(string? search = null, string? category = null)
    {
        var q = db.Products.Include(x => x.Category).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Name.Contains(search));
        if (!string.IsNullOrWhiteSpace(category))
        {
            if (int.TryParse(category, out var cid))
                q = q.Where(x => x.CategoryId == cid);
            else
                q = q.Where(x => x.Category.Name == category);
        }

        var products = await q.OrderBy(x => x.Id).ToListAsync();
        return Good(products.Select(ProductDto));
    }

    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> Product(int id)
    {
        var p = await db.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p == null ? NotFound(new { message = "Không tìm thấy bánh." }) : Good(ProductDto(p));
    }

    [Authorize(Roles = "ADMIN"), HttpPost("products"), HttpPut("products/{id:int}")]
    public async Task<IActionResult> SaveProduct(JsonElement j, int? id = null)
    {
        var p = id == null ? new Product() : await db.Products.FindAsync(id);
        if (p == null) return NotFound();
        p.Name = Text(j, "name", 150);
        p.Description = Text(j, "description", 2000, false);
        p.Price = Money(j, "price", 0.01m);
        p.ImageUrl = Image(j);
        p.CategoryId = Int(j, "category_id", 1, int.MaxValue);
        if (!await db.Categories.AnyAsync(x => x.Id == p.CategoryId))
            throw new ShopInputException("Danh mục không tồn tại.");
        // Trong bản Mini, tồn kho được chỉnh trực tiếp, không tạo lô và không yêu cầu hạn sử dụng.
        p.Stock = Int(j, "stock", 0, 1000000);
        if (id == null) db.Products.Add(p);
        await db.SaveChangesAsync();
        return Good(new { id = p.Id });
    }

    [Authorize(Roles = "ADMIN"), HttpDelete("products/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // Không xóa sản phẩm có lịch sử bán hàng nhằm giữ nguyên báo cáo/chi tiết đơn.
        if (await db.ShopOrderItems.AnyAsync(x => x.ProductId == id))
            throw new ShopInputException("Bánh đã xuất hiện trong đơn hàng. Hãy đặt tồn kho = 0 để ngừng bán.");
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.ShopCartItems.Where(x => x.ProductId == id).ExecuteDeleteAsync();
        // Lô cũ là dữ liệu lịch sử; chỉ xóa bản ghi không có phân bổ đơn hàng.
        if (await db.StockBatches.AnyAsync(x => x.ProductId == id))
            throw new ShopInputException("Bánh còn dữ liệu lô cũ. Hãy đặt tồn kho = 0 thay vì xóa.");
        var changed = await db.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
        await tx.CommitAsync();
        return changed == 0 ? NotFound() : Good();
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories() => Good(await db.Categories.Select(x => new { id = x.Id, name = x.Name, description = x.Description }).ToListAsync());
    [Authorize(Roles = "ADMIN"), HttpPost("categories"), HttpPut("categories/{id:int}")]
    public async Task<IActionResult> SaveCategory(JsonElement j, int? id = null)
    {
        var c = id == null ? new Category() : await db.Categories.FindAsync(id);
        if (c == null)
            return NotFound();
        c.Name = Text(j, "name", 100);
        c.Description = Text(j, "description", 1000, false);
        if (id == null)
            db.Categories.Add(c);
        await db.SaveChangesAsync();
        return Good();
    }

    [Authorize(Roles = "ADMIN"), HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (await db.Products.AnyAsync(x => x.CategoryId == id))
            throw new ShopInputException("Danh mục đang có sản phẩm, không thể xóa.");
        await db.Categories.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Good();
    }
    // ===== Dashboard =====
[Authorize(Roles = "ADMIN"), HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.UtcNow.AddHours(7).Date;
        var start = today.AddHours(-7);
        var week = start.AddDays(-6);
        var end = start.AddDays(1);
        var done = await db.ShopOrders.Include(x => x.Items).Where(x => x.Status == "COMPLETED" && x.CompletedAt >= week && x.CompletedAt < end).ToListAsync();
        var td = done.Where(x => x.CompletedAt >= start).ToList();
        var recent = await db.ShopOrders.Include(x => x.Items).OrderByDescending(x => x.Id).Take(8).ToListAsync();
        return Good(new { revenue_today = td.Sum(x => x.TotalAmount), completed_today = td.Count, orders_today = await db.ShopOrders.CountAsync(x => x.CreatedAt >= start && x.CreatedAt < end), customers = (await users.GetUsersInRoleAsync("CUSTOMER")).Count, products = await db.Products.CountAsync(), recent_orders = recent.Select(x => OrderDto(x)), best_products = done.SelectMany(x => x.Items).GroupBy(x => new { x.ProductId, x.ProductName }).Select(g => new { product_id = g.Key.ProductId, product_name = g.Key.ProductName, sold = g.Sum(x => x.Quantity), price = g.Max(x => x.Price) }).OrderByDescending(x => x.sold).Take(5), revenue_7_days = done.GroupBy(x => x.CompletedAt!.Value.AddHours(7).ToString("yyyy-MM-dd")).Select(g => new { day = g.Key, total = g.Sum(x => x.TotalAmount) }) });
    }
    // ===== Orders =====
static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
    object OrderDto(ShopOrder o, string? token = null) => new
    {
        id = o.Id,
        code = o.Code,
        customer_name = o.CustomerName,
        customer_phone = o.CustomerPhone,
        shipping_address = o.ShippingAddress,
        payment_method = o.PaymentMethod,
        status = o.Status,
        total_amount = o.TotalAmount,
        created_at = DateTime.SpecifyKind(o.CreatedAt, DateTimeKind.Utc),
        completed_at = o.CompletedAt,
        cancellation_reason = o.CancellationReason,
        cancelled_at = o.CancelledAt,
        item_count = o.Items.Count,
        guest_tracking_token = token,
        items = o.Items.Select(x => new { id = x.Id, product_id = x.ProductId, product_name = x.ProductName, price = x.Price, quantity = x.Quantity })
    };
    // Khách đặt đơn: kiểm tra giỏ -> trừ tồn -> lưu đơn. Tất cả trong một transaction.
    [HttpPost("orders")]
    public async Task<IActionResult> Order(JsonElement j)
    {
        var o = new ShopOrder
        {
            CustomerName = Text(j, "customer_name", 100),
            CustomerPhone = Text(j, "customer_phone", 15),
            ShippingAddress = Text(j, "shipping_address", 300),
            PaymentMethod = Text(j, "payment_method", 10)
        };
        if (!Regex.IsMatch(o.CustomerPhone, @"^[+0-9 ]{9,15}$"))
            throw new ShopInputException("Số điện thoại không hợp lệ.");
        if (o.PaymentMethod != "COD")
            throw new ShopInputException("Phương thức thanh toán không hợp lệ.");
        var guest = User.Identity?.IsAuthenticated != true;
        string? token = null;
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        List<(int Id, int Quantity)> items;
        if (guest)
        {
            if (!j.TryGetProperty("items", out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() < 1 || arr.GetArrayLength() > 50)
                throw new ShopInputException("Giỏ hàng cần có từ 1 đến 50 loại bánh.");
            items = arr.EnumerateArray().Select(x => (Int(x, "product_id", 1, int.MaxValue), Int(x, "quantity"))).ToList();
            token = Guid.NewGuid().ToString();
            o.GuestTokenHash = Hash(token);
        }
        else
        {
            o.UserId = Uid;
            items = (await db.ShopCartItems.Where(x => x.UserId == Uid).ToListAsync()).Select(x => (x.ProductId, x.Quantity)).ToList();
        }

        if (items.Count == 0 || items.Count > 50 || items.Select(x => x.Id).Distinct().Count() != items.Count)
            throw new ShopInputException("Giỏ hàng trống, quá lớn hoặc có sản phẩm trùng.");
        foreach (var item in items.OrderBy(x => x.Id))
        {
            var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
            if (p == null || item.Quantity < 1 || item.Quantity > 999)
                throw new ShopInputException("Sản phẩm hoặc số lượng không hợp lệ.");
            var orderItem = new ShopOrderItem { ProductId = p.Id, ProductName = p.Name, Price = p.Price, Quantity = item.Quantity };
            // Trừ tồn bằng thao tác có điều kiện; transaction Serializable bảo vệ việc đặt đồng thời.
            var changed = await db.Products.Where(x => x.Id == item.Id && x.Stock >= item.Quantity)
                .ExecuteUpdateAsync(updates => updates.SetProperty(x => x.Stock, x => x.Stock - item.Quantity));
            if (changed != 1) throw new ShopInputException($"{p.Name} không đủ tồn kho.");
            o.Items.Add(orderItem);
            o.TotalAmount += p.Price * item.Quantity;
        }

        db.ShopOrders.Add(o);
        await db.SaveChangesAsync();
        if (!guest)
            await db.ShopCartItems.Where(x => x.UserId == Uid).ExecuteDeleteAsync();
        await tx.CommitAsync();
        return Good(OrderDto(o, token));
    }

    [Authorize, HttpGet("orders")]
    public async Task<IActionResult> Orders() => Good((await db.ShopOrders.Include(x => x.Items).Where(x => Operator || x.UserId == Uid).OrderByDescending(x => x.Id).ToListAsync()).Select(x => OrderDto(x)));
    [Authorize, HttpGet("orders/{id:int}")]
    public async Task<IActionResult> OrderDetail(int id)
    {
        var o = await db.ShopOrders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id && (Operator || x.UserId == Uid));
        return o == null ? NotFound() : Good(OrderDto(o));
    }

    [HttpGet("orders/guest/{token}")]
    public async Task<IActionResult> Guest(string token)
    {
        if (!Guid.TryParse(token, out _))
            return NotFound();
        var hash = Hash(token);
        var o = await db.ShopOrders.Include(x => x.Items).FirstOrDefaultAsync(x => x.UserId == null && x.GuestTokenHash == hash);
        return o == null ? NotFound() : Good(OrderDto(o));
    }

    [Authorize(Roles = "ADMIN"), HttpPut("orders/{id:int}/status")]
    public async Task<IActionResult> Status(int id, JsonElement j)
    {
        var next = Text(j, "status", 20);
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var o = await db.ShopOrders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        if (o == null)
            return NotFound();
        // Luồng Mini: chờ xác nhận -> đang xử lý -> hoàn thành; có thể hủy khi chưa hoàn thành.
        // Các trạng thái từ phiên bản cũ vẫn được hỗ trợ khi đọc/chuyển tiếp đơn lịch sử.
        var allowed = o.Status switch
        {
            "PENDING" => new[] { "PROCESSING", "CANCELLED" },
            "CONFIRMED" => new[] { "PROCESSING", "CANCELLED" },
            "PROCESSING" => new[] { "COMPLETED", "CANCELLED" },
            "SHIPPING" => new[] { "COMPLETED", "CANCELLED" },
            _ => Array.Empty<string>()
        };
        if (!allowed.Contains(next))
            throw new ShopInputException("Không thể chuyển trạng thái này. Hãy tải lại đơn hàng.");
        if (next == "CANCELLED")
        {
            var reason = Text(j, "cancellation_reason", 500);
            if (reason.Length < 3) throw new ShopInputException("Vui lòng nêu lý do hủy đơn.");
            o.CancellationReason = reason;
            o.CancelledAt = DateTime.UtcNow;
            o.CancelledByUserId = Uid;
            foreach (var item in o.Items.OrderBy(x => x.ProductId))
            {
                // Nếu đơn lịch sử có phân bổ lô, hoàn lại lô cũ; đơn Mini chỉ phục hồi tổng tồn.
                var allocations = await db.OrderBatchAllocations.Where(x => x.ShopOrderItemId == item.Id)
                    .Include(x => x.StockBatch).ToListAsync();
                foreach (var allocation in allocations) allocation.StockBatch.Quantity += allocation.Quantity;
                var n = await db.Products.Where(x => x.Id == item.ProductId && x.Stock <= int.MaxValue - item.Quantity).ExecuteUpdateAsync(s => s.SetProperty(x => x.Stock, x => x.Stock + item.Quantity));
                if (n != 1)
                    throw new ShopInputException("Không thể hoàn tồn kho. Liên hệ chủ quầy.");
            }
        }

        o.Status = next;
        if (next == "COMPLETED")
            o.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Good(OrderDto(o));
    }
}
