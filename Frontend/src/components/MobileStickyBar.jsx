import React from 'react';
import { NavLink, Link } from 'react-router-dom';

function MobileStickyBar({ user }) {
  return (
    <div className="mobile-sticky-cta d-lg-none">
      {/* 1. Əsas */}
      <NavLink 
        to="/" 
        end 
        className={({ isActive }) => `sticky-cta-item ${isActive ? 'active' : ''}`}
      >
        <i className="bi bi-house-door-fill"></i>
        <span>Əsas</span>
      </NavLink>

      {/* 2. Seçilmişlər */}
      <NavLink 
        to="/#favorites" 
        className={({ isActive }) => `sticky-cta-item ${isActive ? 'active' : ''}`}
        onClick={(e) => {
          // Saytda Seçilmişlər / Favorites bölməsi və ya filtri
        }}
      >
        <i className="bi bi-heart"></i>
        <span>Seçilmişlər</span>
      </NavLink>

      {/* 3. Yeni Elan (Ortada iri narıncı/yaşıl + ikonlu dairə) */}
      {user ? (
        <Link 
          to="/warehouse/add-part" 
          className="sticky-cta-item sticky-cta-add"
          title="Yeni elan"
        >
          <div className="sticky-add-btn">
            <i className="bi bi-plus-lg"></i>
          </div>
          <span>Yeni elan</span>
        </Link>
      ) : (
        <Link 
          to="/login" 
          state={{ from: '/warehouse/add-part' }} 
          className="sticky-cta-item sticky-cta-add"
          title="Yeni elan"
        >
          <div className="sticky-add-btn">
            <i className="bi bi-plus-lg"></i>
          </div>
          <span>Yeni elan</span>
        </Link>
      )}

      {/* 4. Kabinet (3-cü sırada, Əlaqədən əvvəl) */}
      <NavLink 
        to={user ? (user.role === 'Admin' ? '/admin' : '/warehouse') : '/login'} 
        className={({ isActive }) => `sticky-cta-item ${isActive ? 'active' : ''}`}
      >
        <i className="bi bi-person-circle"></i>
        <span>Kabinet</span>
      </NavLink>

      {/* 5. Əlaqə (Mesajlar əvəzinə ən sonda yer alır, əlaqə səhifəsinə yönləndirir) */}
      <NavLink 
        to="/elaqe" 
        className={({ isActive }) => `sticky-cta-item ${isActive ? 'active' : ''}`}
      >
        <i className="bi bi-chat-dots"></i>
        <span>Əlaqə</span>
      </NavLink>
    </div>
  );
}

export default MobileStickyBar;
