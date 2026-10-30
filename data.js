const defaultProducts = [
    { id: 1, name: "Bánh kem dâu tây", price: 120000, category: "Bánh kem", image: "images/banh-kem-dau-tay.jpg" },
    { id: 2, name: "Bánh quy kẹp kem", price: 100000, category: "Bánh ngọt", image: "images/banh-quy-kem.jpg" },
    { id: 3, name: "Bánh kem socola", price: 150000, category: "Bánh kem", image: "images/banh-kem-socola.jpg" },
    { id: 4, name: "Bánh sừng bò Croissant", price: 35000, category: "Bánh mì", image: "images/banh-croissant.jpg" },
    { id: 5, name: "Cupcake dâu tây", price: 40000, category: "Bánh ngọt", image: "images/cupcake-dau.jpg" },
    { id: 6, name: "Bánh Tiramisu", price: 100000, category: "Bánh ngọt", image: "images/tiramisu.jpg" }
];

function copyDefaultProducts() {
    return defaultProducts.map(function(item) { return {...item }; });
}

// Đọc dữ liệu đã lưu. Chỉ dùng dữ liệu mặc định khi CHƯA có gì được lưu
// (hoặc dữ liệu lưu bị hỏng), không ghi đè dữ liệu người dùng đã sửa.
function loadProducts() {
    try {
        const saved = JSON.parse(localStorage.getItem("products"));
        if (Array.isArray(saved)) {
            return saved;
        }
    } catch (error) {
        console.warn("Dữ liệu sản phẩm bị lỗi, dùng dữ liệu mặc định.", error);
    }
    return copyDefaultProducts();
}

let products = loadProducts();

function saveProducts() {
    try {
        localStorage.setItem("products", JSON.stringify(products));
        return true;
    } catch (error) {
        alert("Không lưu được dữ liệu (bộ nhớ trình duyệt đầy hoặc bị chặn).");
        return false;
    }
}

// Lần đầu truy cập: lưu luôn danh sách mặc định
if (localStorage.getItem("products") === null) {
    saveProducts();
}

// Khôi phục 6 sản phẩm ban đầu (dùng cho nút "Khôi phục dữ liệu" nếu cần)
function resetProducts() {
    products = copyDefaultProducts();
    saveProducts();
}

function formatMoney(number) {
    return Number(number).toLocaleString("vi-VN") + "đ";
}