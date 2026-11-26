# Bơ Vàng Bakery Mini — Giao diện hiện đại (bản 2)

Đây là bản chỉnh giao diện từ `Bo_Vang_Bakery_Mini_PROG2008.zip` và giữ nguyên Backend, API, models, Database, Identity và luồng JavaScript.

## Thay đổi
- Trang khách hàng dùng `wwwroot/css/bovang-modern.css`, được tải sau các CSS cũ để cập nhật kiểu hiển thị mà không thay đổi ID/class dùng bởi JS.
- Dashboard quản trị dùng `wwwroot/css/mini-admin-modern.css`.
- Hai stylesheet đã được gắn vào các Razor View trong `Views/Shop/`.
- Trang chủ, trang sản phẩm, giỏ hàng, thanh toán, đăng nhập, tài khoản, theo dõi đơn và Dashboard được đồng bộ màu sắc, kiểu thẻ, nút, khoảng cách và responsive.

## Cách chạy
1. Sao lưu dự án hiện tại và Database trước khi thay thế.
2. Giải nén toàn bộ ZIP mới ra thư mục riêng, mở `Web-barkery1`.
3. Giữ cấu hình SQL Server hợp lệ trong User Secrets hoặc biến môi trường tại máy.
4. Chạy `dotnet restore`, `dotnet build`, `dotnet run` trong thư mục chứa `.csproj`.
5. Truy cập địa chỉ localhost mà terminal hiển thị. Nhấn `Ctrl+F5` trên trình duyệt nếu CSS bị cache.

## Kiểm thử cần thực hiện
- Home, danh mục sản phẩm, tìm kiếm, lọc, phân trang và chi tiết sản phẩm.
- Giỏ hàng, thanh toán và lịch sử/tra cứu đơn.
- Đăng nhập và bảng quản trị: Tổng quan, Sản phẩm, Danh mục, Đơn hàng.
- Kiểm thử điện thoại màn hình 375px và 768px.

## Lưu ý
Chỉ chỉnh diện mạo, không tuyên bố đã kiểm thử runtime vì môi trường tạo gói không có SQL Server và .NET SDK. Ảnh hero hiện sử dụng URL Unsplash nên cần internet để tải.
