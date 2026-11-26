# Chạy sau khi `dotnet run`; thử các trang không cần đăng nhập.
param([string]$BaseUrl = "http://localhost:5025")
$paths = @(
    "/", "/san-pham", "/products.html?category=1",
    "/pages/login.html", "/pages/register.html",
    "/pages/cart.html", "/pages/checkout.html",
    "/api/products", "/api/categories"
)
foreach ($path in $paths) {
    try {
        $response = Invoke-WebRequest -Uri ($BaseUrl + $path) -UseBasicParsing -MaximumRedirection 5
        if ($response.StatusCode -eq 200) {
            Write-Host "OK 200  $path" -ForegroundColor Green
        } else {
            Write-Host "CHECK $($response.StatusCode)  $path" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "LOI      $path  $($_.Exception.Message)" -ForegroundColor Red
    }
}
