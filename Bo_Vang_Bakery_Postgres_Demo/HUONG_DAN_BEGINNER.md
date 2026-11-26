# Bơ Vàng Bakery – bản dành cho người mới

## 1. Chỉ cần nhớ các phần chính

- `Program.cs`: khởi động web, kết nối SQL Server và cấu hình đăng nhập.
- `data/BakeryDbContext.cs`: kết nối Model C# với các bảng SQL Server.
- `Models/`: các lớp dữ liệu như sản phẩm, danh mục, đơn hàng và tài khoản.
- `Controllers/ShopController.cs`: mở các trang của website.
- `Controllers/Api/ShopApiController.cs`: toàn bộ API của website.
- `Views/Shop/`: giao diện các trang bán hàng.
- `wwwroot/js/`: JavaScript xử lý giao diện và gọi API.
- `wwwroot/css/`: CSS giao diện.
- `Database/`: các script SQL.
- `Migrations/`: lịch sử thay đổi Database của EF Core.

## 2. Vì sao chỉ còn một ShopApiController.cs?

Các file API trước đây được chia thành nhiều file `ShopApiController.Auth.cs`, `Cart.cs`, `Catalog.cs`, `Dashboard.cs`, `Orders.cs`.

Chúng vẫn là cùng một Controller vì dùng `partial class`. Bản Beginner gom chúng thành một file để người mới dễ tìm code hơn.

Chức năng không thay đổi:

- Đăng ký / đăng nhập / đăng xuất.
- Xem và sửa tài khoản.
- Xem, thêm, sửa, xóa sản phẩm.
- Xem, thêm, sửa, xóa danh mục.
- Tìm kiếm, lọc và phân trang sản phẩm.
- Giỏ hàng.
- Đặt hàng COD.
- Theo dõi đơn hàng.
- Cập nhật hoặc hủy đơn.
- Dashboard thống kê.
- Phân quyền CUSTOMER / ADMIN.

## 3. Cách hiểu một chức năng

Ví dụ thêm sản phẩm:

1. Admin nhập thông tin trên trang quản trị.
2. `mini-admin.js` gửi `POST /api/products`.
3. `ShopApiController.cs` nhận dữ liệu.
4. Backend kiểm tra dữ liệu.
5. `BakeryDbContext` lưu vào SQL Server.
6. API trả kết quả về trình duyệt.

## 4. Các file không dùng đã được bỏ

Đã bỏ các ViewModel cũ không còn được gọi bởi ứng dụng:

- `StaffCreateViewModel.cs`
- `StockAdjustmentViewModel.cs`
- `ProductCreateViewModel.cs`
- `ProductEditViewModel.cs`

Không bỏ các Model đang liên quan tới Database hoặc lịch sử đơn hàng.

## 5. Chạy project

Mở Terminal tại thư mục có `Web-barkery1.csproj`:

```powershell
dotnet restore
dotnet build
dotnet run
```

Sau đó mở URL mà Terminal hiển thị.

> Môi trường tạo bản ZIP này không có .NET SDK và SQL Server nên chưa thể chạy `dotnet build` thực tế tại đây. Hãy chạy `dotnet build` trên máy của bạn trước khi nộp.
