let featured = products.slice(0, 4);
let container = document.getElementById("featured-products");

featured.forEach(function(product) {
    container.innerHTML += `
        <div class="product-card">
            <img class="product-image" src="${product.image}" alt="${product.name}">
            <h3>${product.name}</h3>
            <p class="price">${formatMoney(product.price)}</p>
            <a class="btn" href="products.html">Xem sản phẩm</a>
        </div>
    `;
});
