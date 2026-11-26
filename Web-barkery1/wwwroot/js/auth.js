document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("login-form");
  async function login(identifier, password) {
    const button = form?.querySelector("button[type=submit]");
    if (button) {
      button.disabled = true;
      button.textContent = "Đang đăng nhập...";
    }
    try {
      const result = await api.post("/auth/login", { identifier, password });
      saveAuth(result.data);
      const role = result.data.user.role;
      const target =
        role === "ADMIN"
          ? "dashboard-owner.html"
          : "../index.html";
      location.href = BASE + "pages/" + target;
    } catch (e) {
      showToast(e.message);
    } finally {
      if (button) {
        button.disabled = false;
        button.textContent = "Đăng nhập →";
      }
    }
  }
  form?.addEventListener("submit", (e) => {
    e.preventDefault();
    login(
      document.getElementById("login-user").value.trim(),
      document.getElementById("login-pass").value,
    );
  });
});
