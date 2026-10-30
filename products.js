let currentProducts = [...products];

function displayProducts(list) {
    let container = document.getElementById("product-list");
    container.innerHTML = "";

    if (list.length === 0) {
        container.innerHTML = "<p>Không tìm thấy sản phẩm phù hợp.</p>";
        return;
    }

    list.forEach(function(product) {
        container.innerHTML += `
            <div class="product-card">
                <img class="product-image" src="${product.image}" alt="${product.name}">
                <h3>${product.name}</h3>
                <p>${product.category}</p>
                <p class="price">${formatMoney(product.price)}</p>
                <button class="btn" onclick="addToCart(${product.id})">Thêm vào giỏ</button>
            </div>
        `;
    });
}

function searchProduct() {
    let keyword = document.getElementById("search").value.toLowerCase();

    currentProducts = products.filter(function(product) {
        return product.name.toLowerCase().includes(keyword);
    });

    displayProducts(currentProducts);
}

function filterCategory() {
    let category = document.getElementById("category").value;

    if (category === "all") {
        currentProducts = [...products];
    } else {
        currentProducts = products.filter(function(product) {
            return product.category === category;
        });
    }

    displayProducts(currentProducts);
}

function sortProduct() {
    let type = document.getElementById("sort").value;

    if (type === "low") {
        currentProducts.sort(function(a, b) {
            return a.price - b.price;
        });
    }

    if (type === "high") {
        currentProducts.sort(function(a, b) {
            return b.price - a.price;
        });
    }

    displayProducts(currentProducts);
}

function addToCart(id) {
    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    let product = products.find(function(item) {
        return item.id === id;
    });

    cart.push(product);
    localStorage.setItem("cart", JSON.stringify(cart));

    alert("Đã thêm " + product.name + " vào giỏ hàng!");
}

displayProducts(products);
