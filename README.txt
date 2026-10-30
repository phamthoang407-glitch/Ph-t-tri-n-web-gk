SWEET BAKERY - WEBSITE BÁN BÁNH
BÀI KIỂM TRA GIỮA KỲ

1. GIỚI THIỆU

Sweet Bakery là website bán bánh được xây dựng phục vụ học tập và kiểm tra
giữa kỳ. Website giới thiệu cửa hàng, trưng bày sản phẩm, cung cấp giao diện
tìm kiếm, lọc, sắp xếp bánh, giỏ hàng và quản lý sản phẩm.

Mục tiêu: vận dụng HTML, CSS và JavaScript để xây dựng một website có nhiều
trang liên kết với nhau, giao diện dễ sử dụng và có xử lý dữ liệu cơ bản.

2. CÔNG NGHỆ SỬ DỤNG

- HTML: xây dựng cấu trúc và nội dung từng trang.
- CSS: định dạng màu sắc, bố cục, hình ảnh, biểu mẫu và danh sách sản phẩm.
- JavaScript: xử lý sự kiện, hiển thị và thao tác với dữ liệu.
- LocalStorage: lưu dữ liệu trên trình duyệt theo thiết kế của dự án.
- Không sử dụng Backend, API hoặc cơ sở dữ liệu phía máy chủ.

3. PHẠM VI VÀ CÁC TRANG CHỨC NĂNG

3.1. Trang chủ - index.html
- Hiển thị thương hiệu, hình ảnh giới thiệu và thông tin Sweet Bakery.
- Có khu vực sản phẩm nổi bật và nút chuyển đến trang sản phẩm.
- Có thanh điều hướng đến các trang còn lại.

3.2. Trang sản phẩm - products.html
- Có khu vực hiển thị danh sách bánh.
- Có ô tìm kiếm theo tên bánh.
- Có bộ lọc: tất cả danh mục, bánh kem, bánh ngọt, bánh mì.
- Có lựa chọn sắp xếp giá từ thấp đến cao hoặc cao đến thấp.
- Theo phạm vi dự án: cho phép thêm bánh vào giỏ hàng bằng JavaScript.

3.3. Trang giỏ hàng - cart.html
- Có khu vực hiển thị các sản phẩm trong giỏ.
- Hiển thị tổng tiền và nút Đặt hàng.
- Đặt hàng trong phạm vi bài tập là mô phỏng, không phải giao dịch thực tế.

3.4. Trang quản lý - admin.html
- Có biểu mẫu tên bánh, giá, danh mục và chọn tệp ảnh.
- Có khu vực xem trước ảnh, nút Lưu sản phẩm và nút Làm mới.
- Có khu vực hiển thị danh sách sản phẩm quản lý.
- Theo phạm vi dự án: thực hiện thêm, xem, sửa và xóa sản phẩm.
- Trang quản lý phục vụ minh họa chức năng; chưa có cơ chế xác thực và
  phân quyền phía máy chủ.

Lưu ý về mức độ kiểm tra:
Bộ tệp dùng để biên soạn README gồm 4 trang HTML và README ban đầu.
Các tệp CSS, JavaScript và ảnh được tham chiếu chưa có trong bộ tệp này.
Vì vậy, các thao tác xử lý dữ liệu cần được chạy thử với mã nguồn đầy đủ;
README không xác nhận các chức năng JavaScript đã kiểm thử thành công.

4. CẤU TRÚC THƯ MỤC CẦN CÓ

Đặt 4 trang HTML và README.txt ở thư mục gốc của dự án.

- index.html: trang chủ.
- products.html: trang sản phẩm.
- cart.html: trang giỏ hàng.
- admin.html: trang quản lý.
- README.txt: giới thiệu, kế hoạch và hướng dẫn sử dụng.

Thư mục css/:
- style.css: định dạng giao diện chung.
- admin.css: định dạng riêng cho trang quản lý.

Thư mục js/ (theo đường dẫn đang được gọi trong HTML):
- data.js: phần dữ liệu dùng chung.
- main.js: xử lý trang chủ.
- products.js: xử lý trang sản phẩm.
- cart.js: xử lý giỏ hàng.
- admin.js: xử lý quản lý sản phẩm.

Thư mục images/:
- bakery-avatar.png: ảnh được trang chủ và quản lý sử dụng.
- logo-bakery.jpg: logo được trang sản phẩm và giỏ hàng sử dụng.
- Các ảnh bánh được mã nguồn dữ liệu tham chiếu, nếu có.

