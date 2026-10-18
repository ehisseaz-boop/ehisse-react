import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Api } from '../api';
import { formStyle } from './Login';
import { usePageSeo } from '../utils/seo';

function ForgotPassword() {
  usePageSeo({
    title: 'Şifrənin bərpası',
    description: 'Şifrənizi unutmusunuzsa, email və ya nömrənizlə şifrəni asanlıqla bərpa edin.'
  });

  const [username, setUsername] = useState('');
  const [verified, setVerified] = useState(null);
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();

  const check = async (e) => {
    e.preventDefault(); 
    setError('');
    setBusy(true);
    try { 
      const r = await Api.forgotPassword({ username }); 
      setVerified(r.username); 
    }
    catch (err) { 
      setError(err.message || 'İstifadəçi tapılmadı.'); 
    }
    finally {
      setBusy(false);
    }
  };

  const reset = async (e) => {
    e.preventDefault(); 
    setError('');
    setBusy(true);
    try { 
      await Api.resetPassword({ username: verified, newPassword }); 
      navigate('/thank-you', {
        state: {
          message: 'Şifrəniz uğurla yeniləndi! İndi yeni şifrənizlə daxil ola bilərsiniz.'
        }
      });
    }
    catch (err) { 
      setError(err.message || 'Şifrəni yeniləyərkən xəta baş verdi.'); 
    }
    finally {
      setBusy(false);
    }
  };

  return (
    <div className="container">
      <form onSubmit={verified ? reset : check} className="product-card" style={formStyle}>
        <h2 className="section-title">Şifrə bərpası</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        {!verified ? (
          <input className="form-control mb-3" placeholder="Email, telefon və ya istifadəçi adı" value={username} onChange={e => setUsername(e.target.value)} required />
        ) : (
          <input className="form-control mb-3" type="password" placeholder="Yeni şifrə (min 6 simvol)" minLength={6} value={newPassword} onChange={e => setNewPassword(e.target.value)} required />
        )}
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2" disabled={busy}>
          {busy ? (
            <>
              <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
              <span>Gözləyin...</span>
            </>
          ) : (
            verified ? 'Şifrəni yenilə' : 'Davam et'
          )}
        </button>
        <div className="mt-3"><Link to="/login" className="link-accent">Girişə qayıt</Link></div>
      </form>
    </div>
  );
}

export default ForgotPassword;

