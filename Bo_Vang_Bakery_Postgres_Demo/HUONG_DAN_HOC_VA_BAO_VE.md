# BƠ VÀNG BAKERY MINI – TÀI LIỆU DỄ HỌC, DỄ BẢO VỆ

## 1. Đây là website gì?

Ứng dụng bán bánh cho **một cửa hàng**, làm bằng ASP.NET Core MVC (.NET 10), Entity Framework Core, SQL Server, HTML/CSS và JavaScript. Dữ liệu sản phẩm, tài khoản và đơn hàng được lưu trong SQL Server; không dùng danh sách giả để thay cho Database.

**Hai vai trò:** CUSTOMER (khách hàng), ADMIN (chủ cửa hàng). Khách chưa đăng nhập vẫn xem bánh và đặt hàng COD. Khách đăng ký chỉ được quyền CUSTOMER.

## 2. Chỉ cần nhớ 6 chỗ quan trọng

| File | Giải thích một câu |
|---|---|
| `Program.cs` | Khởi tạo ứng dụng, kết nối SQL Server, Identity, middleware và MVC. |
| `Controllers/ShopController.cs` | Trả về các trang bán hàng; chuyển đường dẫn HTML cũ sang trang đúng. |
| `Controllers/Api/ShopApiController.Catalog.cs` | API đọc, thêm, sửa, xóa sản phẩm/danh mục. |
| `Controllers/Api/ShopApiController.Orders.cs` | API tạo đơn, trừ tồn kho, chuyển trạng thái và hủy đơn. |
| `data/BakeryDbContext.cs` | Ánh xạ bảng SQL Server với các Model C#. |
| `wwwroot/js/products.js` | Nhận danh sách bánh từ API và hiển thị tìm kiếm, lọc, phân trang. |

Các thành phần liên quan: `Views/Shop/` = HTML Razor của giao diện; `wwwroot/css/` = giao diện; `wwwroot/js/mini-admin.js` = thao tác màn hình quản trị.

## 3. Một chức năng đi qua những đâu?

Ví dụ giảng viên hỏi: **Tại sao tìm kiếm bánh được?**

1. Người dùng chọn danh mục hoặc nhập tên sản phẩm.
2. `wwwroot/js/products.js` đọc từ khóa và gọi `GET /api/products?search=...&category=...`.
3. `ShopApiController.Catalog.cs` lọc `db.Products` bằng Entity Framework Core.
4. SQL Server trả về dữ liệu thật; API gửi JSON cho trình duyệt.
5. JavaScript vẽ các thẻ sản phẩm, phân trang mỗi trang 6 bánh.

Ví dụ giảng viên hỏi: **Đặt hàng hoạt động thế nào?**

1. Khách chọn bánh, cho vào giỏ, nhập tên/số điện thoại/địa chỉ.
2. `checkout.js` gọi `POST /api/orders`.
3. Backend kiểm tra dữ liệu, đọc **giá từ Database** (không tin giá do trình duyệt gửi).
4. Backend kiểm tra và trừ tồn kho, tạo đơn + chi tiết đơn trong một transaction.
5. Khách nhận mã theo dõi đơn. Admin có thể xác nhận, hoàn thành hoặc hủy có lý do.

## 4. Các route chuẩn và route tương thích

| Địa chỉ | Công dụng |
|---|---|
| `/` | Trang chủ |
| `/san-pham` | Danh sách sản phẩm |
| `/san-pham?category=1` | Sản phẩm danh mục 1 |
| `/san-pham?search=kem` | Tìm tên bánh |
| `/products.html?category=1` | Đường dẫn cũ, tự chuyển sang `/san-pham?category=1` |
| `/san-pham/ten-banh-12` | Chi tiết sản phẩm |
| `/pages/cart.html` | Giỏ hàng |
| `/pages/checkout.html` | Đặt hàng |
| `/pages/login.html` | Đăng nhập |
| `/pages/register.html` | Đăng ký |
| `/pages/account.html` | Hồ sơ khách hàng |
| `/pages/order-tracking.html` | Theo dõi đơn hàng |
| `/pages/dashboard-owner.html` | Quản trị viên |

## 5. Các yêu cầu rubric PROG2008

- Đăng ký, đăng nhập, đăng xuất: `ShopApiController.Auth.cs` và các trang tương ứng.
- Xem/sửa hồ sơ: API `/api/auth/me` và trang `account`.
- CRUD: `ShopApiController.Catalog.cs` và Dashboard.
- Tìm kiếm/lọc/phân trang: `products.js`, danh sách từ SQL Server.
- Tối thiểu 2 quyền: CUSTOMER và ADMIN. `[Authorize(Roles = "ADMIN")]` chặn người thường gọi API quản trị.
- Thống kê: `ShopApiController.Dashboard.cs`, lấy số liệu từ đơn hàng đã lưu.
- Backend/RESTful API: `/api/products`, `/api/categories`, `/api/orders`.
- Database: EF Core, khóa chính/ngoại, các bảng sản phẩm, danh mục, đơn hàng, chi tiết đơn và Identity.

