import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, Tabs, Modal } from '../../components/Panel';

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

  // Form states - Kateqoriya
  const [catName, setCatName] = useState('');
  const [catOrder, setCatOrder] = useState(0);

  // Form states - Brend
  const [brandName, setBrandName] = useState('');
  const [assignCatId, setAssignCatId] = useState('');
  const [assignBrandId, setAssignBrandId] = useState('');

  // Form states - Model
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
    setTimeout(() => setNotification(''), 4000);
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
      notify('Kateqoriya yaradıldı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
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
      notify('Brend əlavə edildi.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
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

  async function handleAssignBrand(e) {
    e.preventDefault();
    if (!assignCatId || !assignBrandId) return;
    try {
      await apiFetch('/api/Admin/AssignBrand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ categoryId: Number(assignCatId), brandId: Number(assignBrandId) })
      });
      notify('Brend kateqoriyaya bağlandı.');
      loadAllCatalogData();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleRemoveBrand(categoryId, brandId) {
    if (!window.confirm('Bu brendi kateqoriyadan ayırmaq istədiyinizə əminsiniz?')) return;
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
      alert('Zəhmət olmasa model adını, kateqoriyanı və brendi seçin.');
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
      notify('Cihaz modeli əlavə edildi.');
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
    if (!window.confirm('Bu hissə növünü kateqoriyadan ayırmaq istədiyinizə əminsiniz?')) return;
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

  // Filtrasiya olunmuş modellər
  const filteredModels = models.filter(m => {
    if (selectedCatFilter && m.categoryId !== Number(selectedCatFilter)) return false;
    if (selectedBrandFilter && m.brandId !== Number(selectedBrandFilter)) return false;
    return true;
  });

  const tabsConfig = [
    { key: 'categories', label: '1. Kateqoriyalar', count: categories.length },
    { key: 'brands', label: '2. Brendlər', count: brands.length },
    { key: 'models', label: '3. Modellər', count: models.length },
    { key: 'partTypes', label: '4. Hissə Növləri & Matris', count: partTypes.length },
    { key: 'qualities', label: '5. Keyfiyyət Səviyyələri', count: qualities.length },
  ];

  if (loading) return <div className="text-center py-5 text-muted">Kataloq məlumatları yüklənir...</div>;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h3 className="section-title mb-1">Kataloq və Hiyerarxiya Matrisi</h3>
          <p className="text-muted m-0">Kateqoriya &gt; Brend &gt; Model və Kateqoriya &gt; Hissə Növü əlaqələndirilməsi</p>
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
              <h6 className="mb-3">Yeni Kateqoriya Əlavə Et</h6>
              <form onSubmit={handleCreateCategory} className="row g-2 align-items-end">
                <div className="col-12 col-md-6">
                  <label className="form-label text-muted small">Ad (məs: Telefon, Planşet, Noutbuk)</label>
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
                    <i className="bi bi-plus-lg me-1"></i> Əlavə et
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
                  label: 'Əməliyyat',
                  render: c => (
                    <button
                      onClick={() => handleToggleCategory(c.id)}
                      className={`btn btn-sm ${c.isActive ? 'btn-outline-danger' : 'btn-outline-success'}`}
                    >
                      {c.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                    </button>
                  )
                }
              ]}
              rows={categories}
            />
          </div>
        )}

        {/* ============================================
            TAB 2: BRENDLƏR VƏ MATRİS
           ============================================ */}
        {activeTab === 'brands' && (
          <div>
            <div className="row g-3 mb-4">
              <div className="col-12 col-lg-6">
                <div className="card p-3 h-100" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3">Yeni Brend Əlavə Et</h6>
                  <form onSubmit={handleCreateBrand} className="d-flex gap-2">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Brend adı (məs: Apple, Samsung, Dell)..."
                      value={brandName}
                      onChange={e => setBrandName(e.target.value)}
                      required
                    />
                    <button type="submit" className="btn text-nowrap" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }}>
                      <i className="bi bi-plus-lg me-1"></i> Əlavə et
                    </button>
                  </form>
                </div>
              </div>

              <div className="col-12 col-lg-6">
                <div className="card p-3 h-100" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3">Brendi Kateqoriyaya Bağla (Matris)</h6>
                  <form onSubmit={handleAssignBrand} className="row g-2">
                    <div className="col-6">
                      <select className="form-select" value={assignCatId} onChange={e => setAssignCatId(e.target.value)} required>
                        <option value="">Kateqoriya seçin...</option>
                        {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                      </select>
                    </div>
                    <div className="col-6">
                      <select className="form-select" value={assignBrandId} onChange={e => setAssignBrandId(e.target.value)} required>
                        <option value="">Brend seçin...</option>
                        {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
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
              <h6 className="mb-3">Mövcud Kateqoriya &gt; Brend Əlaqələri</h6>
              <div className="d-flex flex-wrap gap-2">
                {categoryBrands.map(cb => {
                  const cat = categories.find(c => c.id === cb.categoryId);
                  const br = brands.find(b => b.id === cb.brandId);
                  return (
                    <span key={`${cb.categoryId}-${cb.brandId}`} className="badge bg-secondary p-2 d-flex align-items-center gap-2">
                      <span><strong>{cat?.name || cb.categoryId}</strong> &gt; {br?.name || cb.brandId}</span>
                      <i
                        className="bi bi-x-circle text-danger"
                        style={{ cursor: 'pointer' }}
                        onClick={() => handleRemoveBrand(cb.categoryId, cb.brandId)}
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
                { key: 'name', label: 'Brend Adı' },
                {
                  key: 'isActive',
                  label: 'Status',
                  render: b => <span className={`badge ${b.isActive ? 'bg-success' : 'bg-secondary'}`}>{b.isActive ? 'Aktiv' : 'Deaktiv'}</span>
                },
                {
                  key: 'actions',
                  label: 'Əməliyyat',
                  render: b => (
                    <button
                      onClick={() => handleToggleBrand(b.id)}
                      className={`btn btn-sm ${b.isActive ? 'btn-outline-danger' : 'btn-outline-success'}`}
                    >
                      {b.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                    </button>
                  )
                }
              ]}
              rows={brands}
            />
          </div>
        )}

        {/* ============================================
            TAB 3: MODELLƏR
           ============================================ */}
        {activeTab === 'models' && (
          <div>
            <div className="card p-3 mb-4" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
              <h6 className="mb-3">Yeni Model Əlavə Et</h6>
              <form onSubmit={handleCreateModel} className="row g-2">
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Kateqoriya</label>
                  <select className="form-select" value={modelCatId} onChange={e => setModelCatId(e.target.value)} required>
                    <option value="">Seçin...</option>
                    {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Brend</label>
                  <select className="form-select" value={modelBrandId} onChange={e => setModelBrandId(e.target.value)} required>
                    <option value="">Seçin...</option>
                    {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                  </select>
                </div>
                <div className="col-12 col-md-3">
                  <label className="form-label text-muted small">Model Adı</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="məs: iPhone 15 Pro Max"
                    value={modelName}
                    onChange={e => setModelName(e.target.value)}
                    required
                  />
                </div>
                <div className="col-12 col-md-2">
                  <label className="form-label text-muted small">Model Kodu</label>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="A2849"
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
                    <i className="bi bi-plus-lg me-1"></i> Modeli Yadda Saxla
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
                { key: 'releaseYear', label: 'İl' },
                {
                  key: 'isActive',
                  label: 'Status',
                  render: m => <span className={`badge ${m.isActive ? 'bg-success' : 'bg-secondary'}`}>{m.isActive ? 'Aktiv' : 'Deaktiv'}</span>
                },
                {
                  key: 'actions',
                  label: 'Əməliyyat',
                  render: m => (
                    <button
                      onClick={() => handleToggleModel(m.id)}
                      className={`btn btn-sm ${m.isActive ? 'btn-outline-danger' : 'btn-outline-success'}`}
                    >
                      {m.isActive ? 'Deaktiv et' : 'Aktivləşdir'}
                    </button>
                  )
                }
              ]}
              rows={filteredModels}
            />
          </div>
        )}

        {/* ============================================
            TAB 4: HİSSƏ NÖVLƏRİ VƏ MATRİS
           ============================================ */}
        {activeTab === 'partTypes' && (
          <div>
            <div className="row g-3 mb-4">
              <div className="col-12 col-lg-6">
                <div className="card p-3 h-100" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3">Yeni Hissə Növü Əlavə Et</h6>
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
                  <h6 className="mb-3">Kateqoriyaya Hissə Növü Bağla (Matris)</h6>
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
              <h6 className="mb-3">Kateqoriya &gt; Hissə Növü Uyğunluq Matrisi</h6>
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
                { key: 'key', label: 'Sistem Açarı' }
              ]}
              rows={partTypes}
            />
          </div>
        )}

        {/* ============================================
            TAB 5: KEYFİYYƏT SƏVİYYƏLƏRİ
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
    </div>
  );
}
