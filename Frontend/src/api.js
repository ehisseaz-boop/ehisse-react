// Mərkəzi API köməkçisi — bütün backend sorğuları Vercel proxy (/api/...) üzərindən keçir.
// BASE boş string olduqda bütün cookie-lər eyni domendə (First-party) işləyir və mobil brauzerlər tərəfindən ƏSLA bloklanmır!
const BASE = '';



function sanitizeString(str) {
  if (typeof str !== 'string') return str;
  return str
    .replace(/\u00c6\u008f/g, 'Ə')
    .replace(/\u00c6\s*\u008f/g, 'Ə')
    .replace(/\u00e6\u008f/g, 'ə')
    .replace(/\u00c6[\s\u0080-\u009f]*liyev/gi, 'Əliyev')
    .replace(/\u00c6[\s\u0080-\u009f]*hmədli/gi, 'Əhmədli')
    .replace(/\u00c6[\s\u0080-\u009f]*sas/gi, 'Əsas')
    .replace(/Æ\x8f/g, 'Ə')
    .replace(/Æ\s*liyev/gi, 'Əliyev')
    .replace(/Æ\s*hmədli/gi, 'Əhmədli')
    .replace(/Æ\s*sas/gi, 'Əsas')
    .replace(/Æ/g, 'Ə')
    .replace(/Â·/g, '·')
    .replace(/Â/g, '')
    .replace(/\bHiss\?si\b/gi, 'Hissəsi')
    .replace(/\b\?n Kamera\b/gi, 'Ön Kamera')
    .replace(/\b\?arj\b/gi, 'Şarj')
    .replace(/\bSetkas\?\b/gi, 'Setkası')
    .replace(/\bT\?mir\b/gi, 'Təmir')
    .replace(/\bM\?rk\?zi\b/gi, 'Mərkəzi')
    .replace(/\b\?\?l\?nmi\?\b/gi, 'İşlənmiş')
    .replace(/\b\?stifad\b/gi, 'İstifad')
    .replace(/k\?\?\./g, 'küç.')
    .replace(/k\?\?h\./g, 'küç.')
    .replace(/rayonu,\s*C\?f\?r/g, 'rayonu, Cəfər')
    .replace(/Cabbarl\?\s*küç\./g, 'Cabbarlı küç.')
    .replace(/N\?simi/g, 'Nəsimi')
    .replace(/N\?rimanov/g, 'Nərimanov')
    .replace(/M\?mm\?dov/g, 'Məmmədov')
    .replace(/H\?s\?nova/g, 'Həsənova')
    .replace(/El\?in/g, 'Elçin')
    .replace(/R\?\?ad/g, 'Rəşad')
    .replace(/Bak\?,/g, 'Bakı,')
    .replace(/\s+/g, ' ');
}

function deepSanitize(val) {
  if (!val) return val;
  if (typeof val === 'string') return sanitizeString(val);
  if (Array.isArray(val)) return val.map(deepSanitize);
  if (typeof val === 'object') {
    const res = {};
    for (const [k, v] of Object.entries(val)) {
      res[k] = deepSanitize(v);
    }
    return res;
  }
  return val;
}