Tệp trang chủ trong bộ gửi hiện có tên index(1).html. Khi đóng gói và chạy,
cần đổi tên thành index.html vì các liên kết trong website sử dụng tên này.

5. KẾ HOẠCH THỰC HIỆN WEBSITE

Kế hoạch dưới đây đề xuất tiến độ 7 buổi cho phạm vi bài giữa kỳ.
Đây là kế hoạch tham khảo, không phải nhật ký xác nhận thời gian đã làm.
Có thể điều chỉnh theo lịch học và thời hạn nộp bài.

Giai đoạn 1 - Xác định đề tài và yêu cầu (buổi 1)
Công việc:
- Chọn đề tài website bán bánh Sweet Bakery.
- Xác định nhu cầu người xem: tìm bánh, xem giá, chọn bánh và xem giỏ hàng.
- Xác định nhu cầu quản lý: thêm, xem, sửa và xóa sản phẩm.
- Giới hạn bài tập ở HTML, CSS, JavaScript và lưu trữ trên trình duyệt.
Kết quả cần đạt: danh sách chức năng và danh sách 4 trang của website.

Giai đoạn 2 - Thiết kế giao diện và tổ chức thư mục (buổi 2)
Công việc:
- Phác thảo thanh điều hướng, phần nội dung và chân trang.
- Chọn màu sắc, kiểu chữ và hình ảnh phù hợp cửa hàng bánh.
- Thiết kế thẻ sản phẩm, bộ tìm kiếm/lọc và biểu mẫu quản lý.
- Tổ chức các thư mục css, js và images.
Kết quả cần đạt: bố cục các trang và cấu trúc thư mục rõ ràng.

Giai đoạn 3 - Xây dựng HTML và CSS (buổi 3-4)
Công việc:
- Tạo trang chủ, sản phẩm, giỏ hàng và quản lý.
- Liên kết các trang bằng thanh điều hướng.
- Định dạng bố cục, nút bấm, hình ảnh và biểu mẫu.
- Điều chỉnh giao diện để nội dung dễ đọc trên các kích thước màn hình.
Kết quả cần đạt: 4 trang có giao diện thống nhất, liên kết và ảnh đúng đường dẫn.

Giai đoạn 4 - Xây dựng xử lý JavaScript (buổi 5-6)
Công việc:
- Chuẩn bị dữ liệu bánh: mã, tên, giá, danh mục và ảnh.
- Hiển thị danh sách bánh và sản phẩm nổi bật.
- Xử lý tìm kiếm, lọc danh mục và sắp xếp giá.
- Xử lý thêm vào giỏ, hiển thị giỏ, tính tổng và đặt hàng mô phỏng.
- Xử lý thêm, sửa, xóa sản phẩm và xem trước ảnh.
- Kiểm tra dữ liệu nhập: tên không rỗng, giá hợp lệ và ảnh phù hợp.
- Lưu và đọc dữ liệu bằng LocalStorage.
Kết quả cần đạt: các thao tác chạy đúng và dữ liệu cần lưu vẫn còn sau tải lại.

Giai đoạn 5 - Kiểm tra, hoàn thiện và nộp bài (buổi 7)
Công việc:
- Chạy các tình huống kiểm tra ở mục 7.
- Sửa lỗi đường dẫn, giao diện và xử lý dữ liệu được phát hiện.
- Hoàn thiện README, kiểm tra đủ mã nguồn và tài nguyên.
- Chuẩn bị phần giới thiệu và trình diễn các chức năng chính.
Kết quả cần đạt: bộ bài nộp đầy đủ, mở được và có hướng dẫn sử dụng.

Thứ tự ưu tiên khi thời gian hạn chế:
1. Hoàn thiện 4 trang và các liên kết điều hướng.
2. Hiển thị sản phẩm và chức năng quản lý cơ bản.
3. Tìm kiếm, lọc, sắp xếp và giỏ hàng.
4. Kiểm tra dữ liệu, lưu trữ và sửa lỗi.
5. Hoàn thiện hình ảnh, bố cục và tài liệu.

6. HƯỚNG DẪN CHẠY VÀ SỬ DỤNG

Chuẩn bị:
- Có trình duyệt web và toàn bộ thư mục mã nguồn.
- Kiểm tra đủ các tệp tại mục 4.
- Đổi index(1).html thành index.html nếu tên này vẫn còn trong thư mục.

