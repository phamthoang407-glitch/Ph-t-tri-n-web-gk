# BƠ VÀNG BAKERY — HƯỚNG DẪN DEMO RENDER POSTGRESQL

## Phạm vi

- Đây là **bản demo dùng PostgreSQL mới**, không ghi hoặc xóa dữ liệu SQL Server trên máy.
- Các migrations trong thư mục `Migrations/` được tạo cho SQL Server **không được chạy trên PostgreSQL**. Bản demo dùng `EnsureCreatedAsync()` trên database PostgreSQL mới hoàn toàn.
- File ZIP mã nguồn **không chứa dữ liệu thực tế từ SSMS**. Bản demo tạo 10 mẫu bánh, mỗi mẫu 12 chiếc. Tài khoản/đơn hàng cũ không tự chuyển sang.
- Render PostgreSQL Free **hết hạn sau 30 ngày**; phải sao lưu dữ liệu quan trọng trước thời hạn.

## 1. Đưa mã nguồn mới lên GitHub

1. Giữ lại bản SQL Server đang chạy trên laptop và/hoặc tạo một branch Git mới tên `postgres-demo`.
2. Chép toàn bộ nội dung thư mục `Bo_Vang_Bakery_Postgres_Demo/` vào một thư mục dự án riêng trong repository; giữ nguyên `Dockerfile` ngang hàng với `.csproj`.
3. Commit và Push bản mới. Không đưa bất kỳ URL có mật khẩu hoặc tài khoản thực nào lên GitHub.
4. Trên Render, **tạo web service mới** cho branch demo để tránh thay đổi service SQL Server cũ; chọn Docker, Root Directory đúng vị trí `.csproj`, Dockerfile Path `./Dockerfile`, Port 10000.

## 2. Tạo Render PostgreSQL Free

1. Render Dashboard → New → Postgres (hoặc PostgreSQL).
2. Name: `bo-vang-bakery-demo-db`; Region: cùng vùng với web service demo; Plan: Free.
3. Chờ trạng thái Available và sao chép `Internal Database URL` trong Connections.
4. **Không gửi URL này cho người khác**: nó chứa cả mật khẩu database.

## 3. Environment của web service demo

Ở Render → web service demo → Environment → Add Environment Variable:

| Key | Value |
| --- | --- |
| `DATABASE_URL` | Dán nguyên `Internal Database URL`, bắt đầu bằng `postgresql://` hoặc `postgres://` |
| `BAKERY_DEMO_SETUP` | `true` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `SeedOwner__Email` | Email chủ quầy demo tự chọn |
| `SeedOwner__Password` | Mật khẩu demo mạnh, tối thiểu 8 ký tự, gồm hoa/thường/số |

**Chú ý:** Chỉ dùng mật khẩu demo riêng, không tái sử dụng mật khẩu cá nhân. Không đưa 5 giá trị ở trên vào file GitHub. Nếu chưa muốn tạo chủ quầy, bỏ qua cả hai biến `SeedOwner__*`.

Chọn Save, rebuild, and deploy. Database phải là một database demo **mới, trống**, không dùng với ứng dụng khác.

## 4. Kiểm tra

1. Render → Logs: kiểm tra không có lỗi kết nối Npgsql/Entity Framework/EnsureCreated.
2. Mở `https://<ten-web>.onrender.com/api/products` → kết quả JSON thành công, có 10 sản phẩm.
3. Mở `/api/categories` → danh mục bánh.
4. Mở `/pages/products.html`, thử thêm bánh vào giỏ, `/pages/cart.html`, đặt đơn hàng thử.
5. Đăng nhập chủ quầy demo và kiểm tra chức năng quản trị (nếu đã điền SeedOwner).

Nếu `api/products` trả 500, xem ngay **Runtime Logs**: kiểm tra `DATABASE_URL`, vùng Render, trạng thái database và quyền tạo bảng.

## 5. Giới hạn và khôi phục

- `EnsureCreated` chỉ tạo cấu trúc cho database mới. **Không** dùng `dotnet ef database update` với các migrations SQL Server cũ.
- Nếu database đã có bảng cũ không tương thích, không tự xóa dữ liệu; tạo một database demo mới hoặc yêu cầu trợ giúp.
- Để trở lại website SQL Server gốc, dùng lại branch và cấu hình web service cũ. Bản SQL Server ban đầu và SSMS trên laptop không bị chỉnh sửa.
- Để chuyển dữ liệu **thật** từ SQL Server sang PostgreSQL cần thêm bước xuất, chuyển kiểu dữ liệu, nhập bảng theo thứ tự khóa ngoại và kiểm chứng số bản ghi; **chưa thực hiện trong bản demo này**.