export async function api(path, { method = 'GET', body, params } = {}) {
  let url = BASE + path;
  if (params) {
    const qs = new URLSearchParams();
    Object.entries(params).forEach(([k, v]) => {
      if (v !== undefined && v !== null && v !== '') qs.append(k, v);
    });
    if (qs.toString()) url += '?' + qs.toString();
  }

  const headers = {};
  if (body && !(body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  // Mobil brauzerlərin cookie bloklamasını aşmaq üçün istifadəçi ID-sini birbaşa Header ilə göndəririk
  try {
    const saved = localStorage.getItem('ehisse_user');
    if (saved) {
      const u = JSON.parse(saved);
      if (u?.id) headers['X-User-Id'] = String(u.id);
      if (u?.username) headers['X-Username'] = encodeURIComponent(u.username);
    }
  } catch { }

  const res = await fetch(url, {
    method,
    credentials: 'include',
    headers,
    body: body instanceof FormData ? body : (typeof body === "string" ? body : (body ? JSON.stringify(body) : undefined)),
  });

  const text = await res.text();
  let data = null;
  try { data = text ? JSON.parse(text) : null; } catch { data = null; }

  if (!res.ok) {
    let message = '';
    if (data?.errors && typeof data.errors === 'object') {
      const errorList = Object.entries(data.errors).map(([k, v]) => k + ': ' + (Array.isArray(v) ? v.join(', ') : v));
      if (errorList.length > 0) message = errorList.join(' | ');
    }
    if (!message) message = data?.message;
    if (!message && data?.title && data.title !== 'One or more validation errors occurred.') message = data.title;
    if (!message && res.status === 401) message = 'Bu əməliyyat üçün daxil olmalısınız.';
    if (!message && res.status === 403) message = 'Bu əməliyyat üçün icazəniz yoxdur.';
    const err = new Error(sanitizeString(message || `Xəta: ${res.status}`));
    err.status = res.status;
    err.data = data;
    throw err;
  }
  return deepSanitize(data);
}

export function imageSrc(url) {
  if (!url) return null;
  if (/^https?:\/\//i.test(url)) return url;
  return `${BASE}/Image/GetImage?path=${encodeURIComponent(url)}`;
}

export const Api = {
  // PublicApiController
  categories: () => api('/api/publicapi/categories'),
  products: (params) => api('/api/publicapi/products', { params }),
  // HomeController
  catalog: (params) => api('/api/Home/Index', { params }),
  allCategories: () => api('/api/Home/Categories'),
  sellers: (params) => {
    if (typeof params === 'string') {
      return api('/api/Home/Saticilar', { params: { searchString: params } });
    }
    return api('/api/Home/Saticilar', { params });
  },
  help: () => api('/api/Home/Yardim'),
  contact: () => api('/api/Home/Elaqe'),
  details: (id) => api(`/api/Home/Details/${id}`),
  getSellerReviews: (sellerId) => api('/api/Home/GetSellerReviews', { params: { sellerId } }),
  addSellerReview: (body) => api('/api/Home/AddSellerReview', { method: 'POST', body }),
  // AuthController
  me: () => api('/api/Auth/Me'),
  login: (body) => api('/api/Auth/Login', { method: 'POST', body }),
  operators: () => api('/api/Auth/Operators'),
  register: (body) => api('/api/Auth/Register', { method: 'POST', body }),
  sendRegisterOtp: (body) => api('/api/Auth/SendRegisterOtp', { method: 'POST', body }),
  verifyOtp: (body) => api('/api/Auth/VerifyOtp', { method: 'POST', body }),
  resendOtp: (body) => api('/api/Auth/ResendOtp', { method: 'POST', body }),
  logout: () => api('/api/Auth/Logout', { method: 'POST' }),
  forgotPassword: (body) => api('/api/Auth/ForgotPassword', { method: 'POST', body }),
  verifyResetOtp: (body) => api('/api/Auth/VerifyResetOtp', { method: 'POST', body }),
  resetPassword: (body) => api('/api/Auth/ResetPassword', { method: 'POST', body }),
  changePassword: (body) => api('/api/Auth/ChangePassword', { method: 'POST', body }),
  sendEmailOtp: (body) => api('/api/Auth/SendEmailOtp', { method: 'POST', body }),
  verifyEmailOtp: (body) => api('/api/Auth/VerifyEmailOtp', { method: 'POST', body }),

  // AjaxController (asılı seçimlər + axtarış)
  lookupCategories: () => api('/api/Ajax/GetCategories'),
  qualityTypes: () => api('/api/Ajax/GetQualityTypes'),
  brandsByCategory: (categoryId) => api('/api/Ajax/GetBrandsByCategory', { params: { categoryId } }),
  modelsByBrand: (categoryId, brandId) => api('/api/Ajax/GetModelsByBrand', { params: { categoryId, brandId } }),
  partTypesByCategory: (categoryId) => api('/api/Ajax/GetPartTypesByCategory', { params: { categoryId } }),
  search: (q) => api('/api/Ajax/Search', { params: { q } }),

  // WarehouseController ("Elanlarım" kabineti)
  warehouse: {
    list: () => api('/api/Warehouse/Index'),
    part: (id) => api(`/api/Warehouse/Part/${id}`),
    formData: () => api('/api/Warehouse/FormData'),
    add: (body) => api('/api/Warehouse/AddPart', { method: 'POST', body }),
    get: (id) => api(`/api/Warehouse/EditPart/${id}`),
    update: (id, body) => api(`/api/Warehouse/EditPart/${id}`, { method: 'PUT', body }),
    updatePrice: (id, newPrice) => api(`/api/Warehouse/UpdatePrice/${id}`, { method: 'POST', body: { newPrice } }),
    deactivate: (id) => api(`/api/Warehouse/ToggleActive/${id}`, { method: 'POST' }),
    toggleActive: (id) => api(`/api/Warehouse/ToggleActive/${id}`, { method: 'POST' }),
    remove: (id) => api(`/api/Warehouse/DeletePart/${id}`, { method: 'DELETE' }),
    deletedAds: () => api('/api/Warehouse/DeletedAds'),
    bulkImport: (items) => api('/api/Warehouse/BulkImportParts', { method: 'POST', body: items }),
    restorePart: (id) => api(`/api/Warehouse/RestorePart/${id}`, { method: 'POST' }),
    recordSale: (id, expectedStock) => api(`/api/Warehouse/RecordSale/${id}`, { method: 'POST', body: { expectedStock } }),
    salesHistory: () => api('/api/Warehouse/SalesHistory'),
    settings: () => api('/api/Warehouse/Settings'),
    saveSettings: (body) => api('/api/Warehouse/Settings', { method: 'PUT', body }),
  },

  // ImageController
  images: {
    upload: (sparePartId, file, isMain = false) => {
      const fd = new FormData();
      fd.append('sparePartId', sparePartId);
      fd.append('file', file);
      fd.append('isMain', isMain);
      return api('/api/Image/Upload', { method: 'POST', body: fd });
    },
    remove: (imageId) => api(`/api/Image/Delete/${imageId}`, { method: 'DELETE' }),
  },

  // AdminController
  admin: {
    dashboard: () => api('/api/Admin/Dashboard'),
    requests: () => api('/api/Admin/Requests'),
    user: (id) => api(`/api/Admin/UserDetails/${id}`),
    approveUser: (id) => api(`/api/Admin/ApproveUser/${id}`, { method: 'POST' }),
    rejectUser: (id, reason) => api(`/api/Admin/RejectUser/${id}`, { method: 'POST', body: { reason } }),
    blockUser: (id, reason) => api(`/api/Admin/BlockUser/${id}`, { method: 'POST', body: { reason } }),
    unblockUser: (id) => api(`/api/Admin/UnblockUser/${id}`, { method: 'POST' }),
    ad: (id) => api(`/api/Admin/Ad/${id}`),
    approveAd: (id) => api(`/api/Admin/ApproveAd/${id}`, { method: 'POST' }),
    rejectAd: (id, reason) => api(`/api/Admin/RejectAd/${id}`, { method: 'POST', body: { reason } }),
    restoreAd: (id) => api(`/api/Admin/RestoreAd/${id}`, { method: 'POST' }),
    deactivateAd: (id, reason) => api(`/api/Admin/DeactivateAd/${id}`, { method: 'POST', body: { reason } }),
    allUsers: () => api('/api/Admin/AllUsers'),
    allAds: () => api('/api/Admin/AllAds'),
    activityLogs: () => api('/api/Admin/ActivityLogs'),
    catalog: {
      all: () => api('/api/Admin/Catalog'),
      save: (kind, item) => item.id
        ? api(`/api/Admin/Catalog/${kind}/${item.id}`, { method: 'PUT', body: item })
        : api(`/api/Admin/Catalog/${kind}`, { method: 'POST', body: item }),
      remove: (kind, id) => api(`/api/Admin/Catalog/${kind}/${id}`, { method: 'DELETE' }),
      saveLinks: (categoryId, body) => api(`/api/Admin/Catalog/links/${categoryId}`, { method: 'PUT', body }),
    },
  },
};
