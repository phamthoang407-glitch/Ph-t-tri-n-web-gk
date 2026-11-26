using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Web_barkery1.data;
using Web_barkery1.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Kiểm tra phiên đăng nhập thường xuyên.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

// Hiển thị ngày, tiền theo kiểu Việt Nam.
var vietnamCulture = CultureInfo.GetCultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = vietnamCulture;
CultureInfo.DefaultThreadCurrentUICulture = vietnamCulture;

// 2. PostgreSQL dùng riêng cho bản demo Render.
// Render cung cấp DATABASE_URL dạng postgresql://user:password@host:port/database.
// Không đưa mật khẩu vào appsettings.json hay GitHub.
var databaseUrl = builder.Configuration["DATABASE_URL"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    if (uri.Scheme != "postgresql" && uri.Scheme != "postgres")
        throw new InvalidOperationException("DATABASE_URL phải bắt đầu bằng postgresql:// hoặc postgres://.");
    var credentials = uri.UserInfo.Split(':', 2);
    if (credentials.Length != 2)
        throw new InvalidOperationException("DATABASE_URL thiếu tên đăng nhập hoặc mật khẩu.");
    var cs = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
        Username = Uri.UnescapeDataString(credentials[0]),
        Password = Uri.UnescapeDataString(credentials[1]),
        Pooling = true
    };
    connectionString = cs.ConnectionString;
}
if (string.IsNullOrWhiteSpace(connectionString) ||
    connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Bản PostgreSQL demo cần DATABASE_URL hoặc ConnectionStrings__DefaultConnection hợp lệ.");

builder.Services.AddDbContext<BakeryDbContext>(options => options.UseNpgsql(connectionString));

// 3. Đăng nhập và phân quyền bằng ASP.NET Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<BakeryDbContext>()
    .AddDefaultTokenProviders();

// 4. Cấu hình Cookie đăng nhập
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/pages/login.html";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;

    // API trả mã 401/403 thay vì chuyển trang đăng nhập.
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = 401;
        else
            context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = 403;
        else
            context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };
});

// Token chống CSRF cho các request thay đổi dữ liệu.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

// Chỉ khởi tạo trên database demo trống khi được bật rõ ràng trong Render.
if (app.Configuration.GetValue<bool>("BAKERY_DEMO_SETUP"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BakeryDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DemoDataSeeder.SeedAsync(db);
    if (!string.IsNullOrWhiteSpace(app.Configuration["SeedOwner:Email"]) &&
        !string.IsNullOrWhiteSpace(app.Configuration["SeedOwner:Password"]))
        await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

// 5. Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 6. Route MVC mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
