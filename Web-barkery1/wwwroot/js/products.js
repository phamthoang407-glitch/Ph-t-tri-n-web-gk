function productSeoUrl(p) {
  const slug = String(p.name || "banh").toLowerCase().replace(/đ/g, "d")
    .normalize("NFD").replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "") || "banh";
  return `/san-pham/${slug}-${p.id}`;
}
let productsCache = [];
let catalogPage = 1;
const CATALOG_PAGE_SIZE = 6;

function productCard(p) {
  const badge =
    p.status === "Còn hàng"
      ? "badge--green"
      : p.status === "Sắp hết"
        ? "badge--orange"
        : "badge--red";
  return `<article class="product-card">
    <a href="${productSeoUrl(p)}" class="product-card__image">
      <img src="${safeImage(p.image)}" alt="${escapeHtml(p.name)}" loading="lazy"><span class="badge ${badge}">${escapeHtml(p.status)}</span>
    </a>
    <div class="product-card__body">
      <div class="product-card__meta"><span class="rating">★ ${escapeHtml(p.rating)}</span><span class="price">${formatVND(p.price)}</span></div>
      <h3>${escapeHtml(p.name)}</h3><div class="card-actions"><a class="btn btn--soft" href="${productSeoUrl(p)}">Chi tiết</a><button class="btn btn--primary" ${p.stock <= 0 ? 'disabled' : `onclick="addToCart(${p.id})"`}>${p.stock <= 0 ? 'Tạm hết hàng' : 'Thêm giỏ'}</button></div>
    </div>
  </article>`;
}
async function fetchProducts(params = "") {
  const result = await api.get("/products" + params);
  productsCache = result.data;
  return productsCache;
}
async function renderFeatured() {
  const el = document.getElementById("featured-products");
  if (!el) return;
  try {
    el.innerHTML = `<div class="empty-state">Đang tải sản phẩm...</div>`;
    const list = await fetchProducts("");
    el.innerHTML = list.length ? list.slice(0, 4).map(productCard).join("") : `<p class="empty-state">Chưa có sản phẩm. Chủ quầy có thể thêm bánh trong trang quản lý.</p>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">${escapeHtml(e.message)}</div>`;
  }
}
async function renderCatalog() {
  const el = document.getElementById("catalog-products");
  if (!el) return;
  try {
    el.innerHTML = `<div class="empty-state">Đang tải sản phẩm...</div>`;
    const params = new URLSearchParams(location.search);
    const cat = params.get("category"),
      search = params.get("search") || "";
    const query = new URLSearchParams();
    if (cat) query.set("category", cat);
    if (search) query.set("search", search);
    let list = await fetchProducts(
      query.toString() ? "?" + query.toString() : "",
    );
    const range = params.get("price");
    if(range === "low") list = list.filter(p=>p.price<100000);
    if(range === "mid") list = list.filter(p=>p.price>=100000&&p.price<=300000);
    if(range === "high") list = list.filter(p=>p.price>300000);
    const sort = document.getElementById("sort-products")?.value;
    if (sort === "price-low") list.sort((a, b) => a.price - b.price);
    if (sort === "price-high") list.sort((a, b) => b.price - a.price);
    const count = document.getElementById("catalog-count");
    if (count) count.textContent = list.length + " sản phẩm";
    const totalPages = Math.max(1, Math.ceil(list.length / CATALOG_PAGE_SIZE));
    catalogPage = Math.min(Math.max(catalogPage, 1), totalPages);
    const slice = list.slice((catalogPage-1)*CATALOG_PAGE_SIZE, catalogPage*CATALOG_PAGE_SIZE);
    el.innerHTML = list.length
      ? slice.map(productCard).join("")
      : `<div class="empty-state">Không tìm thấy sản phẩm phù hợp.</div>`;
    const pageBox = document.getElementById("catalog-pagination");
    if(pageBox) {
      pageBox.innerHTML = list.length > CATALOG_PAGE_SIZE
        ? `<button type="button" data-page="${catalogPage-1}" ${catalogPage<=1?'disabled':''}>← Trước</button><span>Trang ${catalogPage}/${totalPages}</span><button type="button" data-page="${catalogPage+1}" ${catalogPage>=totalPages?'disabled':''}>Sau →</button>`
        : "";
    }
  } catch (e) {
    el.innerHTML = `<div class="empty-state">${escapeHtml(e.message)}</div>`;
  }
}
async function renderProductDetail() {
  const el = document.getElementById("product-detail");
  if (!el) return;
  try {
    const id = window.BAKERY_PRODUCT_ID || new URLSearchParams(location.search).get("id");
    if (!id) return;
    const p = (await api.get("/products/" + id)).data;
    el.innerHTML = `<div class="panel product-detail-panel">
      <div style="padding:18px;background:var(--cream-2)"><img style="width:100%;height:360px;object-fit:cover;border-radius:14px" src="${safeImage(p.image)}" alt="${escapeHtml(p.name)}"></div>
      <div style="padding:48px"><span class="eyebrow">${escapeHtml(p.category)}</span><h1 style="font-size:34px;line-height:1.2">${escapeHtml(p.name)}</h1><div style="margin:14px 0;color:#C98925">★★★★★ <span style="color:var(--muted);font-size:11px">${escapeHtml(p.rating)} / 5</span></div><div class="price" style="font-size:27px">${formatVND(p.price)}</div>
      <p style="color:var(--muted);font-size:13px;margin:20px 0">${escapeHtml(p.description)}</p><div style="padding:18px 0;border-top:1px solid var(--line);border-bottom:1px solid var(--line)"><strong style="font-size:12px">Thông tin sản phẩm</strong><p style="font-size:11px;color:var(--muted);margin-top:7px">Sản phẩm được cập nhật theo số lượng còn trong cửa hàng.</p></div>
      <div style="display:flex;gap:10px;margin-top:24px"><button class="btn btn--primary" style="flex:1" onclick="addToCart(${p.id})">Thêm vào giỏ hàng</button></div></div></div>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">${escapeHtml(e.message)}</div>`;
  }
}
document.addEventListener("DOMContentLoaded", () => {
  renderFeatured();
  renderCatalog();
  renderProductDetail();
  document.getElementById("catalog-pagination")?.addEventListener("click", e => {
    const button = e.target.closest("button[data-page]");
    if (!button || button.disabled) return;
    catalogPage = Number(button.dataset.page);
    renderCatalog();
    document.getElementById("catalog-products")?.scrollIntoView({behavior:"smooth",block:"start"});
  });
  document
    .getElementById("sort-products")
    ?.addEventListener("change", () => { catalogPage=1; renderCatalog(); });
});
window.fetchProducts = fetchProducts;
