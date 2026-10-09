import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../api';
import { formStyle } from './Login';
import { Modal } from '../components/Panel';

function Register() {
  const [form, setForm] = useState({ firstName: '', lastName: '', operator: '50', phone: '', email: '', password: '', role: 2 });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [busy, setBusy] = useState(false);
  const [showOtp, setShowOtp] = useState(false);
  const [otp, setOtp] = useState('');

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const validate = () => {
    if (!form.firstName || !form.lastName || !form.password || !form.email) return 'Bütün məcburi sahələri doldurun';
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(form.email)) return 'Email düzgün formatda deyil';
    
    const phoneRegex = /^\d{3}-\d{2}-\d{2}$/;
    if (!phoneRegex.test(form.phone)) return 'Telefon nömrəsi düzgün formatda deyil (xxx-xx-xx)';
    if (form.password.length < 6) return 'Şifrə minimum 6 simvol olmalıdır';
    return null;
  };

  const submit = async (e) => {
    e.preventDefault();
    const valError = validate();
    if (valError) {
      setError(valError);
      return;
    }
    setError(''); setBusy(true);
    try {
      const fullPhone = `+994${form.operator}${form.phone.replace(/-/g, '')}`;
      await Api.register({ ...form, phone: fullPhone, role: Number(form.role) });
      // Simulate sending OTP
      setShowOtp(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  };

  const verifyOtp = (e) => {
    e.preventDefault();
    if (otp.length !== 4 || !/^\d{4}$/.test(otp)) {
      setError('OTP 4 rəqəmli olmalıdır');
      return;
    }
    setShowOtp(false);
    setSuccess('Uğurla yetirildi. Müraciətiniz administratora göndərildi.');
  };

  if (success) {
    return (
      <div className="container">
        <div className="product-card text-center" style={formStyle}>
          <i className="bi bi-check-circle text-accent" style={{ fontSize: '3rem' }}></i>
          <p className="mt-3">{success}</p>
          <Link to="/login" className="link-accent">Giriş səhifəsinə keç</Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <form onSubmit={submit} className="product-card" style={formStyle}>
        <h2 className="section-title">Qeydiyyat</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        <input className="form-control mb-2" placeholder="Ad" value={form.firstName} onChange={set('firstName')} required />
        <input className="form-control mb-2" placeholder="Soyad" value={form.lastName} onChange={set('lastName')} required />
        
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
          <input className="form-control" placeholder="xxx-xx-xx" value={form.phone} onChange={set('phone')} required />
        </div>

        <input className="form-control mb-2" type="email" placeholder="Email" value={form.email} onChange={set('email')} required />
        <input className="form-control mb-2" type="password" placeholder="Şifrə (min 6 simvol)" minLength={6} value={form.password} onChange={set('password')} required />
        <select className="form-select mb-3" value={form.role} onChange={set('role')}>
          <option value={2}>Usta</option>
          <option value={3}>Anbardar</option>
        </select>
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100" disabled={busy}>{busy ? '...' : 'Qeydiyyatdan keç'}</button>
        <div className="mt-3"><Link to="/login" className="link-accent">Artıq hesabım var</Link></div>
      </form>

      <Modal open={showOtp} title="OTP Təsdiqi" onClose={() => setShowOtp(false)}>
        <form onSubmit={verifyOtp}>
          <p>Telefonunuza göndərilən 4 rəqəmli kodu daxil edin:</p>
          <input className="form-control mb-3" placeholder="1234" value={otp} onChange={e => setOtp(e.target.value)} maxLength={4} required />
          <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100">Təsdiqlə</button>
        </form>
      </Modal>
    </div>
  );
}

export default Register;
