import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { usePageSeo } from '../utils/seo';

function ThankYou() {
  const location = useLocation();
  const state = location.state || {};
  const message = state.message || 'Müraciətiniz uğurla qəbul edildi. Ən qısa zamanda sizinlə əlaqə saxlanılacaq və ya məlumatlar təsdiqlənəcəkdir.';

  usePageSeo({
    title: 'Təşəkkür Edirik',
    description: 'Əməliyyatınız uğurla yerinə yetirildi.'
  });

  return (
    <div className="container py-5 text-center" style={{ maxWidth: '600px' }}>
      <div className="product-card p-5 my-4">
        <div className="mb-4">
          <div style={{
            width: '80px',
            height: '80px',
            borderRadius: '50%',
            background: 'rgba(40, 167, 69, 0.15)',
            display: 'inline-flex',
            alignItems: 'center',
            justifyContent: 'center'
          }}>
            <i className="bi bi-check-lg" style={{ fontSize: '3rem', color: '#28a745' }}></i>
          </div>
        </div>

        <h1 className="section-title mb-3">Təşəkkür edirik!</h1>
        <p className="text-muted mb-4" style={{ fontSize: '1.1rem', lineHeight: '1.6' }}>
          {message}
        </p>

        <div className="d-flex justify-content-center gap-3">
          <Link to="/" className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill text-decoration-none">
            <i className="bi bi-house me-1"></i> Ana səhifəyə qayıt
          </Link>
          <Link to="/saticilar" className="btn btn-outline px-4 py-2 rounded-pill text-decoration-none">
            Satıcılar
          </Link>
        </div>
      </div>
    </div>
  );
}

export default ThankYou;
