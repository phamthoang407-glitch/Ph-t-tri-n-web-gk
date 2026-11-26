async function renderOrderTracking() {
  const params = new URLSearchParams(location.search);
  const id = params.get("id");
  try {
    let order;
    if (params.get("guest") === "1" || params.has("token")) {
      let lastOrder;
      try {
        lastOrder = JSON.parse(
          sessionStorage.getItem("bakery_last_order") || "null",
        );
      } catch {
        lastOrder = null;
      }
      if(params.get("token")) lastOrder={guest_tracking_token:params.get("token")};
      if (!lastOrder?.guest_tracking_token) {
        document.querySelector(".tracking-wrap").innerHTML =
          '<div class="empty-state">Không tìm thấy mã theo dõi đơn hàng này.</div>';
        return;
      }
      order = (
        await api.get(
          "/orders/guest/" + encodeURIComponent(lastOrder.guest_tracking_token),
        )
      ).data;
    } else {
      if (!requireAuth()) return;
      const orders = (await api.get("/orders")).data;
      const chooser=document.createElement("div");chooser.className="panel";chooser.style.padding="16px";
      chooser.innerHTML='<h3>Đơn hàng của tôi</h3>'+orders.map(o=>`<a style="display:block;padding:8px" href="?id=${o.id}">${escapeHtml(o.code)} — ${formatVND(o.total_amount)}</a>`).join("");
      document.querySelector(".tracking-wrap")?.prepend(chooser);
      order = id
        ? (await api.get("/orders/" + id)).data
        : orders[0]
          ? (await api.get("/orders/" + orders[0].id)).data
          : null;
    }
    if (!order) {
      document.querySelector(".tracking-wrap").innerHTML =
        '<div class="empty-state">Chưa có đơn hàng.</div>';
      return;
    }
    document.getElementById("order-code").textContent = order.code;
    if(params.get("token")){const note=document.createElement("p");note.textContent="Lưu đường dẫn trang này để theo dõi đơn. Chỉ chia sẻ với người nhận bánh.";document.querySelector(".tracking-head")?.append(note);}
    document.getElementById("order-total").textContent = formatVND(
      order.total_amount,
    );
    const statusLabel = {
      PENDING: "Chờ xác nhận",
      CONFIRMED: "Đã xác nhận",
      PROCESSING: "Đang chuẩn bị",
      SHIPPING: "Đang giao",
      COMPLETED: "Đã giao",
      CANCELLED: "Đã hủy",
    };
    const head = document.querySelector(".tracking-head .status");
    if (head) {
      head.textContent = statusLabel[order.status] || order.status;
      head.className =
        "status " +
        (order.status === "SHIPPING"
          ? "status--ship"
          : order.status === "COMPLETED"
            ? "status--done"
            : "status--wait");
    }
    const timeline = document.querySelector(".timeline");
    const states = ["PENDING", "PROCESSING", "COMPLETED"];
    if (timeline) {
      const current = order.status === "CONFIRMED" ? 1 : order.status === "SHIPPING" ? 1 : states.indexOf(order.status);
      const labels = [
        ["Chờ xác nhận", "Cửa hàng đã nhận đơn"],
        ["Đang xử lý", "Cửa hàng đang chuẩn bị bánh"],
        ["Hoàn thành", "Đơn hàng đã hoàn thành"],
      ];
      timeline.innerHTML = labels
        .map(
          (x, i) =>
            `<div class="timeline-step ${i < current ? "done" : i === current ? "current" : ""}"><div class="timeline-dot">${i < current ? "✓" : i + 1}</div><span>${x[0]}</span><small>${i === current ? x[1] : ""}</small></div>`,
        )
        .join("");
    }
    if (timeline && order.status === "CANCELLED")
      timeline.innerHTML = "<p>Đơn hàng đã hủy.</p>";
    const table = document.querySelector(".order-info .order-table tbody");
    if (table)
      table.innerHTML = order.items
        .map(
          (i) =>
            `<tr><td>${escapeHtml(i.product_name)}</td><td>${i.quantity}</td><td>${formatVND(i.price * i.quantity)}</td></tr>`,
        )
        .join("");
    const delivery = document.querySelector(".delivery-box");
    if (delivery)
      delivery.innerHTML = `<strong>Địa chỉ giao hàng</strong>${escapeHtml(order.customer_name)}<br>${escapeHtml(order.shipping_address)}<br><br><strong>Số điện thoại</strong>${escapeHtml(order.customer_phone)}`;
  } catch (e) {
    showToast(e.message);
  }
}
document.addEventListener("DOMContentLoaded", renderOrderTracking);
