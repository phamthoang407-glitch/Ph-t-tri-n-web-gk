using Microsoft.EntityFrameworkCore;
using Web_barkery1.data;
using Web_barkery1.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Web_barkery1.Controllers;
public class ShopController(BakeryDbContext db) : Controller
{
    [HttpGet("/")]
    [HttpGet("/index.html")]
    public IActionResult Index()
    {
        Response.Headers.CacheControl = "no-store, max-age=0";
        return View("index");
    }

    // Tương thích đường dẫn HTML từ website cũ.
    // Ví dụ: /products.html?category=1 -> /san-pham?category=1
    // Dùng Redirect (302), không tạo hai trang bán hàng riêng.
    [HttpGet("/{page}.html")]
    public IActionResult LegacyPage(string page)
    {
        string? destination = page.ToLowerInvariant() switch
        {
            "index" => "/",
            "products" => "/san-pham",
            "product-detail" => "/pages/product-detail.html",
            "cart" => "/pages/cart.html",
            "checkout" => "/pages/checkout.html",
            "login" => "/pages/login.html",
            "register" => "/pages/register.html",
            "account" => "/pages/account.html",
            "order-tracking" => "/pages/order-tracking.html",
            "dashboard-owner" => "/pages/dashboard-owner.html",
            _ => null
        };
        if (destination is null) return NotFound();
        return Redirect(destination + Request.QueryString);
    }

    [HttpGet("/pages/{page}.html")]
    public IActionResult Page(string page)
    {
        var allowed = new[]
        {
            "account",
            "cart",
            "checkout",
            "dashboard-owner",
            "login",
            "order-tracking",
            "product-detail",
            "products",
            "register"
        };
        if (page == "product-detail")
        {
            if (!int.TryParse(Request.Query["id"], out var productId))
                return RedirectPermanent("/san-pham");
            var product = db.Products.AsNoTracking().FirstOrDefault(x => x.Id == productId);
            return product is null ? NotFound() : RedirectPermanent(SeoUrl.Product(product));
        }
        if (!allowed.Contains(page))
            return NotFound();
        if (page is "dashboard-owner" or "account")
        {
            if (User.Identity?.IsAuthenticated != true)
                return Challenge();
            if (page == "dashboard-owner" && !User.IsInRole("ADMIN"))
                return Forbid();
        }

        return View(page);
    }
}
