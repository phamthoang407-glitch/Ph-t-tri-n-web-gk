document.addEventListener("DOMContentLoaded", async () => {
  if (!requireAuth()) return;
  try {
    const user = (await api.get("/auth/me")).data;
    document.querySelector(".profile-head h2").textContent = user.full_name;
    const roleLabel = {
      CUSTOMER: "Khách hàng",
      STAFF: "Nhân viên",
      ADMIN: "Chủ quầy",
    }[user.role] || "Tài khoản";
    document.querySelector(".profile-head p").textContent =
      roleLabel;
    document.getElementById("account-avatar").textContent = user.full_name
      .split(/\s+/)
      .slice(-2)
      .map((part) => part[0] || "")
      .join("")
      .toUpperCase();
    const inputs = document.querySelectorAll(".form-grid input");
    if (inputs[0]) inputs[0].value = user.full_name || "";
    if (inputs[1]) inputs[1].value = user.phone || "";
    if (inputs[2]) inputs[2].value = user.email || "";
    if (inputs[3]) inputs[3].value = user.address || "";
    document.getElementById("save-profile")?.addEventListener("click", async () => {
      try { await api.put("/auth/me", {full_name:inputs[0].value, phone:inputs[1].value, address:inputs[3].value}); showToast("Đã lưu thông tin."); } catch(e) { showToast(e.message); }
    });
    const logout = document.querySelector('.account-menu a[style*="red"]');
    logout?.addEventListener("click", async (e) => {
      e.preventDefault();
      try {
        await api.post("/auth/logout", {});
      } catch(e) { showToast(e.message); return; }
      clearAuth();
      location.href = BASE + "index.html";
    });
  } catch (e) {
    showToast(e.message);
  }
});
