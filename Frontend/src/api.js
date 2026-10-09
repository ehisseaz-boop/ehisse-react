// Mərkəzi API köməkçisi — bütün backend sorğuları buradan keçir.
// Vite proxy sayəsində nisbi URL istifadə olunur (/api/...), brauzer URL-i dəyişmir.
const BASE = import.meta.env.VITE_API_URL || '';

export async function api(path, { method = 'GET', body, params } = {}) {
  let url = BASE + path;
  if (params) {
    const qs = new URLSearchParams();
    Object.entries(params).forEach(([k, v]) => {
      if (v !== undefined && v !== null && v !== '') qs.append(k, v);
    });
    if (qs.toString()) url += '?' + qs.toString();
  }

  const res = await fetch(url, {
    method,
    credentials: 'include',
    headers: body && !(body instanceof FormData) ? { 'Content-Type': 'application/json' } : undefined,
    body: body instanceof FormData ? body : body ? JSON.stringify(body) : undefined,
  });

  const text = await res.text();
  let data = null;
  try { data = text ? JSON.parse(text) : null; } catch { data = null; }

  if (!res.ok) {
    let message = data?.message || data?.title;
    if (!message && res.status === 401) message = 'Bu əməliyyat üçün daxil olmalısınız.';
    if (!message && res.status === 403) message = 'Bu əməliyyat üçün icazəniz yoxdur.';
    const err = new Error(message || `Xəta: ${res.status}`);
    err.status = res.status;
    err.data = data;
    throw err;
  }
  return data;
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
  // AuthController
  me: () => api('/api/Auth/Me'),
  login: (body) => api('/api/Auth/Login', { method: 'POST', body }),
  operators: () => api('/api/Auth/Operators'),
  register: (body) => api('/api/Auth/Register', { method: 'POST', body }),
  verifyOtp: (body) => api('/api/Auth/VerifyOtp', { method: 'POST', body }),
  resendOtp: (body) => api('/api/Auth/ResendOtp', { method: 'POST', body }),
  logout: () => api('/api/Auth/Logout', { method: 'POST' }),
  forgotPassword: (body) => api('/api/Auth/ForgotPassword', { method: 'POST', body }),
  resetPassword: (body) => api('/api/Auth/ResetPassword', { method: 'POST', body }),

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
    user: (id) => api(`/api/Admin/User/${id}`),
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
