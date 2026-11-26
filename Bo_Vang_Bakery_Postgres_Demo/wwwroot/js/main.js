const BASE = "/";

document.addEventListener("DOMContentLoaded", async () => {
  await loadPart("header", "components/header.html");
  await loadPart("footer", "components/footer.html");
  setupNav();
  await updateCartCount();
  setupSearch();
  setupMobileNav();
  markRole();
  const accountLink = document.querySelector('.header-actions a[href$="login.html"]');
  if(accountLink && currentUser()) accountLink.href = BASE + "pages/account.html";
  if(currentUser()?.role === "ADMIN") {
    const link=document.createElement("a"); link.textContent="Quản lý";link.href=BASE+"pages/dashboard-"+"owner"+".html";
    document.querySelector(".desktop-nav")?.append(link);
  }
  try {
    const categories=(await api.get("/categories")).data;
    const sidebar=document.getElementById("category-filter");
    if(sidebar) sidebar.innerHTML='<h4>DANH MỤC</h4><a class="filter-link" href="/san-pham">Tất cả sản phẩm</a>'+categories.map(c=>`<a class="filter-link" href="/san-pham?category=${encodeURIComponent(c.id)}">${escapeHtml(c.name)}</a>`).join("");
    const grid=document.querySelector(".category-grid");
    if(grid) grid.innerHTML=categories.slice(0,4).map(c=>`<a class="category-card" href="/san-pham?category=${encodeURIComponent(c.id)}"><div class="category-card__icon">🥐</div><div><h3>${escapeHtml(c.name)}</h3><p>${escapeHtml(c.description||"Khám phá bánh ngon")}</p></div></a>`).join("");
  } catch(e) { showToast(e.message); }

});

async function updateStoreAddress() {
  const address = document.querySelector("[data-store-address]");
  const hours = document.querySelector("[data-store-hours]");
  if (!address && !hours) return;
  try {
    const store = (await api.get("/stores")).data[0];
    if (address)
      address.textContent =
        store?.address || "Chưa có thông tin vị trí quầy bánh.";
    if (hours)
      hours.textContent = store
        ? `Giờ mở cửa: ${store.opening_time}`
        : "Chưa có giờ mở cửa.";
  } catch (e) {
    if (address) address.textContent = "Không tải được vị trí quầy bánh.";
    if (hours) hours.textContent = "Không tải được giờ mở cửa.";
    console.warn("Không tải được vị trí quầy bánh:", e);
  }
}

async function loadPart(id, file) {
  const el = document.getElementById(id);
  if (!el) return;
  try {
    const res = await fetch(BASE + file);
    if (!res.ok) throw new Error(res.status);
    const html = await res.text();
    el.innerHTML = html.replaceAll("{{BASE}}", BASE);
  } catch (e) {
    console.warn("Không tải được component:", file, e);
  }
}
function setupNav() {
  const page = document.body.dataset.page;
  document
    .querySelectorAll("[data-nav]")
    .forEach((a) => a.classList.toggle("active", a.dataset.nav === page));
  document.querySelector(".search-toggle")?.addEventListener("click", () => {
    document.querySelector(".search-panel")?.classList.toggle("open");
    document.getElementById("global-search")?.focus();
  });
  document.querySelector(".menu-toggle")?.addEventListener("click", () => {
    document.querySelector(".desktop-nav")?.classList.toggle("mobile-open");
  });
}
function setupSearch() {
  const input = document.getElementById("global-search");
  document.querySelector(".search-panel .btn")?.addEventListener("click", e=>{
    e.preventDefault(); location.href=BASE+"san-pham?search="+encodeURIComponent(input?.value.trim()||"");
  });
  input?.addEventListener("keydown", (e) => {
    if (e.key === "Enter" && input.value.trim()) {
      location.href =
        BASE +
        "san-pham?search=" +
        encodeURIComponent(input.value.trim());
    }
  });
}
function setupMobileNav() {
  const nav = document.getElementById("mobile-nav");
  if (!nav) return;
  nav.innerHTML = `<div class="mobile-nav">
    <a href="${BASE}index.html">⌂<span>Trang chủ</span></a>
    <a href="${BASE}san-pham">▦<span>Danh mục</span></a>
    <a href="${BASE}pages/cart.html">🛒<span>Giỏ hàng</span></a>
    <a href="${BASE}pages/order-tracking.html">◷<span>Đơn hàng</span></a>
    <a href="${BASE}pages/account.html">◯<span>Tài khoản</span></a>
  </div>`;
}
async function updateCartCount() {
  const els = document.querySelectorAll(".cart-count");
  if (!els.length) return;
  if (!localStorage.getItem("bakery_token")) {
    const count = getGuestCart().reduce(
      (sum, item) => sum + (Number(item.quantity) || 0),
      0,
    );
    els.forEach((el) => (el.textContent = String(count)));
    return;
  }
  try {
    const result = await api.get("/cart");
    const count = result.data.reduce((sum, item) => sum + item.quantity, 0);
    els.forEach((el) => (el.textContent = count));
  } catch {
    els.forEach((el) => (el.textContent = "0"));
  }
}
async function addToCart(id, qty = 1) {
  if (!localStorage.getItem("bakery_token")) {
    try {
      const quantity = Number(qty);
      if (!Number.isSafeInteger(quantity) || quantity < 1 || quantity > 999)
        throw new Error("Số lượng không hợp lệ.");
      const product = (await api.get("/products/" + Number(id))).data;
      if (product.status === "Hết hàng")
        throw new Error("Sản phẩm đã hết hàng.");
      const cart = getGuestCart();
      const item = cart.find((entry) => entry.product_id === product.id);
      if (item) {
        if (item.quantity + quantity > 999)
          throw new Error("Số lượng tối đa là 999.");
        item.quantity += quantity;
      } else {
        cart.push({ product_id: product.id, quantity });
      }
      saveGuestCart(cart);
      await updateCartCount();
      showToast("✓ Đã thêm vào giỏ hàng");
    } catch (e) {
      showToast(e.message);
    }
    return;
  }
  try {
    await api.post("/cart", { product_id: Number(id), quantity: Number(qty) });
    await updateCartCount();
    showToast("✓ Đã thêm vào giỏ hàng");
  } catch (e) {
    showToast(e.message);
  }
}
function showToast(msg) {
  let t = document.getElementById("toast");
  if (!t) {
    t = document.createElement("div");
    t.id = "toast";
    t.className = "toast";
    t.setAttribute("role", "status");
    document.body.append(t);
  }
  t.textContent = msg;
  t.classList.add("show");
  clearTimeout(window.__toast);
  window.__toast = setTimeout(() => t.classList.remove("show"), 2600);
}
function markRole() {
  const role = localStorage.getItem("bakery_role");
  document
    .querySelectorAll("[data-role-badge]")
    .forEach(
      (x) =>
        (x.textContent =
          role === "ADMIN"
            ? "CHỦ QUẦY"
            : role === "STAFF"
              ? "NHÂN VIÊN"
              : "KHÁCH HÀNG"),
    );
}
window.BASE = BASE;
window.updateCartCount = updateCartCount;
window.addToCart = addToCart;
window.showToast = showToast;
window.formatVND = formatVND;

document.addEventListener("error", e=>{if(e.target.tagName==="IMG"&&!e.target.dataset.fallback){e.target.dataset.fallback="1";e.target.src="/images/cake-placeholder.svg";}},true);
