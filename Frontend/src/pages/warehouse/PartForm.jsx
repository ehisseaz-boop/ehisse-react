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

  // Şəkil seçimi (həm yeni elan, həm redaktə üçün)
  const [pendingFiles, setPendingFiles] = useState([]); // Yeni elan üçün local File array

  const handleFileSelect = async (e) => {
    const files = Array.from(e.target.files || []);
    e.target.value = '';
    if (!files.length) return;

    if (isEdit) {
      // Redaktə rejimində birbaşa serverə yükləyirik
      setError(''); setMsg('');
      for (const file of files) {
        if (images.length >= 10) break;
        try {
          const r = await Api.images.upload(id, file, images.length === 0);
          setImages(imgs => [...imgs.map(i => (r.image.isMain ? { ...i, isMain: false } : i)), r.image]);
        } catch (err) {
          setError(err.message);
          break;
        }
      }
    } else {
      // Yeni elan yaradarkən: faylları və onların blob preview-larını yadda saxlayırıq
      const availableSlots = 10 - pendingFiles.length;
      const validFiles = files.slice(0, availableSlots);
      const newItems = validFiles.map((file, idx) => ({
        file,
        previewUrl: URL.createObjectURL(file),
        isMain: pendingFiles.length === 0 && idx === 0
      }));
      setPendingFiles(prev => [...prev, ...newItems]);
    }
  };

  const removePendingFile = (index) => {
    setPendingFiles(prev => {
      const updated = prev.filter((_, i) => i !== index);
      if (updated.length > 0 && !updated.some(x => x.isMain)) {
        updated[0].isMain = true;
      }
      return updated;
    });
  };

  const setPendingMain = (index) => {
    setPendingFiles(prev => prev.map((item, idx) => ({ ...item, isMain: idx === index })));
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
        const newPartId = r.id;

        // Əgər seçilmiş şəkillər varsa, ardıcıl olaraq yeni elana yüklə
        if (newPartId && pendingFiles.length > 0) {
          for (const item of pendingFiles) {
            try {
              await Api.images.upload(newPartId, item.file, item.isMain);
            } catch (imgErr) {
              console.error("Şəkil yüklənərkən xəta:", imgErr);
            }
          }
        }

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

  const removeImage = async (imageId) => {
    if (!window.confirm('Şəkli silmək istəyirsiniz?')) return;
    try {
      await Api.images.remove(imageId);
      const p = await Api.warehouse.get(id);
      setImages(p.images || []);
    } catch (err) { setError(err.message); }
  };

  if (loadError) return <ErrorBox message={loadError} />;
  if (!lookups) return <Loading />;

  const field = { className: 'form-control mb-2' };

  // Xəritə URL təyini
  const mapQuery = form.latitude && form.longitude
    ? `${form.latitude},${form.longitude}`
    : (form.locationAddress?.trim() || 'Baku, Azerbaijan');

  const mapEmbedUrl = `https://maps.google.com/maps?q=${encodeURIComponent(mapQuery)}&t=&z=14&ie=UTF8&iwloc=&output=embed`;

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
      {isEdit && form.moderationStatus && <p className="text-muted">Əsas sahələrin dəyişdirilməsi elanı yenidən moderasiyaya göndərir.</p>}
      {error && <div className="alert alert-danger">{error}</div>}
      {msg && <div className="alert alert-success">{msg}</div>}

      {/* ── 1. ŞƏKİL ƏLAVƏ ETMƏK ƏN BAŞDA ── */}
      <div className="p-3 mb-4 rounded" style={{ background: 'var(--bg-main, #141414)', border: '1px solid var(--border-color)' }}>
        <div className="d-flex justify-content-between align-items-center mb-2">
          <label className="fw-bold m-0 text-main" style={{ fontSize: '1.05rem' }}>
            <i className="bi bi-images text-accent me-2"></i>Məhsulun şəkilləri
          </label>
          <span className="badge bg-secondary">
            {isEdit ? images.length : pendingFiles.length} / 10 şəkil
          </span>
        </div>
        <p className="text-muted small mb-3">
          Birinci şəkil əsas şəkil kimi istifadə olunacaq. JPG, PNG, WEBP (maks. 5 MB).
        </p>

        {/* Mövcud şəkillər və ya seçilmiş preview-lar */}
        <div className="d-flex gap-2 flex-wrap mb-3">
          {isEdit ? (
            images.map(img => (
              <div key={img.id} style={{ position: 'relative' }}>
                <img
                  src={imageSrc(img.url)}
                  alt=""
                  style={{ width: 100, height: 100, objectFit: 'cover', borderRadius: 8, border: img.isMain ? '3px solid var(--accent)' : '1px solid var(--border-color)' }}
                />
                {img.isMain && (
                  <span className="badge bg-accent text-dark position-absolute bottom-0 start-0 m-1" style={{ fontSize: '0.65rem' }}>Əsas</span>
                )}
                <button
                  type="button"
                  onClick={() => removeImage(img.id)}
                  style={{ position: 'absolute', top: 3, right: 3, background: 'rgba(0,0,0,0.7)', color: '#fff', border: 'none', borderRadius: '50%', width: 24, height: 24, display: 'flex', alignItems: 'center', justifyContent: 'center' }}
                  title="Sil"
                >
                  <i className="bi bi-x"></i>
                </button>
              </div>
            ))
          ) : (
            pendingFiles.map((item, idx) => (
              <div key={idx} style={{ position: 'relative', cursor: 'pointer' }} onClick={() => setPendingMain(idx)} title="Əsas şəkil etmək üçün klikləyin">
                <img
                  src={item.previewUrl}
                  alt=""
                  style={{ width: 100, height: 100, objectFit: 'cover', borderRadius: 8, border: item.isMain ? '3px solid var(--accent)' : '1px solid var(--border-color)' }}
                />
                {item.isMain && (
                  <span className="badge bg-accent text-dark position-absolute bottom-0 start-0 m-1" style={{ fontSize: '0.65rem' }}>Əsas</span>
                )}
                <button
                  type="button"
                  onClick={(e) => { e.stopPropagation(); removePendingFile(idx); }}
                  style={{ position: 'absolute', top: 3, right: 3, background: 'rgba(0,0,0,0.7)', color: '#fff', border: 'none', borderRadius: '50%', width: 24, height: 24, display: 'flex', alignItems: 'center', justifyContent: 'center' }}
                  title="Sil"
                >
                  <i className="bi bi-x"></i>
                </button>
              </div>
            ))
          )}

          {((isEdit && images.length < 10) || (!isEdit && pendingFiles.length < 10)) && (
            <label
              className="d-flex flex-column align-items-center justify-content-center rounded"
              style={{
                width: 100,
                height: 100,
                border: '2px dashed var(--accent)',
                cursor: 'pointer',
                background: 'rgba(0, 210, 106, 0.05)',
                color: 'var(--accent)',
                transition: 'all 0.2s'
              }}
            >
              <i className="bi bi-cloud-arrow-up" style={{ fontSize: '1.8rem' }}></i>
              <span style={{ fontSize: '0.75rem', marginTop: 4 }}>Şəkil seç</span>
              <input
                type="file"
                multiple
                accept=".jpg,.jpeg,.png,.webp"
                onChange={handleFileSelect}
                style={{ display: 'none' }}
              />
            </label>
          )}
        </div>
      </div>

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

      {/* ── GOOGLE MAPS İNTEQRASİYASI VƏ ÜNVAN ── */}
      <div className="row mt-4 p-3 rounded" style={{ background: 'var(--bg-main, #141414)', border: '1px solid var(--border-color)' }}>
        <div className="col-md-12 mb-2 d-flex justify-content-between align-items-center">
          <strong className="text-main" style={{ fontSize: '1.05rem' }}>
            <i className="bi bi-geo-alt-fill text-accent me-2"></i>Məhsulun yerləşdiyi anbar / ünvan
          </strong>
          <button
            type="button"
            className="btn btn-sm btn-outline d-flex align-items-center gap-1"
            style={{ borderRadius: '20px', fontSize: '0.85rem', borderColor: 'var(--accent)', color: 'var(--accent)' }}
            onClick={() => {
              if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                  (pos) => {
                    const lat = pos.coords.latitude.toFixed(6);
                    const lng = pos.coords.longitude.toFixed(6);
                    setForm(f => ({
                      ...f,
                      latitude: lat,
                      longitude: lng,
                      locationAddress: f.locationAddress || `Koordinat: ${lat}, ${lng}`
                    }));
                  },
                  () => alert('Cari yerinizi təyin etmək mümkün olmadı.')
                );
              } else {
                alert('Brauzeriniz geolokasiyanı dəstəkləmir.');
              }
            }}
          >
            <i className="bi bi-crosshair"></i> Cari yerimi seç
          </button>
        </div>

        <div className="col-md-12">
          <label className="text-muted small">Ünvan (və ya xəritədə axtarılacaq yer/obyekt adı)</label>
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
                title="Google Maps tətbiqində aç"
              >
                <i className="bi bi-box-arrow-up-right"></i>
              </a>
            )}
          </div>
        </div>

        {/* CANLI GOOGLE MAPS XƏRİTƏ EKRANI */}
        <div className="col-md-12 mb-2 mt-1">
          <div style={{ borderRadius: '10px', overflow: 'hidden', border: '1px solid var(--border-color)', boxShadow: '0 4px 12px rgba(0,0,0,0.3)' }}>
            <iframe
              key={mapQuery}
              title="Google Map Seçimi"
              width="100%"
              height="260"
              frameBorder="0"
              style={{ border: 0, display: 'block' }}
              src={mapEmbedUrl}
              allowFullScreen
            />
          </div>
          <div className="d-flex justify-content-between align-items-center mt-2 px-1">
            <span className="text-muted small">
              {form.latitude && form.longitude ? (
                <span><i className="bi bi-pin-map text-accent me-1"></i> Koordinatlar: <strong>{form.latitude}, {form.longitude}</strong></span>
              ) : (
                <span><i className="bi bi-info-circle me-1"></i> Ünvanı yazdıqca və ya cari yerinizi seçdikdə xəritə avtomatik yenilənir.</span>
              )}
            </span>
            <a
              href={`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(mapQuery)}`}
              target="_blank"
              rel="noreferrer"
              className="small text-accent text-decoration-none fw-semibold"
            >
              Google Maps-də aç <i className="bi bi-box-arrow-up-right ms-1"></i>
            </a>
          </div>
        </div>
      </div>

      <div className="d-flex gap-2 mt-4">
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2" disabled={busy}>
          {busy ? 'Yadda saxlanılır...' : 'Yadda saxla'}
        </button>
        <button type="button" className="btn-outline px-4 py-2" onClick={() => navigate('/warehouse')}>
          Geri
        </button>
      </div>
    </form>
  );
}

export default PartForm;


