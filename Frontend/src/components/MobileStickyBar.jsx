import React, { useState, useEffect } from 'react';
import { NavLink, Link } from 'react-router-dom';

function MobileStickyBar({ user }) {
  const [favCount, setFavCount] = useState(() => {
    try {
      const raw = localStorage.getItem('ehisse_favorites');
      return raw ? JSON.parse(raw).length : 0;
    } catch {
      return 0;
    }
  });

  useEffect(() => {
    const updateCount = () => {
      try {
        const raw = localStorage.getItem('ehisse_favorites');
        setFavCount(raw ? JSON.parse(raw).length : 0);
      } catch {
        setFavCount(0);
      }
    };
    window.addEventListener('favorites-updated', updateCount);
    return () => window.removeEventListener('favorites-updated', updateCount);
  }, []);

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

      {/* 2. Seçilmişlər (Sayğacla) */}
      <NavLink 
        to="/favorites" 
        className={({ isActive }) => `sticky-cta-item position-relative ${isActive ? 'active' : ''}`}
      >
        <div className="position-relative d-inline-block">
          <i className={`bi bi-heart${favCount > 0 ? '-fill text-danger' : ''}`}></i>
          {favCount > 0 && (
            <span 
              className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
              style={{ fontSize: '0.62rem', padding: '0.2em 0.45em' }}
            >
              {favCount}
            </span>
          )}
        </div>
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
