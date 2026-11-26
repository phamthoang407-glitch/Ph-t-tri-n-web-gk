// Dữ liệu demo sản phẩm/danh mục/cửa hàng/tin tức đã được chuyển sang REST API.
// File này chỉ giữ các hàm dùng chung để không phá cấu trúc frontend cũ.
const formatVND = (n) =>
  new Intl.NumberFormat("vi-VN").format(Number(n) || 0) + "đ";
window.formatVND = formatVND;
