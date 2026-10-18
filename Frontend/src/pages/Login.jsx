import React, { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { Api } from '../api';
import { usePageSeo } from '../utils/seo';

export const formStyle = { maxWidth: 420, margin: '3rem auto' };

function Login({ onLogin }) {
  usePageSeo({
    title: 'Daxil ol',
    description: 'e-hisse.az hesabınıza daxil olun və elanlarınızı idarə edin.'
  });

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from || '/';

  const submit = async (e) => {
    e.preventDefault();
    setError(''); setBusy(true);
    try {
      const res = await Api.login({ username, password });
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
      await onLogin?.();
      navigate(from, { replace: true });
    } catch (err) {
      setError(err.message || 'İstifadəçi adı və ya şifrə yanlışdır.');
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

        <h2 className="section-title">Daxil ol</h2>
        {error && <div className="alert alert-danger">{error}</div>}
        <input 
          className="form-control mb-2" 
          placeholder="Email, telefon və ya istifadəçi adı" 
          name="login_user_field"
          id="login_user_field"
          value={username} 
          onChange={e => setUsername(e.target.value)} 
          required 
          autoComplete="new-password" 
        />
        <input 
          className="form-control mb-3" 
          type="password" 
          placeholder="Şifrə" 
          name="login_pass_field"
          id="login_pass_field"
          value={password} 
          onChange={e => setPassword(e.target.value)} 
          required 
          autoComplete="new-password" 
        />
        <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2" disabled={busy}>
          {busy ? (
            <>
              <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
              <span>Gözləyin...</span>
            </>
          ) : (
            'Daxil ol'
          )}
        </button>
        <div className="d-flex justify-content-between mt-3">
          <Link to="/forgot-password" className="link-accent">Şifrəni unutdum</Link>
          <Link to="/register" className="link-accent">Qeydiyyat</Link>
        </div>
      </form>
    </div>
  );
}

export default Login;
