# Bơ Vàng Bakery Mini — PROG2008

Đây là phiên bản tinh gọn tạo từ `web-bakery-pro.zip`, không phải dự án mới. Giữ ASP.NET Core MVC (.NET 10), Entity Framework Core, SQL Server và ASP.NET Core Identity.

## Phạm vi

- Khách: xem bánh, tìm kiếm/lọc/phân trang 6 sản phẩm mỗi trang, đăng ký/đăng nhập/đăng xuất, sửa hồ sơ, giỏ hàng, tạo đơn COD, theo dõi đơn.
- Admin: Dashboard (thống kê đơn/doanh thu), CRUD danh mục và sản phẩm, quản lý trạng thái đơn (PENDING -> PROCESSING -> COMPLETED; hủy với lý do).
- Tồn kho mới quản lý trực tiếp ở `Products.Stock`; không còn lập lô hoặc yêu cầu ngày hết hạn. Bảng lô cũ vẫn giữ trong EF Core/SQL Server vì chứa dữ liệu lịch sử.
- Chỉ tạo tài khoản CUSTOMER qua biểu mẫu đăng ký. Tài khoản ADMIN phải do người quản trị cấp, không được tự đăng ký quyền cao.

## Cách chạy trên Windows

1. Giải nén và mở thư mục `Web-barkery1` trong Visual Studio hoặc VS Code.
2. Cài .NET SDK 10, SQL Server LocalDB (hoặc SQL Server Express) và EF Core Tools nếu chưa có.
3. Mặc định `appsettings.json` dùng SQL Server LocalDB với Windows Authentication. Nếu bạn dùng SQL Authentication, không ghi mật khẩu thật vào file công khai. Trong thư mục dự án, dùng User Secrets:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\\SQLEXPRESS;Database=Web_Bakery;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
   ```

4. Nếu đã có Database cũ, hãy SAO LƯU trước khi chạy và giữ nguyên các migrations hiện có. Nếu Database chưa tồn tại và có đầy đủ EF Tools, chạy:

   ```powershell
   dotnet restore
   dotnet ef database update
   ```

5. Khi chạy Development và chưa có Admin, thiết lập email/mật khẩu seed trong User Secrets (không ghi vào source):

   ```powershell
   dotnet user-secrets set "SeedOwner:Email" "admin@example.com"
   dotnet user-secrets set "SeedOwner:Password" "CHANGE_TO_A_STRONG_PASSWORD"
   ```

6. Khởi chạy: `dotnet run`. Mở đường dẫn ứng dụng được hiển thị trên Terminal. Các trang chính:

   - `/` — Trang chủ
   - `/san-pham` — Sản phẩm, tìm kiếm, lọc, phân trang
   - `/pages/cart.html` — Giỏ hàng
   - `/pages/checkout.html` — Đặt hàng
   - `/pages/login.html` — Đăng nhập
   - `/pages/register.html` — Đăng ký
   - `/pages/dashboard-owner.html` — Dashboard Admin

## API chính

`GET /api/products`, `GET /api/products/{id}`, `POST /api/products`, `PUT /api/products/{id}`, `DELETE /api/products/{id}`; `GET/POST/PUT/DELETE /api/categories...`; `POST /api/orders`, `GET /api/orders`, `PUT /api/orders/{id}/status`; `GET /api/dashboard`; `POST /api/auth/register`, `/api/auth/login`, `/api/auth/logout`; `GET/PUT /api/auth/me`.

API thay đổi dữ liệu sử dụng cookie Identity, CSRF token từ trang, và phân quyền server. Không gửi request thay đổi dữ liệu từ trình duyệt không có token hợp lệ.

## Các lưu ý thiết kế

- Không xóa vật lý bánh đã có trong lịch sử bán hàng hoặc có lô cũ; sửa số lượng tồn bằng 0 để ngừng bán.
- Doanh thu chỉ tính đơn hoàn thành. Tạo đơn và trừ tồn được thực hiện trong transaction; hủy đơn hoàn tồn.
- Các bảng legacy như `StockBatches`, `OrderBatchAllocations`, `NewsArticles`, `StoreInfos` vẫn được giữ trong DbContext để bảo toàn dữ liệu/schema cũ, nhưng không xuất hiện trong giao diện Mini.
- Mã nguồn không cần thiết cho Mini nằm ở thư mục `LEGACY_BACKUP` bên cạnh thư mục dự án; KHÔNG đưa thư mục đó vào project và KHÔNG chạy code cũ song song.
- File `Database/04_Them_10_Banh_Mau.sql` có dữ liệu sản phẩm mẫu. Kiểm tra script trước khi chạy trên database hiện có để tránh trùng dữ liệu.

## Trạng thái kiểm thử

Đã kiểm tra cú pháp JavaScript bằng `node --check` và phân tích tĩnh cấu trúc code. **Chưa xác nhận build, migration hoặc chạy SQL Server thực tế** vì môi trường tạo bản ZIP này không có .NET SDK/SQL Server. Trước khi nộp cuối kỳ, phải kiểm thử `dotnet build`, cập nhật DB, hai vai trò, CRUD, giỏ hàng, COD, đơn hàng, thống kê, responsive, API và các ca không đủ quyền.
