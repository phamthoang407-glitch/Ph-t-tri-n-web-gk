// Ảnh người dùng vừa chọn (dạng chuỗi data URL). Rỗng = chưa chọn ảnh mới.
let currentImage = "";
let isProcessingImage = false;

const DEFAULT_IMAGE = "images/bakery-avatar.png";
const MAX_IMAGE_SIZE = 600; // chiều rộng/cao tối đa (px) để ảnh nhẹ, vừa LocalStorage

// Thu nhỏ ảnh rồi trả về chuỗi data URL (JPEG)
function resizeImage(file, callback) {
    let reader = new FileReader();

    reader.onload = function(event) {
        let img = new Image();

        img.onload = function() {
            let scale = Math.min(1, MAX_IMAGE_SIZE / Math.max(img.width, img.height));
            let canvas = document.createElement("canvas");
            canvas.width = Math.round(img.width * scale);
            canvas.height = Math.round(img.height * scale);

            let ctx = canvas.getContext("2d");
            ctx.fillStyle = "#ffffff"; // nền trắng cho ảnh PNG trong suốt
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

            callback(canvas.toDataURL("image/jpeg", 0.8));
        };

        img.onerror = function() {
            callback(null);
        };

        img.src = event.target.result;
    };

    reader.onerror = function() {
        callback(null);
    };

    reader.readAsDataURL(file);
}

function showPreview(src) {
    let preview = document.getElementById("image-preview");

    if (src) {
        preview.src = src;
        preview.hidden = false;
    } else {
        preview.removeAttribute("src");
        preview.hidden = true;
    }
}

// Gọi khi người dùng chọn file ảnh
function previewImage() {
    let input = document.getElementById("image-file");
    let file = input.files[0];

    if (!file) {
        return;
    }

    if (!file.type.startsWith("image/")) {
        alert("Vui lòng chọn file ảnh (jpg, png, webp...)!");
        input.value = "";
        return;
    }

    isProcessingImage = true;

    resizeImage(file, function(dataUrl) {
        isProcessingImage = false;

        if (!dataUrl) {
            alert("Không đọc được ảnh này, vui lòng chọn ảnh khác!");
            input.value = "";
            return;
        }

        currentImage = dataUrl;
        showPreview(dataUrl);
    });
}

function displayAdminProducts() {
    let container = document.getElementById("admin-list");

    let html = `
        <table class="admin-table">
            <tr>
                <th>STT</th>
                <th>Ảnh</th>
                <th>Tên bánh</th>
                <th>Giá</th>
                <th>Danh mục</th>
                <th>Thao tác</th>
            </tr>
    `;

    products.forEach(function(product, index) {
        html += `
            <tr>
                <td>${index + 1}</td>
                <td><img class="admin-thumb" src="${product.image}" alt="${product.name}"></td>
                <td>${product.name}</td>
                <td>${formatMoney(product.price)}</td>
                <td>${product.category}</td>
                <td>
                    <button class="edit-btn" onclick="editProduct(${product.id})">Sửa</button>
                    <button class="delete-btn" onclick="deleteProduct(${product.id})">Xóa</button>
                </td>
            </tr>
        `;
    });

    html += "</table>";
    container.innerHTML = html;
}

function saveProduct() {
    let id = document.getElementById("edit-id").value;
    let name = document.getElementById("name").value.trim();
    let price = Number(document.getElementById("price").value);
    let category = document.getElementById("product-category").value;

    if (name === "" || price <= 0) {
        alert("Vui lòng nhập tên và giá hợp lệ!");
        return;
    }

    if (isProcessingImage) {
        alert("Ảnh đang được xử lý, vui lòng chờ một chút rồi bấm lưu lại!");
        return;
    }

    if (id === "") {
        // THÊM MỚI: dùng ảnh đã chọn, nếu chưa chọn thì dùng ảnh mặc định
        let newProduct = {
            id: Date.now(),
            name: name,
            price: price,
            category: category,
            image: currentImage || DEFAULT_IMAGE
        };

        products.push(newProduct);

        if (!saveProducts()) {
            products.pop(); // lưu thất bại (thường do ảnh quá nặng) -> hoàn tác
            return;
        }

        alert("Thêm sản phẩm thành công!");
    } else {
        // SỬA: chỉ đổi ảnh khi người dùng chọn ảnh mới, còn không giữ ảnh cũ
        let product = products.find(function(item) {
            return item.id == id;
        });

        let oldData = {...product };

        product.name = name;
        product.price = price;
        product.category = category;

        if (currentImage) {
            product.image = currentImage;
        }

        if (!saveProducts()) {
            Object.assign(product, oldData); // hoàn tác
            return;
        }

        alert("Cập nhật sản phẩm thành công!");
    }

    clearForm();
    displayAdminProducts();
}

function editProduct(id) {
    let product = products.find(function(item) {
        return item.id === id;
    });

    document.getElementById("edit-id").value = product.id;
    document.getElementById("name").value = product.name;
    document.getElementById("price").value = product.price;
    document.getElementById("product-category").value = product.category;

    // Hiện ảnh hiện tại; chỉ thay khi chọn file mới
    currentImage = "";
    document.getElementById("image-file").value = "";
    showPreview(product.image);

    window.scrollTo({ top: 0, behavior: "smooth" });
}

function deleteProduct(id) {
    let answer = confirm("Bạn có chắc muốn xóa sản phẩm này không?");

    if (answer) {
        products = products.filter(function(product) {
            return product.id !== id;
        });

        saveProducts();
        displayAdminProducts();
    }
}

function clearForm() {
    document.getElementById("edit-id").value = "";
    document.getElementById("name").value = "";
    document.getElementById("price").value = "";
    document.getElementById("product-category").value = "Bánh kem";
    document.getElementById("image-file").value = "";
    currentImage = "";
    showPreview("");
}

displayAdminProducts();