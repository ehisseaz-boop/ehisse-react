import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { Table, fmtDate } from '../../components/Panel';
import { alertModal } from '../../utils/confirmModal';

export function SalesHistory() {
  const [rows, setRows] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => { Api.warehouse.salesHistory().then(setRows).catch(e => setError(e.message)); }, []);

  if (error) return <ErrorBox message={error} />;
  if (!rows) return <Loading />;

  const total = rows.reduce((s, r) => s + r.price * r.quantity, 0);

  return (
    <>
      <p>Ümumi satış: <strong>{rows.length}</strong> · Təxmini məbləğ: <strong>{total.toFixed(2)} AZN</strong></p>
      <Table
        rows={rows}
        empty="Hələ satış qeydə alınmayıb."
        columns={[
          { key: 'timestamp', label: 'Tarix', render: r => fmtDate(r.timestamp) },
          { key: 'title', label: 'Məhsul', render: r => <Link to={`/details/${r.sparePartId}`}>{r.title}</Link> },
          { key: 'sku', label: 'SKU' },
          { key: 'quantity', label: 'Say' },
          { key: 'price', label: 'Qiymət', render: r => `${r.price} AZN` },
          { key: 'stock', label: 'Stok', render: r => `${r.previousStock} → ${r.newStock}` },
        ]}
      />
    </>
  );
}

