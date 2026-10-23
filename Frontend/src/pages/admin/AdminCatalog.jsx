import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, Tabs, Modal } from '../../components/Panel';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function AdminCatalog() {
  const [activeTab, setActiveTab] = useState('categories');
  const [loading, setLoading] = useState(true);
  const [notification, setNotification] = useState('');

  // Data states
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [categoryBrands, setCategoryBrands] = useState([]);
  const [models, setModels] = useState([]);
  const [partTypes, setPartTypes] = useState([]);
  const [categoryPartTypes, setCategoryPartTypes] = useState([]);
  const [qualities, setQualities] = useState([]);

  // Form states - Kateqoriya Əlavə et
  const [catName, setCatName] = useState('');
  const [catOrder, setCatOrder] = useState(0);

  // Form states - Brend Əlavə et
  const [brandName, setBrandName] = useState('');

  // Form states - Kateqoriyaya Brend Bağla (Matris)
  const [assignCatId, setAssignCatId] = useState('');
  const [assignBrandId, setAssignBrandId] = useState('');

  // Form states - Brendə Model Əlavə et (Kateqoriya + Brend seçimi)
  const [selectedCatFilter, setSelectedCatFilter] = useState('');
  const [selectedBrandFilter, setSelectedBrandFilter] = useState('');
  const [modelName, setModelName] = useState('');
  const [modelCode, setModelCode] = useState('');
  const [modelYear, setModelYear] = useState(new Date().getFullYear());
  const [modelCatId, setModelCatId] = useState('');
  const [modelBrandId, setModelBrandId] = useState('');

  // Form states - Hissə növü
  const [ptName, setPtName] = useState('');
  const [ptKey, setPtKey] = useState('');
  const [assignPtCatId, setAssignPtCatId] = useState('');
  const [assignPtId, setAssignPtId] = useState('');

  // Modal / Edit States
  const [editModal, setEditModal] = useState({ open: false, type: null, data: null });
  const [modalSaving, setModalSaving] = useState(false);

  useEffect(() => {
    loadAllCatalogData();
  }, []);

  async function loadAllCatalogData() {
    setLoading(true);
    try {
      const [cats, brs, cbrs, mods, pts, cpts, qts] = await Promise.all([
        apiFetch('/api/Admin/Categories'),
        apiFetch('/api/Admin/Brands'),
        apiFetch('/api/Admin/CategoryBrands'),
        apiFetch('/api/Admin/ModelsAndBrands'),
        apiFetch('/api/Admin/PartTypes'),
        apiFetch('/api/Admin/CategoryPartTypes'),
        apiFetch('/api/Admin/QualityTypes'),
      ]);

      setCategories(cats || []);
      setBrands(brs || []);
      setCategoryBrands(cbrs || []);
      setModels(mods || []);
      setPartTypes(pts || []);
      setCategoryPartTypes(cpts || []);
      setQualities(qts || []);
    } catch (err) {
      console.error(err);
      notify('Kataloq məlumatları yüklənərkən xəta: ' + err.message, 'danger');
    } finally {
      setLoading(false);
    }
  }

  function notify(text, type = 'success') {
    setNotification({ text, type });
    setTimeout(() => setNotification(''), 5000);
  }

  // ============================================
  // KATEQORİYA ƏMƏLİYYATLARI
  // ============================================
  async function handleCreateCategory(e) {
    e.preventDefault();
    if (!catName.trim()) return;
    try {
      await apiFetch('/api/Admin/CreateCategory', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: catName.trim(), displayOrder: Number(catOrder) })
      });
      setCatName('');
      setCatOrder(0);
      notify('Kateqoriya uğurla yaradıldı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleUpdateCategory(e) {
    e.preventDefault();
    const { id, name, displayOrder, isActive } = editModal.data;
    if (!name.trim()) return;
    setModalSaving(true);
    try {
      await apiFetch(`/api/Admin/UpdateCategory/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name.trim(), displayOrder: Number(displayOrder), isActive })
      });
      setEditModal({ open: false, type: null, data: null });
      notify('Kateqoriya uğurla yeniləndi.');
      await loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
      alertModal('Yeniləmə zamanı xəta baş verdi: ' + err.message, 'Xəta', 'error');
    } finally {
      setModalSaving(false);
    }
  }

  async function handleToggleCategory(id) {
    try {
      await apiFetch(`/api/Admin/ToggleCategory/${id}`, { method: 'POST' });
      notify('Kateqoriya statusu dəyişdirildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleDeleteCategory(id, name) {
    if (!await confirmModal({
      title: 'Kateqoriyanı sil',
      text: `"${name}" kateqoriyasını silmək istədiyinizə əminsiniz?`,
      confirmText: 'Bəli, silinsin',
      isDanger: true
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/DeleteCategory/${id}`, { method: 'POST' });
      notify(res.message || 'Kateqoriya silindi.');
      loadAllCatalogData();
    } catch (err) {
      notify(err.message || 'Silinmə zamanı xəta baş verdi.', 'danger');
    }
  }

  // ============================================
  // BREND ƏMƏLİYYATLARI
  // ============================================
  async function handleCreateBrand(e) {
    e.preventDefault();
    if (!brandName.trim()) return;
    try {
      await apiFetch('/api/Admin/CreateBrand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: brandName.trim() })
      });
      setBrandName('');
      notify('Brend uğurla əlavə edildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleUpdateBrand(e) {
    e.preventDefault();
    const { id, name, isActive } = editModal.data;
    if (!name.trim()) return;
    setModalSaving(true);
    try {
      await apiFetch(`/api/Admin/UpdateBrand/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name.trim(), isActive })
      });
      setEditModal({ open: false, type: null, data: null });
      notify('Brend uğurla yeniləndi.');
      await loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
      alertModal('Yeniləmə zamanı xəta baş verdi: ' + err.message, 'Xəta', 'error');
    } finally {
      setModalSaving(false);
    }
  }

  async function handleToggleBrand(id) {
    try {
      await apiFetch(`/api/Admin/ToggleBrand/${id}`, { method: 'POST' });
      notify('Brend statusu dəyişdirildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleDeleteBrand(id, name) {
    if (!await confirmModal({
      title: 'Brendi sil',
      text: `"${name}" brendini silmək istədiyinizə əminsiniz?`,
      confirmText: 'Bəli, silinsin',
      isDanger: true
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/DeleteBrand/${id}`, { method: 'POST' });
      notify(res.message || 'Brend silindi.');
      loadAllCatalogData();
    } catch (err) {
      notify(err.message || 'Silinmə zamanı xəta baş verdi.', 'danger');
    }
  }

  async function handleAssignBrand(e) {
    e.preventDefault();
    if (!assignCatId || !assignBrandId) return;
    try {
      await apiFetch('/api/Admin/AssignBrand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ categoryId: Number(assignCatId), brandId: Number(assignBrandId) })
      });
      notify('Brend kateqoriyaya uğurla bağlandı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleRemoveBrand(categoryId, brandId) {
    if (!await confirmModal({
      title: 'Brendi ayır',
      text: 'Bu brendi kateqoriyadan ayırmaq istədiyinizə əminsiniz?',
      confirmText: 'Ayır',
      isDanger: true
    })) return;
    try {
      await apiFetch('/api/Admin/RemoveBrand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ categoryId, brandId })
      });
      notify('Brend kateqoriyadan ayrıldı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  // ============================================
  // MODEL ƏMƏLİYYATLARI
  // ============================================
  async function handleCreateModel(e) {
    e.preventDefault();
    if (!modelName.trim() || !modelCatId || !modelBrandId) {
      alertModal('Zəhmət olmasa kateqoriya, brend və model adını qeyd edin.', 'Diqqət', 'warning');
      return;
    }
    try {
      await apiFetch('/api/Admin/CreateModel', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: modelName.trim(),
          modelCode: modelCode.trim(),
          releaseYear: Number(modelYear),
          categoryId: Number(modelCatId),
          brandId: Number(modelBrandId)
        })
      });
      setModelName('');
      setModelCode('');
      notify('Model brendə uğurla əlavə edildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleUpdateModel(e) {
    e.preventDefault();
    const { id, name, modelCode, releaseYear, categoryId, brandId, isActive } = editModal.data;
    if (!name.trim() || !categoryId || !brandId) return;
    try {
      await apiFetch(`/api/Admin/UpdateModel/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: name.trim(),
          modelCode: modelCode ? modelCode.trim() : '',
          releaseYear: Number(releaseYear),
          categoryId: Number(categoryId),
          brandId: Number(brandId),
          isActive
        })
      });
      setEditModal({ open: false, type: null, data: null });
      notify('Model uğurla yeniləndi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleToggleModel(id) {
    try {
      await apiFetch(`/api/Admin/ToggleModel/${id}`, { method: 'POST' });
      notify('Model statusu dəyişdirildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleDeleteModel(id, name) {
    if (!await confirmModal({
      title: 'Modeli sil',
      text: `"${name}" modelini silmək istədiyinizə əminsiniz?`,
      confirmText: 'Bəli, silinsin',
      isDanger: true
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/DeleteModel/${id}`, { method: 'POST' });
      notify(res.message || 'Model silindi.');
      loadAllCatalogData();
    } catch (err) {
      notify(err.message || 'Silinmə zamanı xəta baş verdi.', 'danger');
    }
  }

  // ============================================
  // HİSSƏ NÖVÜ VƏ MATRİS ƏMƏLİYYATLARI
  // ============================================
  async function handleCreatePartType(e) {
    e.preventDefault();
    if (!ptName.trim()) return;
    try {
      await apiFetch('/api/Admin/CreatePartType', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: ptName.trim(), key: ptKey.trim() })
      });
      setPtName('');
      setPtKey('');
      notify('Hissə növü əlavə edildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleUpdatePartType(e) {
    e.preventDefault();
    const { id, name, key, isAccessory, isPortable } = editModal.data;
    if (!name.trim()) return;
    try {
      await apiFetch(`/api/Admin/UpdatePartType/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name.trim(), key: key ? key.trim() : '', isAccessory, isPortable })
      });
      setEditModal({ open: false, type: null, data: null });
      notify('Hissə növü yeniləndi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleDeletePartType(id, name) {
    if (!await confirmModal({
      title: 'Hissə növünü sil',
      text: `"${name}" hissə növünü silmək istədiyinizə əminsiniz?`,
      confirmText: 'Bəli, silinsin',
      isDanger: true
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/DeletePartType/${id}`, { method: 'POST' });
      notify(res.message || 'Hissə növü silindi.');
      loadAllCatalogData();
    } catch (err) {
      notify(err.message || 'Silinmə zamanı xəta baş verdi.', 'danger');
    }
  }

  async function handleAssignPartType(e) {
    e.preventDefault();
    if (!assignPtCatId || !assignPtId) return;
    try {
      await apiFetch('/api/Admin/AssignPartType', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ categoryId: Number(assignPtCatId), partTypeId: Number(assignPtId) })
      });
      notify('Hissə növü kateqoriyaya əlaqələndirildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleRemovePartType(categoryId, partTypeId) {
    if (!await confirmModal({
      title: 'Hissə növünü ayır',
      text: 'Bu hissə növünü kateqoriyadan ayırmaq istədiyinizə əminsiniz?',
      confirmText: 'Ayır',
      isDanger: true
    })) return;
    try {
      await apiFetch('/api/Admin/RemovePartType', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ categoryId, partTypeId })
      });
      notify('Hissə növü kateqoriyadan ayrıldı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  // Brendlərə görə kateqoriyaları və ya əksini dinamik filtrasiya
  const availableBrandsForSelectedCat = modelCatId
    ? brands.filter(b => categoryBrands.some(cb => cb.categoryId === Number(modelCatId) && cb.brandId === b.id))
    : brands;

  // Siyahı filtri üçün
  const filteredModels = models.filter(m => {
    if (selectedCatFilter && m.categoryId !== Number(selectedCatFilter)) return false;
    if (selectedBrandFilter && m.brandId !== Number(selectedBrandFilter)) return false;
    return true;
  });

  const tabsConfig = [
    { key: 'categories', label: '1. Kateqoriyalar', count: categories.length },
    { key: 'brands', label: '2. Brendlər', count: brands.length },
    { key: 'categoryBrands', label: '3. Kateqoriyaya Brend Bağla', count: categoryBrands.length },
    { key: 'models', label: '4. Brendə Model Artırılması', count: models.length },
    { key: 'partTypes', label: '5. Hissə Növləri & Matris', count: partTypes.length },
    { key: 'qualities', label: '6. Keyfiyyət Səviyyələri', count: qualities.length },
  ];

  if (loading) return <div className="text-center py-5 text-muted">Kataloq məlumatları yüklənir...</div>;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h3 className="section-title mb-1">Kataloq və Kateqoriya-Brend-Model İdarəetməsi</h3>
          <p className="text-muted m-0">Kateqoriya, Brend, Model əlavə etmə, yeniləmə, əlaqələndirmə və məhsul asılılıqlı silinmə qaydaları</p>
        </div>
        <button onClick={loadAllCatalogData} className="btn btn-sm btn-outline">
          <i className="bi bi-arrow-clockwise me-1"></i> Yenilə
        </button>
      </div>

      {notification && (
        <div className={`alert alert-${notification.type} mb-4 d-flex align-items-center justify-content-between`}>
          <span>{notification.text}</span>
          <button type="button" className="btn-close" onClick={() => setNotification('')}></button>
        </div>
      )}

      <Tabs tabs={tabsConfig} active={activeTab} onChange={setActiveTab} />

      <div className="mt-4">
        {/* ============================================
            TAB 1: KATEQORİYALAR
           ============================================ */}
        {activeTab === 'categories' && (
          <div>
            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Yeni Kateqoriya Artırılması</h6>
              <form onSubmit={handleCreateCategory} className="row g-2 align-items-end">
                <div className="col-12 col-md-6">
                  <label className="form-label text-muted small">Kateqoriya Adı (məs: Telefon, Planşet, Noutbuk)</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Kateqoriya adı..."
                    value={catName}
                    onChange={e => setCatName(e.target.value)}
                    required
                  />
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Sıralama Sırası</label>
                  <input
                    type="number"
                    className="form-control"
                    value={catOrder}
                    onChange={e => setCatOrder(e.target.value)}
                  />
                </div>
                <div className="col-12 col-md-3">
                  <button type="submit" className="btn w-100" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                    <i className="bi bi-plus-lg me-1"></i> Kateqoriya Əlavə et
                  </button>
                </div>
              </form>
            </div>

            <Table
              columns={[
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Kateqoriya Adı' },
                { key: 'displayOrder', label: 'Sıra' },
                {
                  key: 'isActive',
                  label: 'Status',
                  render: c => <span className={`badge ${c.isActive ? 'bg-success' : 'bg-secondary'}`}>{c.isActive ? 'Aktiv' : 'Deaktiv'}</span>
                },
                {
                  key: 'actions',
                  label: 'Əməliyyatlar',
                  render: c => (
                    <div className="d-flex gap-2">
                      <button
                        onClick={() => setEditModal({ open: true, type: 'category', data: { ...c } })}
                        className="btn btn-sm btn-outline-primary"
                        title="Redaktə et"
                      >
                        <i className="bi bi-pencil me-1"></i> Redaktə
                      </button>
                      <button
                        onClick={() => handleToggleCategory(c.id)}
                        className={`btn btn-sm ${c.isActive ? 'btn-outline-warning' : 'btn-outline-success'}`}
                      >
                        {c.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                      </button>
                      <button
                        onClick={() => handleDeleteCategory(c.id, c.name)}
                        className="btn btn-sm btn-outline-danger"
                        title="Sil"
                      >
                        <i className="bi bi-trash me-1"></i> Sil
                      </button>
                    </div>
                  )
                }
              ]}
              rows={categories}
            />
          </div>
        )}

        {/* ============================================
            TAB 2: BRENDLƏR
           ============================================ */}
        {activeTab === 'brands' && (
          <div>
            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Yeni Brend Artırılması</h6>
              <form onSubmit={handleCreateBrand} className="d-flex gap-2">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Brend adı (məs: Apple, Samsung, Dell, Asus)..."
                  value={brandName}
                  onChange={e => setBrandName(e.target.value)}
                  required
                />
                <button type="submit" className="btn text-nowrap" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                  <i className="bi bi-plus-lg me-1"></i> Brend Əlavə et
                </button>
              </form>
            </div>

            <Table
              columns={[
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Brend Adı' },
                {
                  key: 'isActive',
                  label: 'Status',
                  render: b => <span className={`badge ${b.isActive ? 'bg-success' : 'bg-secondary'}`}>{b.isActive ? 'Aktiv' : 'Deaktiv'}</span>
                },
                {
                  key: 'actions',
                  label: 'Əməliyyatlar',
                  render: b => (
                    <div className="d-flex gap-2">
                      <button
                        onClick={() => setEditModal({ open: true, type: 'brand', data: { ...b } })}
                        className="btn btn-sm btn-outline-primary"
                        title="Redaktə et"
                      >
                        <i className="bi bi-pencil me-1"></i> Redaktə
                      </button>
                      <button
                        onClick={() => handleToggleBrand(b.id)}
                        className={`btn btn-sm ${b.isActive ? 'btn-outline-warning' : 'btn-outline-success'}`}
                      >
                        {b.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                      </button>
                      <button
                        onClick={() => handleDeleteBrand(b.id, b.name)}
                        className="btn btn-sm btn-outline-danger"
                        title="Sil"
                      >
                        <i className="bi bi-trash me-1"></i> Sil
                      </button>
                    </div>
                  )
                }
              ]}
              rows={brands}
            />
          </div>
        )}

        {/* ============================================
            TAB 3: KATEQORİYAYA BREND ARTIRILMASI (ƏLAQƏLƏNDİRMƏ)
           ============================================ */}
        {activeTab === 'categoryBrands' && (
          <div>
            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Kateqoriyaya Brend Artırılması (Əlaqələndirmə)</h6>
              <form onSubmit={handleAssignBrand} className="row g-2">
                <div className="col-12 col-md-5">
                  <label className="form-label text-muted small">Kateqoriya</label>
                  <select className="form-select" value={assignCatId} onChange={e => setAssignCatId(e.target.value)} required>
                    <option value="">Kateqoriya seçin...</option>
                    {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
                <div className="col-12 col-md-5">
                  <label className="form-label text-muted small">Brend</label>
                  <select className="form-select" value={assignBrandId} onChange={e => setAssignBrandId(e.target.value)} required>
                    <option value="">Brend seçin...</option>
                    {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                  </select>
                </div>
                <div className="col-12 col-md-2 d-flex align-items-end">
                  <button type="submit" className="btn w-100" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                    <i className="bi bi-link-45deg me-1"></i> Brend Artır
                  </button>
                </div>
              </form>
            </div>

            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Kateqoriya üzrə Mövcud Brend Əlaqələri</h6>
              {categories.map(cat => {
                const catBrs = categoryBrands.filter(cb => cb.categoryId === cat.id);
                return (
                  <div key={cat.id} className="mb-3 pb-3 border-bottom">
                    <div className="d-flex align-items-center gap-2 mb-2">
                      <strong className="text-primary">{cat.name}</strong>
                      <span className="badge bg-secondary">{catBrs.length} brend</span>
                    </div>
                    <div className="d-flex flex-wrap gap-2">
                      {catBrs.length === 0 ? (
                        <span className="text-muted small">Hələlik brend əlavə edilməyib.</span>
                      ) : (
                        catBrs.map(cb => {
                          const br = brands.find(b => b.id === cb.brandId);
                          return (
                            <span key={`${cb.categoryId}-${cb.brandId}`} className="badge bg-light text-dark border p-2 d-flex align-items-center gap-2">
                              <span>{br?.name || `Brend ID: ${cb.brandId}`}</span>
                              <button
                                type="button"
                                className="btn-close btn-close-white ms-1"
                                style={{ fontSize: '0.65rem' }}
                                onClick={() => handleRemoveBrand(cb.categoryId, cb.brandId)}
                                title="Kateqoriyadan ayır"
                              ></button>
                            </span>
                          );
                        })
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {/* ============================================
            TAB 4: BRENDƏ MODEL ARTIRILMASI
           ============================================ */}
        {activeTab === 'models' && (
          <div>
            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Brendə Model Artırılması</h6>
              <form onSubmit={handleCreateModel} className="row g-2">
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Kateqoriya</label>
                  <select
                    className="form-select"
                    value={modelCatId}
                    onChange={e => {
                      setModelCatId(e.target.value);
                      setModelBrandId('');
                    }}
                    required
                  >
                    <option value="">Seçin...</option>
                    {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Brend</label>
                  <select
                    className="form-select"
                    value={modelBrandId}
                    onChange={e => setModelBrandId(e.target.value)}
                    required
                  >
                    <option value="">Seçin...</option>
                    {availableBrandsForSelectedCat.map(b => (
                      <option key={b.id} value={b.id}>{b.name}</option>
                    ))}
                  </select>
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Model Adı</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="məs: iPhone 15 Pro, Galaxy S24"
                    value={modelName}
                    onChange={e => setModelName(e.target.value)}
                    required
                  />
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Model Kodu</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="A2849, SM-S928B"
                    value={modelCode}
                    onChange={e => setModelCode(e.target.value)}
                  />
                </div>
                <div className="col-12 col-md-1">
                  <label className="form-label text-muted small">İl</label>
                  <input
                    type="number"
                    className="form-control"
                    value={modelYear}
                    onChange={e => setModelYear(e.target.value)}
                  />
                </div>
                <div className="col-12 mt-3">
                  <button type="submit" className="btn" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                    <i className="bi bi-plus-lg me-1"></i> Modeli Brendə Artır
                  </button>
                </div>
              </form>
            </div>

            {/* Siyahı Filtri */}
            <div className="d-flex gap-2 mb-3 flex-wrap align-items-center">
              <span className="text-muted small">Filtrlə:</span>
              <select className="form-select form-select-sm" style={{ width: '180px' }} value={selectedCatFilter} onChange={e => setSelectedCatFilter(e.target.value)}>
                <option value="">Bütün Kateqoriyalar</option>
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
              <select className="form-select form-select-sm" style={{ width: '180px' }} value={selectedBrandFilter} onChange={e => setSelectedBrandFilter(e.target.value)}>
                <option value="">Bütün Brendlər</option>
                {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
              <span className="text-muted small ms-auto">Tapılan: {filteredModels.length} model</span>
            </div>

            <Table
              columns={[
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Model Adı' },
                { key: 'brandName', label: 'Brend' },
                { key: 'categoryName', label: 'Kateqoriya' },
                { key: 'modelCode', label: 'Kod', render: m => m.modelCode || '—' },
                                {
                  key: 'isActive',
                  label: 'Status',
                  render: m => <span className={`badge ${m.isActive ? 'bg-success' : 'bg-secondary'}`}>{m.isActive ? 'Aktiv' : 'Deaktiv'}</span>
                },
                {
                  key: 'actions',
                  label: 'Əməliyyatlar',
                  render: m => (
                    <div className="d-flex gap-2">
                      <button
                        onClick={() => setEditModal({ open: true, type: 'model', data: { ...m } })}
                        className="btn btn-sm btn-outline-primary"
                        title="Redaktə et"
                      >
                        <i className="bi bi-pencil me-1"></i> Redaktə
                      </button>
                      <button
                        onClick={() => handleToggleModel(m.id)}
                        className={`btn btn-sm ${m.isActive ? 'btn-outline-warning' : 'btn-outline-success'}`}
                      >
                        {m.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                      </button>
                      <button
                        onClick={() => handleDeleteModel(m.id, m.name)}
                        className="btn btn-sm btn-outline-danger"
                        title="Sil"
                      >
                        <i className="bi bi-trash me-1"></i> Sil
                      </button>
                    </div>
                  )
                }
              ]}
              rows={filteredModels}
            />
          </div>
        )}

        {/* ============================================
            TAB 5: HİSSƏ NÖVLƏRİ VƏ MATRİS
           ============================================ */}
        {activeTab === 'partTypes' && (
          <div>
            <div className="row g-3 mb-4">
              <div className="col-12 col-lg-6">
                <div className="card p-3 h-100" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3 font-semibold">Yeni Hissə Növü Əlavə Et</h6>
                  <form onSubmit={handleCreatePartType} className="row g-2">
                    <div className="col-6">
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Ad (məs: Ekran, Batareya)..."
                        value={ptName}
                        onChange={e => setPtName(e.target.value)}
                        required
                      />
                    </div>
                    <div className="col-6">
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Açar (məs: screen, battery)..."
                        value={ptKey}
                        onChange={e => setPtKey(e.target.value)}
                      />
                    </div>
                    <div className="col-12">
                      <button type="submit" className="btn w-100" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                        <i className="bi bi-plus-lg me-1"></i> Hissə Növü Əlavə et
                      </button>
                    </div>
                  </form>
                </div>
              </div>

              <div className="col-12 col-lg-6">
                <div className="card p-3 h-100" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3 font-semibold">Kateqoriyaya Hissə Növü Bağla (Matris)</h6>
                  <form onSubmit={handleAssignPartType} className="row g-2">
                    <div className="col-6">
                      <select className="form-select" value={assignPtCatId} onChange={e => setAssignPtCatId(e.target.value)} required>
                        <option value="">Kateqoriya seçin...</option>
                        {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                      </select>
                    </div>
                    <div className="col-6">
                      <select className="form-select" value={assignPtId} onChange={e => setAssignPtId(e.target.value)} required>
                        <option value="">Hissə növü seçin...</option>
                        {partTypes.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
                      </select>
                    </div>
                    <div className="col-12">
                      <button type="submit" className="btn btn-outline w-100" style={{ borderColor: 'var(--accent)', color: 'var(--accent)' }}>
                        <i className="bi bi-link-45deg me-1"></i> Kateqoriyaya Əlaqələndir
                      </button>
                    </div>
                  </form>
                </div>
              </div>
            </div>

            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3 font-semibold">Kateqoriya üzrə Hissə Növləri Matrisi</h6>
              <div className="d-flex flex-wrap gap-2">
                {categoryPartTypes.map(cpt => {
                  const cat = categories.find(c => c.id === cpt.categoryId);
                  const pt = partTypes.find(p => p.id === cpt.partTypeId);
                  return (
                    <span key={`${cpt.categoryId}-${cpt.partTypeId}`} className="badge bg-secondary p-2 d-flex align-items-center gap-2">
                      <span><strong>{cat?.name || cpt.categoryId}</strong> &gt; {pt?.name || cpt.partTypeId}</span>
                      <i
                        className="bi bi-x-circle text-danger"
                        style={{ cursor: 'pointer' }}
                        onClick={() => handleRemovePartType(cpt.categoryId, cpt.partTypeId)}
                        title="Əlaqəni sil"
                      ></i>
                    </span>
                  );
                })}
              </div>
            </div>

            <Table
              columns={[
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Hissə Növü Adı' },
                { key: 'key', label: 'Sistem Açarı' },
                {
                  key: 'actions',
                  label: 'Əməliyyatlar',
                  render: pt => (
                    <div className="d-flex gap-2">
                      <button
                        onClick={() => setEditModal({ open: true, type: 'partType', data: { ...pt } })}
                        className="btn btn-sm btn-outline-primary"
                        title="Redaktə et"
                      >
                        <i className="bi bi-pencil me-1"></i> Redaktə
                      </button>
                      <button
                        onClick={() => handleDeletePartType(pt.id, pt.name)}
                        className="btn btn-sm btn-outline-danger"
                        title="Sil"
                      >
                        <i className="bi bi-trash me-1"></i> Sil
                      </button>
                    </div>
                  )
                }
              ]}
              rows={partTypes}
            />
          </div>
        )}

        {/* ============================================
            TAB 6: KEYFİYYƏT SƏVİYYƏLƏRİ
           ============================================ */}
        {activeTab === 'qualities' && (
          <div>
            <div className="alert alert-info mb-4">
              <i className="bi bi-info-circle me-2"></i>
              Biznes qaydalarına əsasən, sistemdə ehtiyat hissələri üçün 5 sabit keyfiyyət səviyyəsi təyin edilmişdir:
              <strong> Yeni orijinal, İşlənmiş orijinal, A-Class, B-Class, C-Class</strong>.
            </div>

            <Table
              columns={[
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Keyfiyyət Dərəcəsi Adı' },
                { key: 'code', label: 'Kod' },
                { key: 'displayOrder', label: 'Sıra' },
                { key: 'description', label: 'Təsvir', render: q => q.description || '—' }
              ]}
              rows={qualities}
            />
          </div>
        )}
      </div>

      {/* ============================================
          REDAKTƏ (UPDATE) MODALI
         ============================================ */}
      {editModal.open && editModal.type === 'category' && (
        <Modal
          open={true}
          title="Kateqoriyanı Redaktə Et"
          onClose={() => setEditModal({ open: false, type: null, data: null })}
        >
          <form onSubmit={handleUpdateCategory}>
            <div className="mb-3">
              <label className="form-label">Kateqoriya Adı</label>
              <input
                type="text"
                className="form-control"
                value={editModal.data.name}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, name: e.target.value } })}
                required
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Sıralama Sırası</label>
              <input
                type="number"
                className="form-control"
                value={editModal.data.displayOrder}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, displayOrder: e.target.value } })}
              />
            </div>
            <div className="form-check mb-3">
              <input
                type="checkbox"
                className="form-check-input"
                id="catIsActive"
                checked={editModal.data.isActive}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, isActive: e.target.checked } })}
              />
              <label className="form-check-label" htmlFor="catIsActive">Aktivdir</label>
            </div>
            <div className="d-flex justify-content-end gap-2">
              <button type="button" className="btn btn-secondary" onClick={() => setEditModal({ open: false, type: null, data: null })}>
                Ləğv et
              </button>
              <button type="submit" className="btn btn-primary">
                Yadda Saxla
              </button>
            </div>
          </form>
        </Modal>
      )}

      {editModal.open && editModal.type === 'brand' && (
        <Modal
          open={true}
          title="Brendi Redaktə Et"
          onClose={() => setEditModal({ open: false, type: null, data: null })}
        >
          <form onSubmit={handleUpdateBrand}>
            <div className="mb-3">
              <label className="form-label">Brend Adı</label>
              <input
                type="text"
                className="form-control"
                value={editModal.data.name}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, name: e.target.value } })}
                required
              />
            </div>
            <div className="form-check mb-3">
              <input
                type="checkbox"
                className="form-check-input"
                id="brandIsActive"
                checked={editModal.data.isActive}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, isActive: e.target.checked } })}
              />
              <label className="form-check-label" htmlFor="brandIsActive">Aktivdir</label>
            </div>
            <div className="d-flex justify-content-end gap-2">
              <button type="button" className="btn btn-secondary" onClick={() => setEditModal({ open: false, type: null, data: null })}>
                Ləğv et
              </button>
              <button type="submit" className="btn btn-primary">
                Yadda Saxla
              </button>
            </div>
          </form>
        </Modal>
      )}

      {editModal.open && editModal.type === 'model' && (
        <Modal
          open={true}
          title="Modeli Redaktə Et"
          onClose={() => setEditModal({ open: false, type: null, data: null })}
        >
          <form onSubmit={handleUpdateModel}>
            <div className="mb-3">
              <label className="form-label">Kateqoriya</label>
              <select
                className="form-select"
                value={editModal.data.categoryId}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, categoryId: e.target.value } })}
                required
              >
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div className="mb-3">
              <label className="form-label">Brend</label>
              <select
                className="form-select"
                value={editModal.data.brandId}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, brandId: e.target.value } })}
                required
              >
                {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
            <div className="mb-3">
              <label className="form-label">Model Adı</label>
              <input
                type="text"
                className="form-control"
                value={editModal.data.name}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, name: e.target.value } })}
                required
              />
            </div>
            <div className="row g-2 mb-3">
              <div className="col-12">
                <label className="form-label">Model Kodu</label>
                <input
                  type="text"
                  className="form-control"
                  value={editModal.data.modelCode || ''}
                  onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, modelCode: e.target.value } })}
                />
              </div>
              <div className="col-4">
                <label className="form-label">İl</label>
                <input
                  type="number"
                  className="form-control"
                  value={editModal.data.releaseYear}
                  onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, releaseYear: e.target.value } })}
                />
              </div>
            </div>
            <div className="form-check mb-3">
              <input
                type="checkbox"
                className="form-check-input"
                id="modelIsActive"
                checked={editModal.data.isActive}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, isActive: e.target.checked } })}
              />
              <label className="form-check-label" htmlFor="modelIsActive">Aktivdir</label>
            </div>
            <div className="d-flex justify-content-end gap-2">
              <button type="button" className="btn btn-secondary" onClick={() => setEditModal({ open: false, type: null, data: null })}>
                Ləğv et
              </button>
              <button type="submit" className="btn btn-primary">
                Yadda Saxla
              </button>
            </div>
          </form>
        </Modal>
      )}

      {editModal.open && editModal.type === 'partType' && (
        <Modal
          open={true}
          title="Hissə Növünü Redaktə Et"
          onClose={() => setEditModal({ open: false, type: null, data: null })}
        >
          <form onSubmit={handleUpdatePartType}>
            <div className="mb-3">
              <label className="form-label">Hissə Növü Adı</label>
              <input
                type="text"
                className="form-control"
                value={editModal.data.name}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, name: e.target.value } })}
                required
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Sistem Açarı</label>
              <input
                type="text"
                className="form-control"
                value={editModal.data.key || ''}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, key: e.target.value } })}
              />
            </div>
            <div className="form-check mb-2">
              <input
                type="checkbox"
                className="form-check-input"
                id="ptIsAcc"
                checked={editModal.data.isAccessory || false}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, isAccessory: e.target.checked } })}
              />
              <label className="form-check-label" htmlFor="ptIsAcc">Aksesuardır</label>
            </div>
            <div className="form-check mb-3">
              <input
                type="checkbox"
                className="form-check-input"
                id="ptIsPort"
                checked={editModal.data.isPortable || false}
                onChange={e => setEditModal({ ...editModal, data: { ...editModal.data, isPortable: e.target.checked } })}
              />
              <label className="form-check-label" htmlFor="ptIsPort">Daşına biləndir</label>
            </div>
            <div className="d-flex justify-content-end gap-2">
              <button type="button" className="btn btn-secondary" onClick={() => setEditModal({ open: false, type: null, data: null })}>
                Ləğv et
              </button>
              <button type="submit" className="btn btn-primary">
                Yadda Saxla
              </button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
