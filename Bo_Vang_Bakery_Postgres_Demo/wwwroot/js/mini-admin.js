/* Bơ Vàng Bakery Mini — dashboard chỉ gồm 4 nhóm chính, sử dụng API SQL Server hiện có. */
(() => {
  'use strict';
  const $ = (id) => document.getElementById(id);
  const state = { products: [], categories: [], orders: [], productPage: 1, editing: null };
  const titles = { overview:'Tổng quan cửa hàng',products:'Quản lý sản phẩm',categories:'Quản lý danh mục',orders:'Quản lý đơn hàng' };
  const statuses = { PENDING:'Chờ xác nhận',CONFIRMED:'Chờ xử lý',PROCESSING:'Đang xử lý',SHIPPING:'Đang giao',COMPLETED:'Hoàn thành',CANCELLED:'Đã hủy' };
  const vnd = value => Number(value || 0).toLocaleString('vi-VN') + 'đ';
  function message(text, error=false) { const el=$('mini-message');el.textContent=text;el.classList.toggle('error',error);el.hidden=false;el.scrollIntoView({block:'nearest'}); }
  function show(view) {
    document.querySelectorAll('[data-panel]').forEach(x=>x.hidden=x.dataset.panel!==view);
    document.querySelectorAll('[data-view]').forEach(x=>x.classList.toggle('selected',x.dataset.view===view));
    $('mini-page-title').textContent=titles[view];
    $('mini-message').hidden=true;
    if(view==='overview') loadOverview();
    if(view==='products') loadProducts();
    if(view==='categories') loadCategories();
    if(view==='orders') loadOrders();
  }
  function table(headers, rows) {
    if (!rows.length) return '<div class="mini-empty">Chưa có dữ liệu phù hợp.</div>';
    return `<table class="mini-table"><thead><tr>${headers.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.join('')}</tbody></table>`;
  }
  function orderRow(o, actions=false) {
    let controls='';
    if(actions) {
      const next = o.status==='PENDING'||o.status==='CONFIRMED' ? 'PROCESSING' : o.status==='PROCESSING'||o.status==='SHIPPING' ? 'COMPLETED' : null;
      if(next) controls+=`<button class="action" data-order="${o.id}" data-status="${next}">${next==='PROCESSING'?'Xử lý':'Hoàn thành'}</button>`;
      if(['PENDING','CONFIRMED','PROCESSING','SHIPPING'].includes(o.status)) controls+=`<button class="action danger" data-order="${o.id}" data-status="CANCELLED">Hủy</button>`;
    }
    return `<tr><td>${escapeHtml(o.code||String(o.id))}</td><td>${escapeHtml(o.customer_name)}</td><td>${new Date(o.created_at).toLocaleDateString('vi-VN')}</td><td>${vnd(o.total_amount)}</td><td class="mini-status">${statuses[o.status]||escapeHtml(o.status)}</td>${actions?`<td>${controls||'—'}</td>`:''}</tr>`;
  }
  async function loadOverview() {
    try {
      const result=(await api.get('/dashboard')).data;
      $('metric-orders').textContent=result.orders_today;
      $('metric-completed').textContent=result.completed_today;
      $('metric-revenue').textContent=vnd(result.revenue_today);
      $('metric-products').textContent=result.products;
      $('mini-recent-orders').innerHTML=table(['Mã đơn','Khách','Ngày','Tổng tiền','Trạng thái'],(result.recent_orders||[]).map(o=>orderRow(o)));
    }catch(e){message(e.message,true)}
  }
  async function loadCategories() {
    try{
      state.categories=(await api.get('/categories')).data;
      $('mini-product-category').innerHTML='<option value="">Tất cả danh mục</option>'+state.categories.map(c=>`<option value="${c.id}">${escapeHtml(c.name)}</option>`).join('');
      $('mini-categories-table').innerHTML=table(['Tên danh mục','Mô tả','Thao tác'],state.categories.map(c=>`<tr><td>${escapeHtml(c.name)}</td><td>${escapeHtml(c.description||'')}</td><td><button class="action" data-edit-category="${c.id}">Sửa</button><button class="action danger" data-delete-category="${c.id}">Xóa</button></td></tr>`));
    }catch(e){message(e.message,true)}
  }
  async function loadProducts(){
    try{
      if(!state.categories.length) await loadCategories();
      state.products=(await api.get('/products')).data;
      renderProducts();
    }catch(e){message(e.message,true)}
  }
  function renderProducts(){
    const search=$('mini-product-search').value.trim().toLowerCase();
    const category=$('mini-product-category').value;
    const list=state.products.filter(p=>(!search||p.name.toLowerCase().includes(search))&&(!category||String(p.category_id)===category));
    const pages=Math.max(1,Math.ceil(list.length/8)); state.productPage=Math.min(Math.max(state.productPage,1),pages);
    const current=list.slice((state.productPage-1)*8,state.productPage*8);
    $('mini-products-table').innerHTML=table(['Sản phẩm','Danh mục','Giá','Tồn','Thao tác'],current.map(p=>`<tr><td><img src="${safeImage(p.image)}" alt="">${escapeHtml(p.name)}</td><td>${escapeHtml(p.category||'')}</td><td>${vnd(p.price)}</td><td>${p.stock}</td><td><button class="action" data-edit-product="${p.id}">Sửa</button><button class="action danger" data-delete-product="${p.id}">Xóa</button></td></tr>`));
    $('mini-product-pages').innerHTML=pages>1?`<button ${state.productPage===1?'disabled':''} data-p="${state.productPage-1}">Trước</button><span>${state.productPage}/${pages}</span><button ${state.productPage===pages?'disabled':''} data-p="${state.productPage+1}">Sau</button>`:'';
  }
  async function loadOrders(){
    try{state.orders=(await api.get('/orders')).data;renderOrders()}catch(e){message(e.message,true)}
  }
  function renderOrders(){
    const selected=$('mini-order-filter').value;
    const list=state.orders.filter(o=>!selected||o.status===selected||(selected==='PROCESSING'&&['CONFIRMED','SHIPPING'].includes(o.status)));
    $('mini-orders-table').innerHTML=table(['Mã đơn','Khách','Ngày','Tổng tiền','Trạng thái','Thao tác'],list.map(o=>orderRow(o,true)));
  }
  function field(name,label,value='',type='text',required=true){
    return `<label>${label}<input name="${name}" type="${type}" value="${escapeHtml(value)}" ${required?'required':''}></label>`;
  }
  function openEditor(kind,id=null){
    state.editing={kind,id};$('mini-editor-form').reset();
    const product=kind==='product'&&id!==null?state.products.find(p=>p.id===id):null;
    const category=kind==='category'&&id!==null?state.categories.find(c=>c.id===id):null;
    $('mini-dialog-title').textContent=(id===null?'Thêm ':'Sửa ')+(kind==='product'?'sản phẩm':'danh mục');
    if(kind==='product') {
      $('mini-editor-fields').innerHTML=field('name','Tên bánh',product?.name||'')+field('price','Giá bán (VNĐ)',product?.price??'', 'number')+field('stock','Số lượng còn',product?.stock??0,'number')+
        `<label>Danh mục<select name="category_id" required>${state.categories.map(c=>`<option value="${c.id}" ${c.id===product?.category_id?'selected':''}>${escapeHtml(c.name)}</option>`).join('')}</select></label>`+
        field('image','Ảnh (URL https hoặc /images/)',product?.image?.includes('cake-placeholder')?'':product?.image||'','text',false)+
        `<label>Mô tả<textarea name="description" rows="3">${escapeHtml(product?.description||'')}</textarea></label>`;
    }else{
      $('mini-editor-fields').innerHTML=field('name','Tên danh mục',category?.name||'')+`<label>Mô tả<textarea name="description" rows="3">${escapeHtml(category?.description||'')}</textarea></label>`;
    }
    $('mini-editor').showModal();
  }
  async function saveEditor(e){
    e.preventDefault();const {kind,id}=state.editing;const form=new FormData(e.target);const btn=$('mini-save');btn.disabled=true;
    try{
      const data=Object.fromEntries(form);
      if(kind==='product') {
        data.price=Number(data.price);data.stock=Number(data.stock);data.category_id=Number(data.category_id);
        if(!Number.isFinite(data.price)||data.price<=0||!Number.isSafeInteger(data.stock)||data.stock<0) throw new Error('Giá và số lượng không hợp lệ.');
      }
      const resource=kind==='product'?'products':'categories';
      if(id===null) await api.post('/'+resource,data);else await api.put('/'+resource+'/'+id,data);
      $('mini-editor').close();message('Đã lưu thành công.');
      if(kind==='product') await loadProducts();else {await loadCategories();await loadProducts();}
    }catch(err){message(err.message,true)}finally{btn.disabled=false}
  }
  async function action(e){
    const button=e.target.closest('button');if(!button)return;
    if(button.dataset.go) return show(button.dataset.go);
    if(button.dataset.editProduct) return openEditor('product',Number(button.dataset.editProduct));
    if(button.dataset.editCategory) return openEditor('category',Number(button.dataset.editCategory));
    if(button.dataset.p){state.productPage=Number(button.dataset.p);renderProducts();return;}
    if(button.dataset.deleteProduct){
      const id=Number(button.dataset.deleteProduct);
      if(!confirm('Xóa sản phẩm? Nếu đã có đơn hàng, hệ thống sẽ từ chối và bạn có thể đặt tồn = 0.')) return;
      try{await api.delete('/products/'+id);message('Đã xóa sản phẩm.');await loadProducts()}catch(err){message(err.message,true)}return;
    }
    if(button.dataset.deleteCategory){
      if(!confirm('Xóa danh mục này?'))return;
      try{await api.delete('/categories/'+Number(button.dataset.deleteCategory));message('Đã xóa danh mục.');await loadCategories()}catch(err){message(err.message,true)}return;
    }
    if(button.dataset.order&&button.dataset.status){
      const status=button.dataset.status;const id=Number(button.dataset.order);let payload={status};
      if(status==='CANCELLED'){
        const reason=prompt('Nhập lý do hủy đơn (ít nhất 3 ký tự):');
        if(reason===null)return;payload.cancellation_reason=reason.trim();
        if(payload.cancellation_reason.length<3){message('Lý do hủy phải có ít nhất 3 ký tự.',true);return;}
      }else if(!confirm('Xác nhận cập nhật trạng thái đơn?'))return;
      button.disabled=true;
      try{await api.put('/orders/'+id+'/status',payload);message('Cập nhật đơn hàng thành công.');await loadOrders()}catch(err){message(err.message,true)}finally{button.disabled=false}
    }
  }
  document.addEventListener('DOMContentLoaded',()=>{
    document.querySelector('.mini-shell').addEventListener('click',action);
    document.querySelectorAll('[data-view]').forEach(b=>b.addEventListener('click',()=>show(b.dataset.view)));
    $('mini-new-product').addEventListener('click',async()=>{if(!state.categories.length)await loadCategories();if(!state.categories.length){message('Hãy tạo danh mục trước khi thêm bánh.',true);return;}openEditor('product')});
    $('mini-new-category').addEventListener('click',()=>openEditor('category'));
    $('mini-product-search').addEventListener('input',()=>{state.productPage=1;renderProducts()});
    $('mini-product-category').addEventListener('change',()=>{state.productPage=1;renderProducts()});
    $('mini-order-filter').addEventListener('change',renderOrders);
    $('mini-editor-form').addEventListener('submit',saveEditor);
    $('mini-close').addEventListener('click',()=>$('mini-editor').close());
    $('mini-cancel').addEventListener('click',()=>$('mini-editor').close());
    $('mini-logout').addEventListener('click',async()=>{try{await api.post('/auth/logout',{});clearAuth();location.href='/pages/login.html'}catch(err){message(err.message,true)}});
    show('overview');
  });
})();
