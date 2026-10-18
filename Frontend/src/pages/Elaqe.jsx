import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';
import { usePageSeo } from '../utils/seo';

function Elaqe() {
  usePageSeo({
    title: 'Əlaqə',
    description: 'e-hisse.az komandası ilə əlaqə saxlayın. Telefon: +994 10 252 32 26, WhatsApp: +994 10 396 15 01, info@e-hisse.az.'
  });

  const navigate = useNavigate();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  
  // Əlaqə forması vəziyyətləri
  const [form, setForm] = useState({ name: '', email: '', subject: '', message: '' });
  const [formError, setFormError] = useState('');
  const [busy, setBusy] = useState(false);

  useEffect(() => { 
    Api.contact().then(setData).catch(e => setError(e.message)); 
  }, []);

  const handleFormSubmit = (e) => {
    e.preventDefault();
    if (!form.name.trim()) {
      setFormError('Zəhmət olmasa adınızı qeyd edin.');
      return;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!form.email.trim() || !emailRegex.test(form.email.trim())) {
      setFormError('Zəhmət olmasa düzgün e-poçt ünvanı daxil edin.');
      return;
    }
    if (!form.message.trim() || form.message.length < 10) {
      setFormError('Mesajınız ən azı 10 simvoldan ibarət olmalıdır.');
      return;
    }

    setFormError('');
    setBusy(true);

    setTimeout(() => {
      setBusy(false);
      navigate('/thank-you', {
        state: {
          message: 'Mesajınız uğurla göndərildi! Ən qısa zamanda sizinlə əlaqə saxlanılacaq.'
        }
      });
    }, 800);
  };

  if (error) return <div className="container py-4"><ErrorBox message={error} /></div>;
  if (!data) return <div className="container py-4"><Loading /></div>;

  return (
    <div className="container py-4">
      <section className="mb-5">
        <h1 className="section-title">{data.title}</h1>
        <p className="text-muted">{data.subtitle}</p>

        <div className="row g-4 mt-2">
          <div className="col-md-4">
            <div className="product-card h-100 p-4">
              <h5 className="d-flex align-items-center gap-2"><i className="bi bi-geo-alt text-accent"></i> Ünvan</h5>
              <p className="text-muted mt-2">{data.address}</p>
              <div className="small text-muted mt-auto pt-2 border-top border-secondary-subtle">
                <i className="bi bi-clock me-1 text-accent"></i> Hər gün: 09:00 – 19:00
              </div>
            </div>
          </div>

          <div className="col-md-4">
            <div className="product-card h-100 p-4">
              <h5 className="d-flex align-items-center gap-2"><i className="bi bi-telephone text-accent"></i> Telefon &amp; WhatsApp</h5>
              <p className="mb-2 mt-2">
                <a href={`tel:${data.phone.replace(/\s/g, '')}`} className="text-decoration-none text-main fw-bold">
                  {data.phone}
                </a>
              </p>
              <p className="mb-0">
                <a href={`https://wa.me/${data.whatsApp.replace(/\D/g, '')}`} target="_blank" rel="noreferrer" className="text-decoration-none" style={{ color: '#25D366' }}>
                  <i className="bi bi-whatsapp me-1"></i> WhatsApp: {data.whatsApp}
                </a>
              </p>
            </div>
          </div>

          <div className="col-md-4">
            <div className="product-card h-100 p-4">
              <h5 className="d-flex align-items-center gap-2"><i className="bi bi-envelope text-accent"></i> E-poçt</h5>
              <p className="mt-2"><a href={`mailto:${data.email}`} className="text-decoration-none link-accent">{data.email}</a></p>
              <div className="small text-muted mt-auto pt-2 border-top border-secondary-subtle">
                Bizə yazın, 24 saat ərzində cavablandırırıq.
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Mesaj göndərmə forması */}
      <section className="mt-4" style={{ maxWidth: '800px', margin: '0 auto' }}>
        <div className="product-card p-4 p-md-5">
          <h2 className="section-title mb-2">Bizə mesaj göndərin</h2>
          <p className="text-muted mb-4">Sualınız, təklifiniz və ya əməkdaşlıq müraciətiniz varsa aşağıdakı formanı doldurun.</p>

          {formError && <div className="alert alert-danger">{formError}</div>}

          <form onSubmit={handleFormSubmit}>
            <div className="row g-3">
              <div className="col-md-6">
                <label className="form-label text-main">Ad və Soyadınız *</label>
                <input 
                  type="text" 
                  className="form-control" 
                  placeholder="Məs: Əli Əliyev" 
                  value={form.name} 
                  onChange={e => setForm({ ...form, name: e.target.value })} 
                  required 
                />
              </div>
              <div className="col-md-6">
                <label className="form-label text-main">E-poçt ünvanınız *</label>
                <input 
                  type="email" 
                  className="form-control" 
                  placeholder="Məs: ali@example.com" 
                  value={form.email} 
                  onChange={e => setForm({ ...form, email: e.target.value })} 
                  required 
                />
              </div>
              <div className="col-12">
                <label className="form-label text-main">Mövzu</label>
                <input 
                  type="text" 
                  className="form-control" 
                  placeholder="Məs: Ehtiyat hissəsi sifarişi və ya elan haqqında" 
                  value={form.subject} 
                  onChange={e => setForm({ ...form, subject: e.target.value })} 
                />
              </div>
              <div className="col-12">
                <label className="form-label text-main">Mesajınız *</label>
                <textarea 
                  className="form-control" 
                  rows={4} 
                  placeholder="Mesajınızı ətraflı qeyd edin..." 
                  value={form.message} 
                  onChange={e => setForm({ ...form, message: e.target.value })} 
                  required 
                ></textarea>
              </div>
              <div className="col-12">
                <button 
                  type="submit" 
                  className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill d-inline-flex align-items-center gap-2"
                  disabled={busy}
                >
                  {busy ? (
                    <>
                      <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                      <span>Göndərilir...</span>
                    </>
                  ) : (
                    <>
                      <i className="bi bi-send-fill"></i>
                      <span>Mesajı göndər</span>
                    </>
                  )}
                </button>
              </div>
            </div>
          </form>
        </div>
      </section>
    </div>
  );
}

export default Elaqe;

