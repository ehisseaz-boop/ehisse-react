import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Api } from '../api';
import { formStyle } from './Login';
import { usePageSeo } from '../utils/seo';

function ForgotPassword() {
  usePageSeo({
    title: 'Şifrənin bərpası',
    description: 'Şifrənizi unutmusunuzsa, email və ya nömrənizlə təsdiq kodu alaraq asanlıqla bərpa edin.'
  });

  // Step 1: 'request' (username/email/phone)
  // Step 2: 'otp' (6-digit code)
  // Step 3: 'reset' (new password)
  const [step, setStep] = useState('request');
  const [username, setUsername] = useState('');
  const [maskedEmail, setMaskedEmail] = useState('');
  const [verifiedUsername, setVerifiedUsername] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();

  // 1-ci Addım: İstifadəçi adını/nömrəni göndərib emailə 6 rəqəmli OTP kod istəmək
  const handleRequestCode = async (e) => {
    e.preventDefault();
    setError('');
    setBusy(true);
    try {
      const res = await Api.forgotPassword({ username: username.trim() });
      setVerifiedUsername(res.username);
      setMaskedEmail(res.email || '');
      setStep('otp');
    } catch (err) {
      setError(err.message || 'İstifadəçi tapılmadı və ya xəta baş verdi.');
    } finally {
      setBusy(false);
    }
  };

  // 2-ci Addım: Emailə gələn 6 rəqəmli OTP kodu təsdiq etmək
  const handleVerifyOtp = async (e) => {
    e.preventDefault();
    if (otpCode.trim().length !== 6) {
      setError('Zəhmət olmasa emailinizə göndərilən 6 rəqəmli kodu tam daxil edin.');
      return;
    }
    setError('');
    setBusy(true);
    try {
      await Api.verifyResetOtp({ username: verifiedUsername, code: otpCode.trim() });
      setStep('reset');
    } catch (err) {
      setError(err.message || 'Təsdiq kodu yanlışdır və ya vaxtı bitib.');
    } finally {
      setBusy(false);
    }
  };

  // 3-cü Addım: Yeni şifrəni təyin etmək
  const handleResetPassword = async (e) => {
    e.preventDefault();
    if (newPassword.length < 6) {
      setError('Şifrə minimum 6 simvoldan ibarət olmalıdır.');
      return;
    }
    if (newPassword !== confirmPassword) {
      setError('Daxil edilən şifrələr bir-biri ilə eyni deyil.');
      return;
    }
    setError('');
    setBusy(true);
    try {
      await Api.resetPassword({
        username: verifiedUsername,
        code: otpCode.trim(),
        newPassword
      });
      navigate('/thank-you', {
        state: {
          message: 'Şifrəniz uğurla yeniləndi! İndi yeni şifrənizlə daxil ola bilərsiniz.'
        }
      });
    } catch (err) {
      setError(err.message || 'Şifrəni yeniləyərkən xəta baş verdi.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="container">
      <div className="product-card" style={formStyle}>
        <h2 className="section-title">Şifrə bərpası</h2>
        
        {error && <div className="alert alert-danger" style={{ fontSize: '14px' }}>{error}</div>}

        {step === 'request' && (
          <form onSubmit={handleRequestCode}>
            <p className="text-muted" style={{ fontSize: '14px' }}>
              Qeydiyyatdan keçdiyiniz mobil nömrəni və ya email ünvanını daxil edin.
            </p>
            <input
              className="form-control mb-3"
              placeholder="Email və ya nömrə (məs: 0501234567)"
              value={username}
              onChange={e => setUsername(e.target.value)}
              required
              autoFocus
            />
            <button
              type="submit"
              className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2"
              disabled={busy}
            >
              {busy ? (
                <>
                  <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  <span>Kod göndərilir...</span>
                </>
              ) : 'Təsdiq kodu göndər'}
            </button>
          </form>
        )}

        {step === 'otp' && (
          <form onSubmit={handleVerifyOtp}>
            <div className="alert alert-info" style={{ fontSize: '13px' }}>
              Təsdiq kodu <b>{maskedEmail || 'poçt ünvanınıza'}</b> göndərildi. Zəhmət olmasa emailinizi (həmçinin Spam qovluğunu) yoxlayın.
            </div>
            <label className="form-label" style={{ fontSize: '13px', fontWeight: 'bold' }}>
              6 rəqəmli təsdiq kodu:
            </label>
            <input
              className="form-control mb-3 text-center"
              style={{ fontSize: '22px', letterSpacing: '6px', fontWeight: 'bold' }}
              placeholder="••••••"
              maxLength={6}
              value={otpCode}
              onChange={e => setOtpCode(e.target.value.replace(/\D/g, '').slice(0, 6))}
              required
              autoFocus
            />
            <button
              type="submit"
              className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2"
              disabled={busy}
            >
              {busy ? (
                <>
                  <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  <span>Yoxlanılır...</span>
                </>
              ) : 'Kodu təsdiq et'}
            </button>
            <div className="mt-2 text-center">
              <button
                type="button"
                className="btn btn-link text-muted"
                style={{ fontSize: '13px', textDecoration: 'none' }}
                onClick={() => setStep('request')}
              >
                Geri qayıt və ya nömrəni dəyiş
              </button>
            </div>
          </form>
        )}

        {step === 'reset' && (
          <form onSubmit={handleResetPassword}>
            <p className="text-muted" style={{ fontSize: '14px' }}>
              Təsdiq uğurludur! Zəhmət olmasa yeni şifrənizi daxil edin.
            </p>
            <input
              className="form-control mb-3"
              type="password"
              placeholder="Yeni şifrə (minimum 6 simvol)"
              minLength={6}
              value={newPassword}
              onChange={e => setNewPassword(e.target.value)}
              required
              autoFocus
            />
            <input
              className="form-control mb-3"
              type="password"
              placeholder="Yeni şifrəni təkrar daxil edin"
              minLength={6}
              value={confirmPassword}
              onChange={e => setConfirmPassword(e.target.value)}
              required
            />
            <button
              type="submit"
              className="btn btn-primary bg-accent text-dark fw-bold w-100 d-flex align-items-center justify-content-center gap-2"
              disabled={busy}
            >
              {busy ? (
                <>
                  <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                  <span>Şifrə yenilənir...</span>
                </>
              ) : 'Şifrəni tamamla'}
            </button>
          </form>
        )}

        <div className="mt-3 text-center">
          <Link to="/login" className="link-accent" style={{ fontSize: '14px' }}>
            ← Giriş səhifəsinə qayıt
          </Link>
        </div>
      </div>
    </div>
  );
}

export default ForgotPassword;
