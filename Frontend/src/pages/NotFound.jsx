import React from 'react';
import { Link } from 'react-router-dom';
import { usePageSeo } from '../utils/seo';

function NotFound() {
  usePageSeo({
    title: '404 - Səhifə Tapılmadı',
    description: 'Axtardığınız səhifə silinib, adı dəyişdirilib və ya müvəqqəti olaraq əlçatmazdır.'
  });

  return (
    <div className="container py-5 text-center" style={{ maxWidth: '640px' }}>
      <div className="product-card p-5 my-5 border-dashed" style={{ position: 'relative', overflow: 'hidden' }}>
        <div style={{
          fontSize: '6rem',
          fontWeight: 900,
          color: 'var(--accent)',
          lineHeight: 1,
          letterSpacing: '-2px',
          textShadow: '0 0 30px rgba(245, 158, 11, 0.3)'
        }}>
          404
        </div>
        
        <h2 className="section-title mt-3 mb-2">Səhifə Tapılmadı</h2>
        <p className="text-muted mb-4" style={{ fontSize: '1.05rem', lineHeight: '1.6' }}>
          Axtardığınız detal və ya səhifə mövcud deyil. Ola bilsin ki, link köhnəlib və ya ünvan yanlış yazılıb.
        </p>

        <div className="d-flex flex-wrap justify-content-center gap-3">
          <Link to="/" className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill text-decoration-none">
            <i className="bi bi-search me-1"></i> Kataloqa keç
          </Link>
          <Link to="/yardim" className="btn btn-outline px-4 py-2 rounded-pill text-decoration-none">
            <i className="bi bi-question-circle me-1"></i> Kömək mərkəzi
          </Link>
          <Link to="/elaqe" className="btn btn-outline px-4 py-2 rounded-pill text-decoration-none">
            Əlaqə
          </Link>
        </div>
      </div>
    </div>
  );
}

export default NotFound;
