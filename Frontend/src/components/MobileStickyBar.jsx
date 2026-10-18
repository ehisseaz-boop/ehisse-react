import React from 'react';
import { Link } from 'react-router-dom';

function MobileStickyBar({ user }) {
  return (
    <div className="mobile-sticky-cta d-lg-none">
      <Link to="/" className="sticky-cta-item">
        <i className="bi bi-search"></i>
        <span>Axtarış</span>
      </Link>

      <a href="tel:+994102523226" className="sticky-cta-item">
        <i className="bi bi-telephone-fill text-accent"></i>
        <span>Zəng et</span>
      </a>

      <a href="https://wa.me/994103961501" target="_blank" rel="noreferrer" className="sticky-cta-item whatsapp-cta">
        <i className="bi bi-whatsapp"></i>
        <span>WhatsApp</span>
      </a>

      {user ? (
        <Link 
          to="/warehouse/add-part" 
          className="sticky-cta-item highlight"
        >
          <i className="bi bi-plus-circle-fill"></i>
          <span>Elan ver</span>
        </Link>
      ) : (
        <Link to="/register" className="sticky-cta-item highlight">
          <i className="bi bi-plus-circle-fill"></i>
          <span>Elan ver</span>
        </Link>
      )}
    </div>
  );
}

export default MobileStickyBar;
