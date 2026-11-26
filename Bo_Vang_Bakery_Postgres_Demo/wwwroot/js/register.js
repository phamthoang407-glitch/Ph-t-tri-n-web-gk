document.getElementById("register-form").addEventListener("submit", async e => {
 e.preventDefault(); const b=e.target.querySelector("button[type=submit]"); b.disabled=true;
 try { const r=await api.post("/auth/register", {full_name:document.getElementById("full-name").value,email:document.getElementById("login-user").value,password:document.getElementById("login-pass").value}); saveAuth(r.data); location.href="../index.html"; }
 catch(e){showToast(e.message);b.disabled=false;}
});