## 6. Các câu hỏi vấn đáp dễ gặp

**MVC là gì?** Model là dữ liệu, View là giao diện, Controller nhận yêu cầu và xử lý/đưa dữ liệu cho View.

**API là gì?** Là đường giao tiếp để JavaScript gọi Backend, trao đổi dữ liệu JSON. Ví dụ `GET /api/products` lấy bánh, `POST /api/products` tạo bánh.

**Tại sao phải dùng SQL Server?** Dữ liệu vẫn còn sau khi tắt/mở ứng dụng, có quan hệ và ràng buộc, không phải dữ liệu JavaScript cố định.

**CRUD gồm gì?** Create (thêm), Read (xem), Update (sửa), Delete (xóa). Không xóa sản phẩm đã có đơn hàng để giữ lịch sử; có thể chỉnh tồn kho về 0 để ngừng bán.

**Vì sao cần phân quyền?** Khách chỉ được mua và xem đơn của mình; quản trị viên mới được sửa sản phẩm, cập nhật đơn và xem doanh thu. Backend kiểm tra quyền, không chỉ ẩn nút trên màn hình.

**Cookie/Identity dùng làm gì?** ASP.NET Core Identity quản lý tài khoản, lưu mật khẩu theo dạng hash, đăng nhập bằng cookie xác thực.

**CSRF là gì?** Là nguy cơ website khác gửi yêu cầu thay đổi dữ liệu bằng phiên đăng nhập của người dùng. Ứng dụng dùng token chống CSRF trong các request thay đổi dữ liệu.

**Vì sao dùng transaction?** Nếu một bước tạo đơn hoặc trừ tồn bị lỗi thì giao dịch được hoàn tác, tránh trừ tồn mà không lưu đơn.

**Tại sao còn bảng lô bánh cũ?** Giữ để tương thích với lịch sử Database và các đơn hàng đã tạo trước đây; phiên bản Mini không dùng luồng nhập lô mới. Chưa được xóa bảng nếu chưa chuyển đổi dữ liệu và migration an toàn.

## 7. Chạy và kiểm thử trên máy Windows

Mở PowerShell tại **thư mục chứa `Web-barkery1.csproj`**.

```powershell
dotnet restore
dotnet build
dotnet run
```

Sau khi ứng dụng chạy, mở URL Terminal in ra (ví dụ `http://localhost:5025`). Nhấn `Ctrl + Shift + R` nếu trình duyệt đang nhớ CSS cũ.

Chạy `powershell -ExecutionPolicy Bypass -File .\kiem-tra-trang.ps1` để kiểm tra các đường dẫn công khai (sau khi server đang chạy). Kiểm tra riêng các phần cần tài khoản: đăng ký, đăng nhập, đăng xuất, sửa hồ sơ, CRUD Admin, đặt và hủy đơn.

**Lưu ý:** Không đưa mật khẩu SQL Server hoặc tài khoản admin thật vào ZIP/GitHub. Đặt qua User Secrets/biến môi trường.

## 8. Phải tự kiểm tra trước khi nộp

- [ ] `dotnet build` thành công trên máy có .NET 10.
- [ ] Trang chủ, sản phẩm, tất cả đường dẫn mở được, không có 404.
- [ ] Đăng ký/đăng nhập/đăng xuất hoạt động, CUSTOMER không vào được Admin.
- [ ] CRUD sản phẩm/danh mục lưu được và đọc lại từ SQL Server.
- [ ] Tìm kiếm/lọc/phân trang hoạt động sau khi làm mới trang.
- [ ] Giỏ hàng và đặt hàng COD hoạt động, không cho mua quá tồn.
- [ ] Chuyển trạng thái đơn và hủy có lý do; thống kê đúng đơn hoàn thành.
- [ ] Trang web xem được trên điện thoại và desktop.
- [ ] Chuẩn bị báo cáo, ERD, SQL script, slide, video demo, hướng dẫn.

**Trạng thái thực tế:** Đã kiểm tra cú pháp JavaScript và mã nguồn tĩnh. Chưa xác nhận build/chạy với SQL Server trong môi trường tạo ZIP vì không có .NET SDK/SQL Server. Vì vậy checklist trên phải được thực hiện trên máy bạn trước khi nộp.
