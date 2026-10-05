import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';

const empty = {
  categoryId: '', brandId: '', deviceModelId: '', partTypeId: '', qualityTypeId: '',
  title: '', description: '', sku: '', price: '', discountPrice: '', currency: 'AZN',
  stockQuantity: 1, minOrderQuantity: 1, warrantyMonths: 0,
  latitude: '', longitude: '', locationAddress: ''
};

function PartForm() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();

  const [form, setForm] = useState(empty);
  const [lookups, setLookups] = useState(null);
  const [brands, setBrands] = useState([]);
  const [models, setModels] = useState([]);
  const [partTypes, setPartTypes] = useState([]);
  const [images, setImages] = useState([]);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');
  const [busy, setBusy] = useState(false);
  const [loadError, setLoadError] = useState('');

  // İlkin yükləmə
  useEffect(() => {
    (async () => {
      try {
        const fd = await Api.warehouse.formData();
        setLookups(fd);
        if (isEdit) {
          const p = await Api.warehouse.get(id);
          setForm({
            ...empty, ...p,
            categoryId: p.categoryId ?? '', brandId: p.brandId ?? '',
            discountPrice: p.discountPrice ?? '', description: p.description ?? '',
          });
          setImages(p.images || []);
        }
      } catch (e) { setLoadError(e.message); }
    })();
  }, [id, isEdit]);

  // Kateqoriya â†’ brendlər + hissə növləri
  useEffect(() => {
    if (!form.categoryId) { setBrands([]); setPartTypes([]); return; }
    Api.brandsByCategory(form.categoryId).then(setBrands).catch(() => setBrands([]));
    Api.partTypesByCategory(form.categoryId).then(setPartTypes).catch(() => setPartTypes([]));
  }, [form.categoryId]);

  // Brend â†’ modellər
  useEffect(() => {
    if (!form.categoryId || !form.brandId) { setModels([]); return; }
    Api.modelsByBrand(form.categoryId, form.brandId).then(setModels).catch(() => setModels([]));
  }, [form.categoryId, form.brandId]);

  const set = (k) => (e) => {
    const v = e.target.value;
    setForm(f => {
      const n = { ...f, [k]: v };
      if (k === 'categoryId') { n.brandId = ''; n.deviceModelId = ''; n.partTypeId = ''; }
      if (k === 'brandId') { n.deviceModelId = ''; }
      return n;
    });
  };

  const submit = async (e) => {
    e.preventDefault();
    setError(''); setMsg(''); setBusy(true);
    const body = {
      deviceModelId: Number(form.deviceModelId) || 0,
      partTypeId: Number(form.partTypeId) || 0,
      qualityTypeId: Number(form.qualityTypeId) || 0,
      title: form.title,
      description: form.description,
      sku: form.sku || null,
      price: Number(form.price) || 0,
      discountPrice: form.discountPrice === '' ? null : Number(form.discountPrice),
      currency: form.currency || 'AZN',
      stockQuantity: Number(form.stockQuantity) || 0,
      minOrderQuantity: Number(form.minOrderQuantity) || 1,
      warrantyMonths: Number(form.warrantyMonths) || 0,
      latitude: form.latitude === '' ? null : Number(form.latitude),
      longitude: form.longitude === '' ? null : Number(form.longitude),
      locationAddress: form.locationAddress || null,
    };
    try {
      if (isEdit) {
        const r = await Api.warehouse.update(id, body);
        setMsg(r.message);
      } else {
        const r = await Api.warehouse.add(body);
        // Yaradıldıqdan sonra şəkil əlavə etmək üçün redaktəyə keç
        navigate(`/warehouse/edit/${r.id}`, { replace: true });
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  };

  const upload = async (e) => {
    const file = e.target.files?.[0];
    e.target.value = '';
    if (!file) return;
    setError(''); setMsg('');
    try {
      const r = await Api.images.upload(id, file, images.length === 0);
      setImages(imgs => [...imgs.map(i => (r.image.isMain ? { ...i, isMain: false } : i)), r.image]);
      setMsg(r.message);
    } catch (err) { setError(err.message); }
  };

  const removeImage = async (imageId) => {
    if (!window.confirm('Şəkli silmək istəyirsiniz?')) return;
    try {
      await Api.images.remove(imageId);
      const p = await Api.warehouse.get(id);
      setImages(p.images || []);
    } catch (err) { setError(err.message); }
  };

  if (loadError) return <ErrorBox message={loadError} />;
  if (!lookups) return <Loading />;

  const field = { className: 'form-control mb-2' };

  return (
    <form onSubmit={submit} className="product-card" style={{ maxWidth: 760 }}>
      <h3 className="product-title">{isEdit ? 'Elanı redaktə et' : 'Yeni elan'}</h3>
      {isEdit && form.moderationStatus && <p className="text-muted">Əsas sahələrin dəyişdirilməsi elanı yenidən moderasiyaya göndərir.</p>}
      {error && <div className="alert alert-danger">{error}</div>}
      {msg && <div className="alert alert-success">{msg}</div>}

      <div className="row">
        <div className="col-md-4">
          <label>Kateqoriya</label>
          <select className="form-select mb-2" value={form.categoryId} onChange={set('categoryId')} required>
            <option value="">Seçin</option>
            {lookups.categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
        </div>
        <div className="col-md-4">
          <label>Brend</label>
          <select className="form-select mb-2" value={form.brandId} onChange={set('brandId')} required disabled={!brands.length}>
            <option value="">Seçin</option>
            {brands.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
          </select>
        </div>
        <div className="col-md-4">
          <label>Model</label>
          <select className="form-select mb-2" value={form.deviceModelId} onChange={set('deviceModelId')} required disabled={!models.length}>
            <option value="">Seçin</option>
            {models.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
          </select>
        </div>
        <div className="col-md-6">
          <label>Hissə növü</label>
          <select className="form-select mb-2" value={form.partTypeId} onChange={set('partTypeId')} required disabled={!partTypes.length}>
            <option value="">Seçin</option>
            {partTypes.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
          </select>
        </div>
        <div className="col-md-6">
          <label>Keyfiyyət</label>
          <select className="form-select mb-2" value={form.qualityTypeId} onChange={set('qualityTypeId')} required>
            <option value="">Seçin</option>
            {lookups.qualityTypes.map(q => <option key={q.id} value={q.id}>{q.name}</option>)}
          </select>
        </div>
      </div>

      <label>Ad</label>
      <input {...field} value={form.title} onChange={set('title')} minLength={3} maxLength={200} required />
      <label>Təsvir</label>
      <textarea {...field} rows={3} value={form.description} onChange={set('description')} />

      <div className="row">
        <div className="col-md-4"><label>SKU</label><input {...field} value={form.sku} onChange={set('sku')} placeholder="Boş qalsa avtomatik" /></div>
        <div className="col-md-4"><label>Qiymət</label><input {...field} type="number" step="0.01" min="0.01" value={form.price} onChange={set('price')} required /></div>
        <div className="col-md-4"><label>Endirimli qiymət</label><input {...field} type="number" step="0.01" min="0" value={form.discountPrice} onChange={set('discountPrice')} /></div>
        <div className="col-md-4"><label>Stok</label><input {...field} type="number" min="0" value={form.stockQuantity} onChange={set('stockQuantity')} required /></div>
        <div className="col-md-4"><label>Min. sifariş</label><input {...field} type="number" min="1" value={form.minOrderQuantity} onChange={set('minOrderQuantity')} /></div>
        <div className="col-md-4"><label>Zəmanət (ay)</label><input {...field} type="number" min="0" max="120" value={form.warrantyMonths} onChange={set('warrantyMonths')} /></div>
      </div>

        <div className="row mt-3">
          <div className="col-md-12 mb-2"><strong className="text-main">Məhsulun yerləşdiyi anbar (Xəritə üçün)</strong></div>
          <div className="col-md-4"><label>Ünvan</label><input {...field} value={form.locationAddress} onChange={set('locationAddress')} placeholder="Məs: Bakı, 28 May" /></div>
          <div className="col-md-4"><label>Enlem (Latitude)</label><input {...field} type="number" step="any" value={form.latitude} onChange={set('latitude')} placeholder="40.3953" /></div>
          <div className="col-md-4"><label>Boylam (Longitude)</label><input {...field} type="number" step="any" value={form.longitude} onChange={set('longitude')} placeholder="49.8822" /></div>
        </div>

      <div className="d-flex gap-2 mt-2">
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold" disabled={busy}>{busy ? '...' : isEdit ? 'Yadda saxla' : 'Yarat'}</button>
        <button type="button" className="btn-outline" onClick={() => navigate('/warehouse')}>Geri</button>
      </div>

      {isEdit && (
        <div className="mt-4">
          <h4>Şəkillər</h4>
          <div className="d-flex gap-2 flex-wrap mb-2">
            {images.map(img => (
              <div key={img.id} style={{ position: 'relative' }}>
                <img src={imageSrc(img.url)} alt="" style={{ width: 96, height: 96, objectFit: 'cover', borderRadius: 8, border: img.isMain ? '2px solid var(--accent)' : '1px solid var(--border-color)' }} />
                <button type="button" onClick={() => removeImage(img.id)} style={{ position: 'absolute', top: 2, right: 2, background: '#000a', color: '#fff', border: 'none', borderRadius: 4 }}><i className="bi bi-x"></i></button>
              </div>
            ))}
          </div>
          <input type="file" accept=".jpg,.jpeg,.png,.webp" onChange={upload} className="form-control" />
          <small className="text-muted">JPG, PNG, WEBP Â· maks. 5 MB</small>
        </div>
      )}
    </form>
  );
}

export default PartForm;


