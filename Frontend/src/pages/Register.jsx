import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Api } from '../api';
import { formStyle } from './Login';
import { Modal } from '../components/Panel';
import { usePageSeo } from '../utils/seo';

function Register({ onRegister }) {
  usePageSeo({
    title: 'Qeydiyyat',
    description: 'e-hisse.az platformasında usta və ya anbardar kimi qeydiyyatdan keçin.'
  });

  const navigate = useNavigate();
  const [form, setForm] = useState({ 
    firstName: '', 
    lastName: '', 
    operator: '50', 
    phone: '', 
    email: '', 
    pin: '',
    idCardImageUrl: '',
    password: '', 
    role: 2 
  });
  const [idCardFile, setIdCardFile] = useState(null);
  const [idCardPreview, setIdCardPreview] = useState('');
  const [uploadingImage, setUploadingImage] = useState(false);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const [showOtp, setShowOtp] = useState(false);
  const [otp, setOtp] = useState('');

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handlePhoneChange = (e) => {
    const val = e.target.value.replace(/\D/g, '').slice(0, 7);
    setForm(prev => ({ ...prev, phone: val }));
  };

  const handlePinChange = (e) => {
    // FİN kod 7 simvol (rəqəm və hərflər, böyük hərflərlə)
    const val = e.target.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase().slice(0, 7);
    setForm(prev => ({ ...prev, pin: val }));
  };

  const handleIdCardFileChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (file.size > 10 * 1024 * 1024) {
      setError('Şəkil ölçüsü 10 MB-dan çox ola bilməz.');
      return;
    }

    setIdCardFile(file);
    setIdCardPreview(URL.createObjectURL(file));

    // Dərhal serverə yükləyirik və URL əldə edirik
    setUploadingImage(true);
    setError('');
    try {
      const fd = new FormData();
      fd.append('file', file);
      const res = await fetch('/api/FileUpload/upload', {
        method: 'POST',
        body: fd
      });
      const data = await res.json();
      if (!res.ok || !data.url) {
        throw new Error(data.message || 'Şəkil yüklənərkən xəta baş verdi.');
      }
      setForm(prev => ({ ...prev, idCardImageUrl: data.url }));
    } catch (err) {
      setError('Vəsiqə şəkli yüklənərkən xəta: ' + err.message);
    } finally {
      setUploadingImage(false);
    }
  };

  const validate = () => {
    if (!form.firstName.trim()) return 'Zəhmət olmasa adınızı daxil edin.';
    if (!form.lastName.trim()) return 'Zəhmət olmasa soyadınızı daxil edin.';
    if (!form.email.trim()) return 'Zəhmət olmasa e-poçt ünvanınızı daxil edin.';
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(form.email.trim())) return 'E-poçt ünvanı düzgün formatda deyil (məs: usta@gmail.com).';
    
    const cleanNumber = form.phone.replace(/\D/g, '');
    if (cleanNumber.length !== 7) return 'Mobil nömrə 7 rəqəmli olmalıdır (məsələn: 123 45 67).';

    if (!form.pin.trim() || form.pin.trim().length !== 7) {
      return 'FİN kod mütləqdir və 7 simvoldan ibarət olmalıdır (məs: 7ABC123).';
    }

    if (!form.idCardImageUrl) {
      return 'Zəhmət olmasa şəxsiyyət vəsiqəsinin şəklini yükləyin.';
    }

    if (!form.password || form.password.length < 6) return 'Şifrə minimum 6 simvoldan ibarət olmalıdır.';
    return null;
  };

  const submit = async (e) => {
    e.preventDefault();
    const valError = validate();
    if (valError) {
      setError(valError);
      return;
    }
    setError('');
    setBusy(true);
    try {
      // 1. Əvvəlcə istifadəçinin daxil etdiyi email ünvanına 4 rəqəmli OTP kod göndəririk
      await Api.sendRegisterOtp({ email: form.email.trim() });
      setShowOtp(true);
    } catch (err) {
      setError(err.message || 'Təsdiq kodu göndərilərkən xəta baş verdi.');
    } finally {
      setBusy(false);
    }
  };

  const verifyOtp = async (e) => {
    e.preventDefault();
    if (otp.length !== 4 || !/^\d{4}$/.test(otp)) {
      setError('Təsdiq kodu 4 rəqəmli olmalıdır.');
      return;
    }
    setError('');
    setBusy(true);
    try {
      const cleanPhone = form.phone.replace(/\D/g, '');
      const fullPhone = `+994${form.operator}${cleanPhone}`;
      // 2. Kodu backend-də yoxlayıb istifadəçini qeydiyyatdan keçiririk
      const res = await Api.register({ 
        ...form, 
        phone: fullPhone, 
        role: Number(form.role), 
        pin: form.pin.trim().toUpperCase(),
        idCardImageUrl: form.idCardImageUrl,
        code: otp.trim() 
      });
      if (res && res.username) {
        const loggedUser = {
          isAuthenticated: true,
          id: res.id,
          username: res.username,
          fullName: res.fullName,
          role: res.role,
          email: res.email
        };
        localStorage.setItem('ehisse_user', JSON.stringify(loggedUser));
      }
      setShowOtp(false);
      await onRegister?.();
      navigate('/thank-you', {
        state: {
          message: 'Qeydiyyatınız uğurla tamamlandı və hesabınıza daxil oldunuz! Giriş məlumatlarınız və şifrəniz e-poçt ünvanınıza göndərildi. İndi birbaşa elan yerləşdirə bilərsiniz.'
        }
      });
    } catch (err) {
      setError(err.message || 'Təsdiq kodu yanlışdır və ya xəta baş verdi.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="container">
      <form onSubmit={submit} className="product-card" style={formStyle} autoComplete="off">
        {/* Chrome-un formanı zorla doldurmasının qarşısını almaq üçün gizli sahələr */}
        <input type="text" style={{ display: 'none' }} tabIndex={-1} autoComplete="off" />
        <input type="password" style={{ display: 'none' }} tabIndex={-1} autoComplete="off" />

        <h2 className="section-title">Qeydiyyat</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        <input className="form-control mb-2" placeholder="Ad" value={form.firstName} onChange={set('firstName')} required autoComplete="off" />
        <input className="form-control mb-2" placeholder="Soyad" value={form.lastName} onChange={set('lastName')} required autoComplete="off" />
        
        <div className="d-flex mb-2 gap-2" style={{ alignItems: 'center' }}>
          <span style={{ fontWeight: 'bold' }}>+994</span>
          <select className="form-select" style={{ width: '90px' }} value={form.operator} onChange={set('operator')}>
            <option value="50">50</option>
            <option value="51">51</option>
            <option value="55">55</option>
            <option value="70">70</option>
            <option value="77">77</option>
            <option value="99">99</option>
            <option value="10">10</option>
            <option value="60">60</option>
          </select>
          <input 
            className="form-control" 
            placeholder="xxx-xx-xx" 
            value={form.phone} 
            onChange={handlePhoneChange} 
            maxLength={7}
            inputMode="numeric"
            pattern="[0-9]*"
            autoComplete="off"
            required 
          />
        </div>

        <input className="form-control mb-2" type="email" placeholder="Email" value={form.email} onChange={set('email')} autoComplete="off" required />

        {/* FİN kod */}
        <div className="mb-2">
          <input 
            className="form-control" 
            placeholder="FİN kod (7 simvol, məs: 7ABC123)" 
            value={form.pin} 
            onChange={handlePinChange} 
            maxLength={7}
            autoComplete="off"
            style={{ textTransform: 'uppercase', letterSpacing: '2px', fontWeight: 'bold' }}
            required 
          />
          <div className="text-muted small mt-1" style={{ fontSize: '11px' }}>
            Şəxsiyyət vəsiqənizin sağ aşağı küncündəki 7 simvollu kod.
          </div>
        </div>

        {/* Vəsiqə şəkli */}
        <div className="mb-3">
          <label className="form-label small text-muted mb-1 d-block text-start">
            Şəxsiyyət vəsiqəsinin şəkli (üz tərəfi):
          </label>
          <input 
            type="file" 
            className="form-control form-control-sm" 
            accept="image/*" 
            onChange={handleIdCardFileChange}
            required 
          />
          {uploadingImage && (
            <div className="text-info small mt-1 d-flex align-items-center gap-1">
              <span className="spinner-border spinner-border-sm" role="status"></span>
              <span>Şəkil yüklənir...</span>
            </div>
          )}
          {idCardPreview && !uploadingImage && (
            <div className="mt-2 text-start">
              <img 
                src={idCardPreview} 
                alt="Vəsiqə önbaxış" 
                style={{ maxHeight: '110px', borderRadius: '6px', border: '1px solid var(--border-color)', objectFit: 'cover' }} 
              />
              <span className="text-success small ms-2"><i className="bi bi-check-circle-fill"></i> Yükləndi</span>
            </div>
          )}
        </div>

        <input className="form-control mb-2" type="password" placeholder="Şifrə (min 6 simvol)" minLength={6} value={form.password} onChange={set('password')} autoComplete="new-password" required />
        <select className="form-select mb-3" value={form.role} onChange={set('role')}>
          <option value={2}>Usta</option>
          <option value={3}>Anbardar</option>
        </select>
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2" disabled={busy || uploadingImage}>
          {busy ? (
            <>
              <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
              <span>Gözləyin...</span>
            </>
          ) : (
            'Qeydiyyatdan keç'
          )}
        </button>
        <div className="mt-3"><Link to="/login" className="link-accent">Artıq hesabım var</Link></div>
      </form>

      <Modal open={showOtp} title="E-poçt OTP Təsdiqi" onClose={() => setShowOtp(false)}>
        <form onSubmit={verifyOtp}>
          <div className="alert alert-info" style={{ fontSize: '13px' }}>
            Təsdiq kodu <b>{form.email}</b> poçt ünvanınıza göndərildi. Zəhmət olmasa emailinizi yoxlayın.
          </div>
          <p style={{ fontSize: '14px', marginBottom: '8px' }}>E-poçtunuza göndərilən 4 rəqəmli kodu daxil edin:</p>
          <input
            className="form-control mb-3 text-center"
            style={{ fontSize: '20px', letterSpacing: '4px', fontWeight: 'bold' }}
            placeholder="••••"
            value={otp}
            onChange={e => setOtp(e.target.value.replace(/\D/g, '').slice(0, 4))}
            maxLength={4}
            required
            autoFocus
          />
          <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100" disabled={busy}>
            {busy ? 'Yoxlanılır...' : 'Təsdiqlə və Tamamla'}
          </button>
        </form>
      </Modal>
    </div>
  );
}

export default Register;
