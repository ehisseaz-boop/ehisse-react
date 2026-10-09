import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { statusBadge } from '../../components/Panel';

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
            locationAddress: p.locationAddress ?? '',
            latitude: p.latitude ?? '',
            longitude: p.longitude ?? '',
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

    const selectedBrand = brands.find(b => String(b.id) === String(form.brandId))?.name || '';
    const selectedModel = models.find(m => String(m.id) === String(form.deviceModelId))?.name || '';
    const selectedPartType = partTypes.find(p => String(p.id) === String(form.partTypeId))?.name || '';
    const autoTitle = [selectedBrand, selectedModel, selectedPartType].filter(Boolean).join(' ') || form.title || '';

    const body = {
      deviceModelId: Number(form.deviceModelId) || 0,
      partTypeId: Number(form.partTypeId) || 0,
      qualityTypeId: Number(form.qualityTypeId) || 0,
      title: autoTitle,
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
        if (window.Swal) {
          await window.Swal.fire({
            icon: 'success',
            title: 'Uğurlu!',
            text: r.message || 'Dəyişikliklər yadda saxlanıldı.',
            confirmButtonText: 'Kabinetə keç',
            confirmButtonColor: '#00d26a',
            timer: 2000,
            timerProgressBar: true
          });
        }
        navigate('/warehouse');
      } else {
        const r = await Api.warehouse.add(body);
        if (window.Swal) {
          await window.Swal.fire({
            icon: 'success',
            title: 'Elan yerləşdirildi!',
            text: r.message || 'Elan uğurla əlavə edildi.',
            confirmButtonText: 'Kabinetə keç',
            confirmButtonColor: '#00d26a',
            timer: 2000,
            timerProgressBar: true
          });
        }
        navigate('/warehouse');
      }
    } catch (err) {
      setError(err.message);
      if (window.Swal) {
        window.Swal.fire({
          icon: 'error',
          title: 'Uğursuz əməliyyat!',
          text: err.message || 'Məlumatları yadda saxlayarkən xəta baş verdi.',
          confirmButtonText: 'Bağla',
          confirmButtonColor: '#d33'
        });
      }
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
      <div className="d-flex justify-content-between align-items-center mb-2">
        <h3 className="product-title m-0">{isEdit ? 'Elanı redaktə et' : 'Yeni elan'}</h3>
        {isEdit && form.moderationStatus && (
          <div className="d-flex align-items-center gap-2">
            <span className="text-muted small">Status:</span>
            {statusBadge(form.moderationStatus)}
          </div>
        )}
      </div>
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

      <label>Təsvir</label>
      <textarea {...field} rows={3} value={form.description} onChange={set('description')} />

      <div className="row">
        <div className="col-md-3"><label>Qiymət (AZN)</label><input {...field} type="number" step="0.01" min="0.01" value={form.price} onChange={set('price')} required /></div>
        <div className="col-md-3"><label>Endirimli qiymət</label><input {...field} type="number" step="0.01" min="0" value={form.discountPrice} onChange={set('discountPrice')} /></div>
        <div className="col-md-3"><label>Stok sayı</label><input {...field} type="number" min="0" value={form.stockQuantity} onChange={set('stockQuantity')} required /></div>
        <div className="col-md-3"><label>Zəmanət (ay)</label><input {...field} type="number" min="0" max="120" value={form.warrantyMonths} onChange={set('warrantyMonths')} /></div>
      </div>

      <div className="row mt-3">
        <div className="col-md-12 mb-2 d-flex justify-content-between align-items-center">
          <strong className="text-main"><i className="bi bi-geo-alt text-accent me-1"></i> Məhsulun yerləşdiyi anbar / ünvan</strong>
          <button
            type="button"
            className="btn btn-sm btn-outline"
            style={{ borderRadius: '20px', fontSize: '0.8rem' }}
            onClick={() => {
              if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                  (pos) => {
                    setForm(f => ({
                      ...f,
                      latitude: pos.coords.latitude.toFixed(6),
                      longitude: pos.coords.longitude.toFixed(6),
                      locationAddress: f.locationAddress || 'Mənim cari yerim'
                    }));
                  },
                  () => alert('Cari yerinizi təyin etmək mümkün olmadı.')
                );
              } else {
                alert('Brauzeriniz geolokasiyanı dəstəkləmir.');
              }
            }}
          >
            <i className="bi bi-crosshair me-1"></i> Cari yerimi seç
          </button>
        </div>
        <div className="col-md-12">
          <label>Ünvan (və ya xəritədə axtarılacaq yer)</label>
          <div className="input-group mb-2">
            <input 
              {...field} 
              className="form-control" 
              value={form.locationAddress} 
              onChange={set('locationAddress')} 
              placeholder="Məs: Bakı, Nizami küç. 10 və ya 28 May m/s" 
            />
            {form.locationAddress && (
              <a
                href={`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(form.locationAddress)}`}
                target="_blank"
                rel="noreferrer"
                className="btn btn-outline"
                title="Google Maps-də aç"
              >
                <i className="bi bi-box-arrow-up-right"></i>
              </a>
            )}
          </div>
        </div>

        {/* Xəritə nümayişi və koordinatlar */}
        {(form.locationAddress || (form.latitude && form.longitude)) && (
          <div className="col-md-12 mb-3">
            <div style={{ borderRadius: '8px', overflow: 'hidden', border: '1px solid var(--border-color)' }}>
              <iframe
                title="Google Map Seçimi"
                width="100%"
                height="220"
                frameBorder="0"
                style={{ border: 0, display: 'block' }}
                src={
                  form.latitude && form.longitude
                    ? `https://maps.google.com/maps?q=${form.latitude},${form.longitude}&t=&z=15&ie=UTF8&iwloc=&output=embed`
                    : `https://maps.google.com/maps?q=${encodeURIComponent(form.locationAddress)}&t=&z=14&ie=UTF8&iwloc=&output=embed`
                }
                allowFullScreen
              />
            </div>
            <div className="d-flex justify-content-between align-items-center mt-1">
              <span className="text-muted small">
                {form.latitude && form.longitude ? (
                  <>Koordinatlar: {form.latitude}, {form.longitude}</>
                ) : (
                  <>Ünvana uyğun Google Map nümayişi</>
                )}
              </span>
              <a
                href={
                  form.latitude && form.longitude
                    ? `https://www.google.com/maps?q=${form.latitude},${form.longitude}`
                    : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(form.locationAddress)}`
                }
                target="_blank"
                rel="noreferrer"
                className="small text-accent text-decoration-none"
              >
                Google Maps-də böyüt <i className="bi bi-box-arrow-up-right"></i>
              </a>
            </div>
          </div>
        )}
      </div>

      <div className="d-flex gap-2 mt-2">
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold" disabled={busy}>{busy ? 'Yadda saxlanılır...' : 'Yadda saxla'}</button>
        <button type="button" className="btn-outline" onClick={() => navigate('/warehouse')}>Geri</button>
      </div>

      {isEdit && (
        <div className="mt-4">
          <div className="d-flex justify-content-between align-items-center mb-2">
            <h4 className="mb-0">Şəkillər</h4>
            <span className="badge bg-secondary">{images.length} / 10 şəkil</span>
          </div>
          <div className="d-flex gap-2 flex-wrap mb-2">
            {images.map(img => (
              <div key={img.id} style={{ position: 'relative' }}>
                <img src={imageSrc(img.url)} alt="" style={{ width: 96, height: 96, objectFit: 'cover', borderRadius: 8, border: img.isMain ? '2px solid var(--accent)' : '1px solid var(--border-color)' }} />
                <button type="button" onClick={() => removeImage(img.id)} style={{ position: 'absolute', top: 2, right: 2, background: '#000a', color: '#fff', border: 'none', borderRadius: 4 }}><i className="bi bi-x"></i></button>
              </div>
            ))}
          </div>
          {images.length < 10 ? (
            <>
              <input type="file" accept=".jpg,.jpeg,.png,.webp" onChange={upload} className="form-control" />
              <small className="text-muted">JPG, PNG, WEBP · maks. 5 MB (Maksimum 10 şəkil)</small>
            </>
          ) : (
            <div className="alert alert-info py-2 mb-0">Maksimum 10 şəkil yüklənib. Yeni şəkil əlavə etmək üçün mövcud olanlardan birini silin.</div>
          )}
        </div>
      )}
    </form>
  );
}

export default PartForm;


