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
  const [form, setForm] = useState({ firstName: '', lastName: '', operator: '50', phone: '', email: '', password: '', role: 2 });
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const [showOtp, setShowOtp] = useState(false);
  const [otp, setOtp] = useState('');

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handlePhoneChange = (e) => {
    const val = e.target.value.replace(/\D/g, '').slice(0, 7);
    setForm(prev => ({ ...prev, phone: val }));
  };

  const validate = () => {
    if (!form.firstName.trim()) return 'Zəhmət olmasa adınızı daxil edin.';
    if (!form.lastName.trim()) return 'Zəhmət olmasa soyadınızı daxil edin.';
    if (!form.email.trim()) return 'Zəhmət olmasa e-poçt ünvanınızı daxil edin.';
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(form.email.trim())) return 'E-poçt ünvanı düzgün formatda deyil (məs: usta@gmail.com).';
    
    const cleanNumber = form.phone.replace(/\D/g, '');
    if (cleanNumber.length !== 7) return 'Mobil nömrə 7 rəqəmli olmalıdır (məsələn: 123 45 67).';
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
      const res = await Api.register({ ...form, phone: fullPhone, role: Number(form.role), code: otp.trim() });
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
            <option value="50">050</option>
            <option value="51">051</option>
            <option value="55">055</option>
            <option value="70">070</option>
            <option value="77">077</option>
            <option value="99">099</option>
            <option value="10">010</option>
            <option value="60">060</option>
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
        <input className="form-control mb-2" type="password" placeholder="Şifrə (min 6 simvol)" minLength={6} value={form.password} onChange={set('password')} autoComplete="new-password" required />
        <select className="form-select mb-3" value={form.role} onChange={set('role')}>
          <option value={2}>Usta</option>
          <option value={3}>Anbardar</option>
        </select>
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2" disabled={busy}>
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
