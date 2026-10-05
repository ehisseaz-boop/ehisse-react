import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Api } from '../api';
import { formStyle } from './Login';

function ForgotPassword() {
  const [username, setUsername] = useState('');
  const [verified, setVerified] = useState(null);
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const check = async (e) => {
    e.preventDefault(); setError('');
    try { const r = await Api.forgotPassword({ username }); setVerified(r.username); }
    catch (err) { setError(err.message); }
  };

  const reset = async (e) => {
    e.preventDefault(); setError('');
    try { await Api.resetPassword({ username: verified, newPassword }); navigate('/login'); }
    catch (err) { setError(err.message); }
  };

  return (
    <div className="container">
      <form onSubmit={verified ? reset : check} className="product-card" style={formStyle}>
        <h2 className="section-title">Şifrə bərpası</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        {!verified ? (
          <input className="form-control mb-3" placeholder="İstifadəçi adı" value={username} onChange={e => setUsername(e.target.value)} required />
        ) : (
          <input className="form-control mb-3" type="password" placeholder="Yeni şifrə" minLength={6} value={newPassword} onChange={e => setNewPassword(e.target.value)} required />
        )}
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100">{verified ? 'Şifrəni yenilə' : 'Davam et'}</button>
        <div className="mt-3"><Link to="/login" className="link-accent">Girişə qayıt</Link></div>
      </form>
    </div>
  );
}

export default ForgotPassword;