export function Settings() {
  const [form, setForm] = useState(null);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');
  const [busy, setBusy] = useState(false);

  // Şifrə dəyişmə state-ləri
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [pwdMsg, setPwdMsg] = useState('');
  const [pwdError, setPwdError] = useState('');
  const [pwdBusy, setPwdBusy] = useState(false);

  // Email yeniləmə (OTP ilə)
  const [newEmail, setNewEmail] = useState('');
  const [emailOtp, setEmailOtp] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [emailMsg, setEmailMsg] = useState('');
  const [emailError, setEmailError] = useState('');
  const [emailBusy, setEmailBusy] = useState(false);

  const [uploadingShopPhoto, setUploadingShopPhoto] = useState(false);

  useEffect(() => {
    let cachedEmail = '';
    let cachedName = '';
    try {
      const saved = localStorage.getItem('ehisse_user');
      if (saved) {
        const u = JSON.parse(saved);
        cachedEmail = u?.email || '';
        cachedName = u?.fullName || u?.username || '';
      }
    } catch { }

    Api.warehouse.settings()
      .then(data => {
        const email = data.email || cachedEmail;
        const fullName = data.fullName || cachedName;
        const fixedCompanyName = (data.companyName || '').replace(/\(?Anbardar\)?/gi, '(Satıcı)');
        setForm({ ...data, companyName: fixedCompanyName, email, fullName });
        if (email) setNewEmail(email);
      })
      .catch(e => setError(e.message));
  }, []);

  if (!form && error) return <ErrorBox message={error} />;
  if (!form) return <Loading />;

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.type === 'checkbox' ? e.target.checked : e.target.value });

  // Profil & Əlaqə məlumatlarının yadda saxlanması
  const submitProfile = async (e) => {
    e.preventDefault();
    setError('');
    setMsg('');
    setBusy(true);
    try {
      // Əlaqə məlumatları elanda qeyd olunması məcburidir, həmişə true göndərilir
      const r = await Api.warehouse.saveSettings({
        ...form,
        latitude: form.latitude === '' || form.latitude == null ? null : Number(form.latitude),
        longitude: form.longitude === '' || form.longitude == null ? null : Number(form.longitude),
        showContactInfoInAds: true
      });
      setMsg('Məlumatlarınız yadda saxlanıldı');
      // Əgər istifadəçi avatar və ya adını dəyişibsə, localStorage və ya yenilənməni dəstəkləyək
      try {
        const saved = localStorage.getItem('ehisse_user');
        if (saved) {
          const u = JSON.parse(saved);
          u.companyName = form.companyName ? form.companyName.replace(/\(?Anbardar\)?/gi, '(Satıcı)') : form.companyName;
          if (form.shopPhotoUrl) u.shopPhotoUrl = form.shopPhotoUrl;
          localStorage.setItem('ehisse_user', JSON.stringify(u));
        }
      } catch { }
      await alertModal('Məlumatlarınız yadda saxlanıldı', 'Uğurlu əməliyyat', 'success');
    } catch (err) {
      setError(err.message || 'Xəta baş verdi.');
      alertModal(err.message || 'Məlumatları yadda saxlayarkən xəta baş verdi.', 'Xəta', 'error');
    } finally {
      setBusy(false);
    }
  };

  // Köməkçi: Şəkili client tərəfdə sıxıb DataURL əldə edirik (server problemi olarsa etibarlı fallback üçün)
  const compressToBase64 = (imgFile) => {
    return new Promise((resolve) => {
      const reader = new FileReader();
      reader.onload = (event) => {
        const img = new Image();
        img.onload = () => {
          const canvas = document.createElement('canvas');
          const MAX_WIDTH = 800;
          const MAX_HEIGHT = 800;
          let width = img.width;
          let height = img.height;

          if (width > height) {
            if (width > MAX_WIDTH) {
              height = Math.round((height * MAX_WIDTH) / width);
              width = MAX_WIDTH;
            }
          } else {
            if (height > MAX_HEIGHT) {
              width = Math.round((width * MAX_HEIGHT) / height);
              height = MAX_HEIGHT;
            }
          }
          canvas.width = width;
          canvas.height = height;
          const ctx = canvas.getContext('2d');
          ctx.drawImage(img, 0, 0, width, height);
          resolve(canvas.toDataURL('image/jpeg', 0.8));
        };
        img.src = event.target.result;
      };
      reader.readAsDataURL(imgFile);
    });
  };

  const handleShopPhotoChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (file.size > 10 * 1024 * 1024) {
      setError('Şəkil ölçüsü 10 MB-dan çox ola bilməz.');
      return;
    }

    setUploadingShopPhoto(true);
    setError('');
    try {
      // Dərhal sıxılmış DataURL əldə edirik ki, həm preview anında açılsın, həm də bazada daimi qalsın
      const base64Url = await compressToBase64(file);
      setForm(prev => ({ ...prev, shopPhotoUrl: base64Url }));

      const fd = new FormData();
      fd.append('file', file);
      fd.append('folder', 'shops');
      const res = await fetch('/api/FileUpload/upload', {
        method: 'POST',
        body: fd,
        credentials: 'include'
      });
      const data = await res.json().catch(() => null);
      if (res.ok && data?.url && data.url.startsWith('http')) {
        setForm(prev => ({ ...prev, shopPhotoUrl: data.url }));
      }
    } catch {
      // Artıq base64Url uğurla təyin olunub
    } finally {
      setUploadingShopPhoto(false);
    }
  };

  // Kabinetdən şifrə yeniləmə
  const handlePasswordChange = async (e) => {
    e.preventDefault();
    setPwdError('');
    setPwdMsg('');

    if (!oldPassword) {
      setPwdError('Zəhmət olmasa köhnə şifrənizi daxil edin.');
      return;
    }
    if (newPassword.length < 6) {
      setPwdError('Yeni şifrə minimum 6 simvoldan ibarət olmalıdır.');
      return;
    }
    if (newPassword !== confirmPassword) {
      setPwdError('Daxil edilən yeni şifrələr bir-biri ilə eyni deyil.');
      return;
    }

    setPwdBusy(true);
    try {
      const res = await Api.changePassword({
        currentPassword: oldPassword,
        newPassword
      });
      setPwdMsg(res.message || 'Şifrəniz uğurla yeniləndi!');
      setOldPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err) {
      setPwdError(err.message || 'Şifrəni yeniləyərkən xəta baş verdi.');
    } finally {
      setPwdBusy(false);
    }
  };

  // Yeni e-poçt üçün OTP göndərmə
  const handleSendEmailOtp = async () => {
    setEmailError('');
    setEmailMsg('');
    if (!newEmail || !newEmail.includes('@')) {
      setEmailError('Düzgün yeni e-poçt ünvanı daxil edin.');
      return;
    }
    if (newEmail.trim().toLowerCase() === (form.email || '').toLowerCase()) {
      setEmailError('Daxil edilən e-poçt cari e-poçt ünvanınızla eynidir.');
      return;
    }

    setEmailBusy(true);
    try {
      const res = await Api.sendEmailOtp({ newEmail: newEmail.trim() });
      setEmailMsg(res.message || 'Təsdiq kodu yeni e-poçt ünvanınıza göndərildi.');
      setOtpSent(true);
    } catch (err) {
      setEmailError(err.message || 'Kod göndərilərkən xəta baş verdi.');
    } finally {
      setEmailBusy(false);
    }
  };

  // Email OTP təsdiqi
  const handleVerifyEmailOtp = async (e) => {
    e.preventDefault();
    setEmailError('');
    setEmailMsg('');

    if (!emailOtp || emailOtp.trim().length !== 4) {
      setEmailError('Zəhmət olmasa 4 rəqəmli təsdiq kodunu daxil edin.');
      return;
    }

    setEmailBusy(true);
    try {
      const res = await Api.verifyEmailOtp({
        newEmail: newEmail.trim(),
        code: emailOtp.trim()
      });
      setEmailMsg(res.message || 'E-poçt ünvanınız uğurla təsdiqləndi və yeniləndi!');
      setForm(prev => ({ ...prev, email: res.email || newEmail.trim() }));
      setOtpSent(false);
      setEmailOtp('');
    } catch (err) {
      setEmailError(err.message || 'Təsdiq kodu yanlışdır.');
    } finally {
      setEmailBusy(false);
    }
  };

  return (
    <div style={{ maxWidth: 680 }}>
      {/* 1. Kimlik və Qeydiyyat Məlumatları (Bağlı / Disabled) */}
      <div className="product-card mb-4 p-4" style={{ borderRadius: '12px' }}>
        <h5 className="mb-3 d-flex align-items-center gap-2 fw-bold">
          <i className="bi bi-person-badge text-accent fs-5"></i>
          Şəxsiyyət Məlumatları
        </h5>
        <div className="alert alert-secondary py-2 px-3 small mb-3">
          <i className="bi bi-info-circle me-1"></i>
          Ad, Soyad, FİN kod və şəxsiyyət vəsiqəsi məlumatları vəsiqə ilə uyğunluq təşkil etdiyi üçün dəyişdirilə bilməz.
        </div>

        <div className="row g-3">
          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold text-muted">
              <i className="bi bi-person me-1"></i> Ad və Soyad (Əlaqə şəxsi)
            </label>
            <input
              className="form-control"
              value={form.fullName || `${form.firstName || ''} ${form.lastName || ''}`.trim() || '—'}
              disabled
              readOnly
              style={{ backgroundColor: 'var(--border-color)', opacity: 0.85, cursor: 'not-allowed' }}
            />
          </div>

          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold text-muted">
              <i className="bi bi-fingerprint me-1"></i> FİN Kod (Vəsiqə)
            </label>
            <input
              className="form-control"
              value={form.pin || '—'}
              disabled
              readOnly
              style={{ backgroundColor: 'var(--border-color)', opacity: 0.85, cursor: 'not-allowed', fontWeight: 'bold' }}
            />
          </div>

          <div className="col-12">
            <label className="form-label small fw-semibold text-muted">
              <i className="bi bi-card-image me-1"></i> Kimliyi təsdiq edən vəsiqə şəkli
            </label>
            {form.idCardImageUrl ? (
              <div className="d-flex align-items-center gap-2">
                <a
                  href={form.idCardImageUrl}
                  target="_blank"
                  rel="noreferrer"
                  className="btn btn-sm btn-outline-info d-inline-flex align-items-center gap-1"
                >
                  <i className="bi bi-eye"></i> Vəsiqə şəklinə bax
                </a>
                <span className="badge bg-success-subtle text-success small">Yüklənib və təsdiq olunub</span>
              </div>
            ) : (
              <input
                className="form-control"
                value="Vəsiqə şəkli ilkin qeydiyyatda təsdiqlənib"
                disabled
                readOnly
                style={{ backgroundColor: 'var(--border-color)', opacity: 0.85, cursor: 'not-allowed' }}
              />
            )}
          </div>
        </div>
      </div>

      {/* 2. Profil və Əlaqə Məlumatları Formu */}
      <form onSubmit={submitProfile} className="product-card mb-4 p-4" style={{ borderRadius: '12px' }}>
        <h5 className="mb-3 d-flex align-items-center gap-2 fw-bold">
          <i className="bi bi-building-gear text-accent fs-5"></i>
          Profil və Satıcı Məlumatları
        </h5>

        {error && <div className="alert alert-danger py-2">{error}</div>}
        {msg && <div className="alert alert-success py-2">{msg}</div>}

        <div className="mb-3">
          <label className="form-label small fw-semibold">
            <i className="bi bi-briefcase me-1"></i> Şirkət və ya Mağaza adı *
          </label>
          <input
            className="form-control"
            value={form.companyName || ''}
            onChange={set('companyName')}
            required
            placeholder="Məs: Mobil Hissələr Mərkəzi"
          />
        </div>

        <div className="row g-3 mb-3">
          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold">
              <i className="bi bi-telephone me-1"></i> Mobil nömrə (Zənglər üçün)
            </label>
            <input
              className="form-control"
              value={form.phone || ''}
              onChange={set('phone')}
              placeholder="+994 50 000 00 00"
            />
          </div>

          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold">
              <i className="bi bi-whatsapp me-1 text-success"></i> WhatsApp nömrəsi
            </label>
            <input
              className="form-control"
              value={form.whatsAppNumber || ''}
              onChange={set('whatsAppNumber')}
              placeholder="+994 50 000 00 00"
            />
          </div>
        </div>

        {/* Mağaza / Profil Şəkli */}
        <div className="mb-4">
          <label className="form-label small fw-semibold">
            <i className="bi bi-camera me-1"></i> Mağaza / Profil Şəkli
          </label>
          <div className="d-flex align-items-center gap-3">
            <div
              style={{
                width: '72px',
                height: '72px',
                borderRadius: '12px',
                border: '2px solid var(--accent)',
                overflow: 'hidden',
                backgroundColor: 'var(--bg-surface-hover)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0
              }}
            >
              {form.shopPhotoUrl ? (
                <img
                  src={form.shopPhotoUrl}
                  alt="Mağaza şəkli"
                  style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                />
              ) : (
                <i className="bi bi-shop text-muted fs-3"></i>
              )}
            </div>
            <div className="flex-grow-1">
              <input
                type="file"
                className="form-control form-control-sm mb-1"
                accept="image/*"
                onChange={handleShopPhotoChange}
                disabled={uploadingShopPhoto}
              />
              <div className="text-muted" style={{ fontSize: '12px' }}>
                {uploadingShopPhoto ? (
                  <span className="text-info"><span className="spinner-border spinner-border-sm me-1"></span>Şəkil yüklənir...</span>
                ) : (
                  'Mağazanızın loqosunu və ya vitrin şəklini yükləyin (JPG, PNG, WEBP).'
                )}
              </div>
            </div>
            {form.shopPhotoUrl && (
              <button
                type="button"
                className="btn btn-sm btn-outline-danger"
                onClick={() => setForm(f => ({ ...f, shopPhotoUrl: '' }))}
                title="Şəkli sil"
              >
                <i className="bi bi-trash"></i>
              </button>
            )}
          </div>
        </div>

        {/* ── MAĞAZA ÜNVANI VƏ BİRBAŞA GOOGLE MAPS XƏRİTƏSİ ── */}
        <div className="mb-3">
          <div className="d-flex justify-content-between align-items-center mb-1">
            <label className="form-label small fw-semibold m-0">
              <i className="bi bi-geo-alt me-1"></i> Mağaza / Anbar ünvanı
            </label>
            <button
              type="button"
              className="btn btn-sm btn-outline d-flex align-items-center gap-1"
              style={{ borderRadius: '20px', fontSize: '0.78rem', borderColor: 'var(--accent)', color: 'var(--accent)', padding: '2px 8px' }}
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
                        address: f.address || `Koordinat: ${lat}, ${lng}`
                      }));
                    },
                    () => alertModal('Cari yerinizi təyin etmək mümkün olmadı.', 'Diqqət', 'warning')
                  );
                } else {
                  alertModal('Brauzeriniz geolokasiyanı dəstəkləmir.', 'Diqqət', 'warning');
                }
              }}
            >
              <i className="bi bi-crosshair"></i> Cari yerimi təyin et
            </button>
          </div>

          <div className="input-group mb-2">
            <input
              className="form-control"
              value={form.address || ''}
              onChange={set('address')}
              placeholder="Şəhər, rayon, küçə"
            />
            {form.address && (
              <a
                href={
                  form.latitude && form.longitude
                    ? `https://www.google.com/maps?q=${form.latitude},${form.longitude}`
                    : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(form.address)}`
                }
                target="_blank"
                rel="noreferrer"
                className="btn btn-outline"
                title="Google Maps-də aç"
              >
                <i className="bi bi-box-arrow-up-right"></i>
              </a>
            )}
          </div>

          {/* DƏRHAL ÜNVANIN ALTINDAKI CANLI GOOGLE MAPS XƏRİTƏSİ */}
          <div style={{ borderRadius: '10px', overflow: 'hidden', border: '1px solid var(--border-color)', marginTop: '6px' }}>
            <iframe
              key={`${form.latitude}-${form.longitude}-${form.address}`}
              title="Google Map Mağaza Ünvanı"
              width="100%"
              height="230"
              frameBorder="0"
              style={{ border: 0, display: 'block' }}
              src={
                form.latitude && form.longitude
                  ? `https://maps.google.com/maps?q=${form.latitude},${form.longitude}&t=&z=15&ie=UTF8&iwloc=&output=embed`
                  : `https://maps.google.com/maps?q=${encodeURIComponent(form.address?.trim() || 'Baku, Azerbaijan')}&t=&z=14&ie=UTF8&iwloc=&output=embed`
              }
              allowFullScreen
            />
          </div>

          <div className="d-flex justify-content-between align-items-center mt-2 px-1">
            <span className="text-muted small" style={{ fontSize: '12px' }}>
              {form.latitude && form.longitude ? (
                <span><i className="bi bi-pin-map text-accent me-1"></i> Koordinat: <strong>{form.latitude}, {form.longitude}</strong></span>
              ) : (
                <span><i className="bi bi-info-circle me-1"></i> Ünvanı qeyd etdikcə Google Maps xəritəsi avtomatik yenilənir.</span>
              )}
            </span>
            <a
              href={
                form.latitude && form.longitude
                  ? `https://www.google.com/maps?q=${form.latitude},${form.longitude}`
                  : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(form.address?.trim() || 'Baku')}`
              }
              target="_blank"
              rel="noreferrer"
              className="small text-accent text-decoration-none fw-semibold"
              style={{ fontSize: '12px' }}
            >
              Google Maps-də aç <i className="bi bi-box-arrow-up-right ms-1"></i>
            </a>
          </div>
        </div>

        <div className="alert alert-info py-2 px-3 small mb-3 d-flex align-items-center gap-2">
          <i className="bi bi-check-circle-fill text-success fs-5"></i>
          <span>
            <b>Əlaqə məlumatları elanda məcburidir:</b> Alıcıların sizinlə birbaşa əlaqə qurması üçün mobil və WhatsApp nömrələriniz bütün aktiv elanlarınızda avtomatik göstərilir.
          </span>
        </div>

        {error && <div className="alert alert-danger py-2">{error}</div>}
        {msg && <div className="alert alert-success py-2 d-flex align-items-center gap-2"><i className="bi bi-check-circle-fill"></i> {msg}</div>}

        <button
          type="submit"
          className="btn btn-primary bg-accent text-dark fw-bold d-inline-flex align-items-center gap-2"
          disabled={busy}
        >
          {busy ? <span className="spinner-border spinner-border-sm" role="status"></span> : <i className="bi bi-check2"></i>}
          Məlumatları yadda saxla
        </button>
      </form>

      {/* 3. E-poçt Yenilənməsi (OTP ilə Təsdiq) */}
      <div className="product-card mb-4 p-4" style={{ borderRadius: '12px' }}>
        <h5 className="mb-3 d-flex align-items-center gap-2 fw-bold">
          <i className="bi bi-envelope-at text-accent fs-5"></i>
          E-poçt Ünvanının Yenilənməsi
        </h5>

        {emailError && <div className="alert alert-danger py-2">{emailError}</div>}
        {emailMsg && <div className="alert alert-success py-2">{emailMsg}</div>}

        <div className="mb-3">
          <label className="form-label small fw-semibold">Cari E-poçt</label>
          <input
            className="form-control mb-2"
            value={form.email || 'Qeyd olunmayıb'}
            disabled
            readOnly
            style={{ backgroundColor: 'var(--border-color)', opacity: 0.8 }}
          />
        </div>

        <div className="mb-3">
          <label className="form-label small fw-semibold">Yeni E-poçt ünvanı</label>
          <div className="input-group">
            <input
              type="email"
              className="form-control"
              value={newEmail}
              onChange={e => setNewEmail(e.target.value)}
              placeholder="yeni.email@example.com"
              disabled={otpSent || emailBusy}
            />
            {!otpSent ? (
              <button
                type="button"
                className="btn btn-outline"
                style={{ borderColor: 'var(--accent)', color: 'var(--accent)' }}
                onClick={handleSendEmailOtp}
                disabled={emailBusy}
              >
                {emailBusy ? <span className="spinner-border spinner-border-sm"></span> : <i className="bi bi-send me-1"></i>}
                OTP Göndər
              </button>
            ) : (
              <button
                type="button"
                className="btn btn-outline-secondary"
                onClick={() => setOtpSent(false)}
              >
                Dəyiş
              </button>
            )}
          </div>
        </div>

        {otpSent && (
          <form onSubmit={handleVerifyEmailOtp} className="mt-3 p-3" style={{ background: 'var(--border-color)', borderRadius: '8px' }}>
            <label className="form-label small fw-bold">
              Yeni emailə göndərilən 4 rəqəmli OTP kod:
            </label>
            <div className="d-flex gap-2">
              <input
                className="form-control text-center fw-bold"
                style={{ fontSize: '20px', letterSpacing: '4px', maxWidth: 160 }}
                placeholder="••••"
                maxLength={4}
                value={emailOtp}
                onChange={e => setEmailOtp(e.target.value.replace(/\D/g, '').slice(0, 4))}
                required
                autoFocus
              />
              <button
                type="submit"
                className="btn btn-success fw-bold d-inline-flex align-items-center gap-1"
                disabled={emailBusy}
              >
                {emailBusy ? <span className="spinner-border spinner-border-sm"></span> : <i className="bi bi-check-lg"></i>}
                Təsdiq et və Yenilə
              </button>
            </div>
          </form>
        )}
      </div>

      {/* 4. Şifrənin Dəyişdirilməsi (Köhnə şifrə + Yeni şifrə 2 dəfə) */}
      <form onSubmit={handlePasswordChange} className="product-card p-4" style={{ borderRadius: '12px' }}>
        <h5 className="mb-3 d-flex align-items-center gap-2 fw-bold">
          <i className="bi bi-shield-lock text-accent fs-5"></i>
          Şifrənin Dəyişdirilməsi
        </h5>

        {pwdError && <div className="alert alert-danger py-2">{pwdError}</div>}
        {pwdMsg && <div className="alert alert-success py-2">{pwdMsg}</div>}

        <div className="mb-3">
          <label className="form-label small fw-semibold">
            <i className="bi bi-key me-1"></i> Köhnə (Cari) şifrə *
          </label>
          <input
            type="password"
            className="form-control"
            value={oldPassword}
            onChange={e => setOldPassword(e.target.value)}
            placeholder="Cari şifrənizi daxil edin"
            required
          />
        </div>

        <div className="row g-3 mb-3">
          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold">
              <i className="bi bi-lock me-1"></i> Yeni şifrə *
            </label>
            <input
              type="password"
              className="form-control"
              value={newPassword}
              onChange={e => setNewPassword(e.target.value)}
              placeholder="Minimum 6 simvol"
              minLength={6}
              required
            />
          </div>

          <div className="col-12 col-sm-6">
            <label className="form-label small fw-semibold">
              <i className="bi bi-lock-fill me-1"></i> Yeni şifrənin təkrarı *
            </label>
            <input
              type="password"
              className="form-control"
              value={confirmPassword}
              onChange={e => setConfirmPassword(e.target.value)}
              placeholder="Yeni şifrəni təkrar edin"
              minLength={6}
              required
            />
          </div>
        </div>

        <button
          type="submit"
          className="btn btn-primary bg-accent text-dark fw-bold d-inline-flex align-items-center gap-2"
          disabled={pwdBusy}
        >
          {pwdBusy ? <span className="spinner-border spinner-border-sm" role="status"></span> : <i className="bi bi-shield-check"></i>}
          Şifrəni yenilə
        </button>
      </form>
    </div>
  );
}
