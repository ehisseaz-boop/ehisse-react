import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function CookieBanner() {
  const [accepted, setAccepted] = useState(true);

  useEffect(() => {
    const consent = localStorage.getItem('ehisse_cookie_consent');
    if (!consent) {
      setAccepted(false);
    }
  }, []);

  const handleAccept = () => {
    localStorage.setItem('ehisse_cookie_consent', 'true');
    setAccepted(true);
  };

  if (accepted) return null;

  return (
    <div
      className="position-fixed bottom-0 start-0 end-0 p-3"
      style={{
        zIndex: 1050,
        background: 'var(--bg-surface)',
        borderTop: '1px solid var(--border-color)',
        boxShadow: '0 -4px 20px rgba(0,0,0,0.3)',
      }}
    >
      <div className="container d-flex flex-column flex-md-row align-items-center justify-content-between gap-3">
        <div className="d-flex align-items-center gap-3">
          <i className="bi bi-shield-check text-accent" style={{ fontSize: '1.8rem' }}></i>
          <p className="mb-0 text-main" style={{ fontSize: '0.9rem', lineHeight: '1.4' }}>
            Platformamızda ən yaxşı istifadəçi təcrübəsini təmin etmək və təhlükəsizlik üçün çərəzlərdən (cookies) istifadə edirik. 
            Ətraflı məlumat üçün <Link to="/privacy" className="link-accent text-decoration-underline">Məxfilik Siyasəti</Link> bölməsinə baxa bilərsiniz.
          </p>
        </div>
        <div className="d-flex gap-2 flex-shrink-0">
          <button 
            type="button" 
            onClick={handleAccept} 
            className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill"
          >
            Razıyam
          </button>
        </div>
      </div>
    </div>
  );
}

export default CookieBanner;
