let checkoutCart = [];
async function renderCheckout() {
  const wrap = document.getElementById("checkout-items");
  if (!wrap) return;
  try {
    wrap.innerHTML = "<p>Đang tải đơn hàng...</p>";
    checkoutCart = await fetchCart();
    if (!checkoutCart.length) {
      wrap.innerHTML = '<p class="empty-state">Giỏ hàng đang trống.</p>';
      return;
    }
    let total = 0;
    wrap.innerHTML = checkoutCart
      .map((i) => {
        const sum = i.price * i.quantity;
        total += sum;
        return `<div class="checkout-line"><img src="${safeImage(i.image)}" alt="${escapeHtml(i.name)}"><div><b>${escapeHtml(i.name)}</b><small>${i.quantity} × ${formatVND(i.price)}</small></div><strong>${formatVND(sum)}</strong></div>`;
      })
      .join("");
    document.getElementById("checkout-total").textContent = formatVND(total);
  } catch (e) {
    wrap.innerHTML = `<p class="empty-state">${escapeHtml(e.message)}</p>`;
  }
}
async function goCheckout() {
  const cart = await fetchCart();
  if (!cart.length) {
    showToast("Giỏ hàng đang trống");
    return;
  }
  location.href = BASE + "pages/checkout.html";
}
async function placeOrder() {
  const name = document.getElementById("customer-name")?.value.trim();
  const phone = document.getElementById("customer-phone")?.value.trim();
  const address = document.getElementById("customer-address")?.value.trim();
  const button = document.querySelector(".place-order");
  if (button?.disabled) return;
  if (!name || !phone || !address) {
    showToast("Vui lòng điền đủ thông tin nhận hàng");
    return;
  }
  button?.setAttribute("disabled", "disabled");
  if (button) button.textContent = "Đang xử lý...";
  try {
    const payment = "COD";
    const result = await api.post("/orders", {
      customer_name: name,
      customer_phone: phone,
      shipping_address: address,
      payment_method: payment,
      ...(localStorage.getItem("bakery_token")
        ? {}
        : {
            items: getGuestCart().map((item) => ({
              product_id: item.product_id,
              quantity: item.quantity,
            })),
          }),
    });
    if (!localStorage.getItem("bakery_token")) saveGuestCart([]);
    sessionStorage.setItem("bakery_last_order", JSON.stringify(result.data));
    location.href =
      BASE +
      "pages/order-tracking.html" +
      (result.data.guest_tracking_token
        ? "?token=" + encodeURIComponent(result.data.guest_tracking_token)
        : "?id=" + result.data.id);
  } catch (e) {
    showToast(e.message);
    if (button) {
      button.disabled = false;
      button.textContent = "Xác nhận đặt hàng";
    }
  }
}
document.addEventListener("DOMContentLoaded", renderCheckout);
window.goCheckout = goCheckout;
window.placeOrder = placeOrder;