Cách chạy ưu tiên:
1. Mở cả thư mục dự án trong Visual Studio Code.
2. Nếu đã cài Live Server, mở index.html bằng Open with Live Server.
3. Giữ cùng địa chỉ và cổng khi chuyển trang hoặc mở lại để dùng chung
   dữ liệu LocalStorage của website.

Cách chạy trực tiếp:
- Có thể mở index.html bằng trình duyệt. Tuy nhiên, việc dùng LocalStorage
  giữa các tệp mở trực tiếp có thể khác nhau theo trình duyệt; nên chạy qua
  máy chủ cục bộ khi trình diễn chức năng lưu dữ liệu và giỏ hàng.

Luồng sử dụng dự kiến:
1. Trang chủ: xem giới thiệu và chọn Xem sản phẩm.
2. Sản phẩm: tìm tên bánh, chọn danh mục, sắp xếp giá và thêm bánh vào giỏ.
3. Giỏ hàng: xem danh sách, tổng tiền và thử Đặt hàng.
4. Quản lý: nhập thông tin bánh, chọn ảnh, lưu và thử sửa/xóa sản phẩm.
5. Tải lại trang để kiểm tra dữ liệu đã lưu.

7. DANH SÁCH KIỂM TRA TRƯỚC KHI NỘP

Đây là các tình huống cần chạy thử, chưa phải kết quả kiểm thử.

[ ] Mở trang chủ; menu chuyển đúng cả 4 trang.
[ ] CSS và hình ảnh được tải, không có đường dẫn bị thiếu.
[ ] Danh sách sản phẩm và sản phẩm nổi bật hiển thị đúng dữ liệu.
[ ] Tìm một tên bánh có tồn tại và một tên không tồn tại.
[ ] Lọc từng danh mục, sau đó chọn lại Tất cả danh mục.
[ ] Sắp xếp giá tăng dần và giảm dần cho kết quả đúng.
[ ] Thêm bánh vào giỏ, kiểm tra sản phẩm và tổng tiền.
[ ] Kiểm tra giỏ rỗng và thao tác Đặt hàng trong cả hai trường hợp.
[ ] Thêm sản phẩm hợp lệ, kiểm tra xuất hiện trong danh sách.
[ ] Sửa sản phẩm, kiểm tra thông tin mới; xóa đúng sản phẩm được chọn.
[ ] Thử để trống tên, giá không hợp lệ và kiểm tra thông báo.
[ ] Chọn ảnh, kiểm tra xem trước ảnh và nút Làm mới biểu mẫu.
[ ] Tải lại trang, kiểm tra sản phẩm và giỏ hàng cần lưu.
[ ] Kiểm tra giao diện ở màn hình rộng và hẹp.
[ ] Kiểm tra Console của trình duyệt để phát hiện lỗi JavaScript.
[ ] Đóng gói đầy đủ HTML, CSS, JavaScript, ảnh và README.

8. TIÊU CHÍ HƯỚNG TỚI

1. Giao diện HTML có cấu trúc rõ ràng.
2. Thiết kế CSS thống nhất và dễ đọc.
3. Có chức năng thêm, xem, sửa, xóa sản phẩm.
4. Có JavaScript xử lý sự kiện và dữ liệu.
5. Có tìm kiếm, lọc, sắp xếp và kiểm tra dữ liệu đầu vào.
6. Hoạt động ổn định trong phạm vi bài tập.
7. Dễ sử dụng, điều hướng rõ ràng.
8. Có trang quản lý để mở rộng chức năng website.

9. GIỚI HẠN VÀ HƯỚNG PHÁT TRIỂN

Giới hạn của phiên bản học tập:
- Dữ liệu LocalStorage chỉ nằm trên trình duyệt đang sử dụng, không tự
  đồng bộ giữa nhiều máy và có thể mất khi xóa dữ liệu website.
- Chưa có máy chủ xử lý đơn hàng hoặc thanh toán thực tế.
- Chưa có xác thực và phân quyền quản trị phía máy chủ.
- Cần kiểm tra kích thước ảnh để tránh vượt dung lượng lưu trữ trình duyệt.

Hướng phát triển sau bài giữa kỳ:
- Thêm trang chi tiết sản phẩm và thông tin người đặt hàng.
- Bổ sung quản lý đơn hàng.
- Kết nối Backend và cơ sở dữ liệu khi mở rộng dự án.
- Bổ sung đăng nhập, phân quyền và kiểm tra dữ liệu phía máy chủ.
- Tiếp tục cải thiện giao diện trên điện thoại.
