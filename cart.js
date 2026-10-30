function displayCart() {
    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    let container = document.getElementById("cart-list");
    let total = 0;

    container.innerHTML = "";

    if (cart.length === 0) {
        container.innerHTML = "<p>Giỏ hàng đang trống.</p>";
        document.getElementById("total").innerText = "0đ";
        return;
    }

    cart.forEach(function(product, index) {
        total += product.price;

        container.innerHTML += `
            <div class="cart-item">
                <img class="cart-image" src="${product.image}" alt="${product.name}">
                <div class="cart-info">
                    <strong>${product.name}</strong>
                    <p>${formatMoney(product.price)}</p>
                </div>
                <button class="delete-btn" onclick="removeCart(${index})">Xóa</button>
            </div>
        `;
    });

    document.getElementById("total").innerText = formatMoney(total);
}

function removeCart(index) {
    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    cart.splice(index, 1);
    localStorage.setItem("cart", JSON.stringify(cart));
    displayCart();
}

function checkout() {
    let cart = JSON.parse(localStorage.getItem("cart")) || [];

    if (cart.length === 0) {
        alert("Giỏ hàng đang trống!");
        return;
    }

    alert("Đặt hàng thành công! Cảm ơn bạn.");
    localStorage.removeItem("cart");
    displayCart();
}

displayCart();