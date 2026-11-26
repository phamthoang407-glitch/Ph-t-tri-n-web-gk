function escapeHtml(value) {
  return String(value ?? "").replace(
    /[&<>"']/g,
    (m) =>
      ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[
        m
      ],
  );
}
function safeImage(value) {
  try {
    const u = new URL(String(value || ""), location.href);
    return ["http:", "https:"].includes(u.protocol) ? escapeHtml(u.href) : "";
  } catch {
    return "";
  }
}
window.escapeHtml = escapeHtml;
window.safeImage = safeImage;
const API_BASE_URL = "/api";
if (window.BAKERY_USER) {
 localStorage.setItem("bakery_token", "cookie-session");
 localStorage.setItem("bakery_user", JSON.stringify(window.BAKERY_USER));
 localStorage.setItem("bakery_role", window.BAKERY_USER.role);
} else {
 ["bakery_token","bakery_user","bakery_role"].forEach(k => localStorage.removeItem(k));
}
const GUEST_CART_KEY = "bakery_guest_cart";

function getGuestCart() {
  try {
    const cart = JSON.parse(localStorage.getItem(GUEST_CART_KEY) || "[]");
    return Array.isArray(cart) ? cart : [];
  } catch {
    return [];
  }
}

function saveGuestCart(cart) {
  localStorage.setItem(GUEST_CART_KEY, JSON.stringify(cart));
}

async function apiRequest(path, options = {}) {
  const controller = new AbortController();
  const timeout = setTimeout(
    () => controller.abort(),
    options.timeout || 10000,
  );
  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {}),
  };
  const token = localStorage.getItem("bakery_token");
  headers["X-CSRF-TOKEN"] = document.querySelector('meta[name="csrf-token"]')?.content || "";

  try {
    const response = await fetch(API_BASE_URL + path, {
      ...options,
      headers,
      signal: controller.signal,
    });
    const text = await response.text();
    let body = {};
    try {
      body = text ? JSON.parse(text) : {};
    } catch {
      body = { message: text };
    }

    if (!response.ok || body.success === false) {
      const error = new Error(body.message || `HTTP ${response.status}`);
      error.status = response.status;
      if (response.status === 401) {
        localStorage.removeItem("bakery_token");
        localStorage.removeItem("bakery_user");
        localStorage.removeItem("bakery_role");
      }
      throw error;
    }
    return body;
  } catch (error) {
    if (error.name === "AbortError") {
      const e = new Error("Yêu cầu mất quá nhiều thời gian. Vui lòng thử lại.");
      e.code = "TIMEOUT";
      throw e;
    }
    if (error instanceof TypeError) {
      const e = new Error("Không thể kết nối đến máy chủ.");
      e.code = "NETWORK";
      throw e;
    }
    throw error;
  } finally {
    clearTimeout(timeout);
  }
}

const api = {
  get: (path) => apiRequest(path),
  post: (path, data) =>
    apiRequest(path, { method: "POST", body: JSON.stringify(data) }),
  put: (path, data) =>
    apiRequest(path, { method: "PUT", body: JSON.stringify(data) }),
  delete: (path) => apiRequest(path, { method: "DELETE" }),
};

function saveAuth(data) {
  localStorage.setItem("bakery_token", "cookie-session");
  localStorage.setItem("bakery_user", JSON.stringify(data.user));
  localStorage.setItem("bakery_role", data.user.role);
}
function clearAuth() {
  localStorage.removeItem("bakery_token");
  localStorage.removeItem("bakery_user");
  localStorage.removeItem("bakery_role");
}
function currentUser() {
  try {
    return JSON.parse(localStorage.getItem("bakery_user") || "null");
  } catch {
    return null;
  }
}
function requireRole(...roles) {
  const user = currentUser();
  if (!user || !roles.includes(user.role)) {
    showToast("Bạn không có quyền truy cập trang này.");
    setTimeout(
      () => (location.href = `${window.BASE || "../"}index.html`),
      700,
    );
    return false;
  }
  return true;
}
function requireAuth() {
  if (!localStorage.getItem("bakery_token")) {
    location.href = `${window.BASE || "../"}pages/login.html`;
    return false;
  }
  return true;
}
window.API_BASE_URL = API_BASE_URL;
window.getGuestCart = getGuestCart;
window.saveGuestCart = saveGuestCart;
window.api = api;
window.saveAuth = saveAuth;
window.clearAuth = clearAuth;
window.currentUser = currentUser;
window.requireAuth = requireAuth;
window.requireRole = requireRole;
