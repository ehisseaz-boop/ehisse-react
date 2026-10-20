import React, { useState, useEffect, useCallback, lazy, Suspense } from 'react';
import { Routes, Route, Link, NavLink, useNavigate, Navigate, useLocation, useNavigationType } from 'react-router-dom';
import './index.css';
import { Api } from './api';
import Home from './pages/Home';

// Lazy loading səhifələr — Ana səhifə dərhal açılır, digər böyük modullar yalnız lazım olduqda yüklənir
const Categories = lazy(() => import('./pages/Categories'));
const Saticilar = lazy(() => import('./pages/Saticilar'));
const Yardim = lazy(() => import('./pages/Yardim'));
const Elaqe = lazy(() => import('./pages/Elaqe'));
const Details = lazy(() => import('./pages/Details'));
const ElanRedirect = lazy(() => import('./pages/ElanRedirect'));
const Login = lazy(() => import('./pages/Login'));
const Register = lazy(() => import('./pages/Register'));
const ForgotPassword = lazy(() => import('./pages/ForgotPassword'));
const Favorites = lazy(() => import('./pages/Favorites'));
const NotFound = lazy(() => import('./pages/NotFound'));

// Admin modulları (Geniş həcmli)
const AdminIndex = lazy(() => import('./pages/admin/AdminIndex'));
const AdminRequests = lazy(() => import('./pages/admin/AdminRequests'));
const AdminAdReview = lazy(() => import('./pages/admin/AdminAdReview'));
const AdminUserReview = lazy(() => import('./pages/admin/AdminUserReview'));
const AdminUsers = lazy(() => import('./pages/admin/AdminUsers'));
const AdminAds = lazy(() => import('./pages/admin/AdminAds'));
const AdminCatalog = lazy(() => import('./pages/admin/AdminCatalog'));
const AdminLogs = lazy(() => import('./pages/admin/AdminLogs'));

// Anbardar / Satıcı modulları
const WarehouseIndex = lazy(() => import('./pages/warehouse/WarehouseIndex'));
const WarehousePartDetails = lazy(() => import('./pages/warehouse/WarehousePartDetails'));
const PartForm = lazy(() => import('./pages/warehouse/PartForm'));
const WarehouseDeleted = lazy(() => import('./pages/warehouse/WarehouseDeleted'));
const SalesHistory = lazy(() => import('./pages/warehouse/WarehouseExtra').then(m => ({ default: m.SalesHistory })));
const Settings = lazy(() => import('./pages/warehouse/WarehouseExtra').then(m => ({ default: m.Settings })));

const Privacy = lazy(() => import('./pages/Privacy'));
const Terms = lazy(() => import('./pages/Terms'));
const ThankYou = lazy(() => import('./pages/ThankYou'));
import CookieBanner from './components/CookieBanner';
import MobileStickyBar from './components/MobileStickyBar';
import NotificationBell from './components/NotificationBell';

import { RequireRole, RequireAuth, PanelLayout } from './components/Panel';

const adminLinks = [
  { to: '/admin', end: true, icon: 'bi-speedometer2', label: 'İdarə paneli' },
  { to: '/admin/requests', icon: 'bi-inbox', label: 'Müraciətlər və Sorğular' },
  { to: '/admin/users', icon: 'bi-people', label: '5. Ümumi istifadəçilər' },
  { to: '/admin/ads', icon: 'bi-megaphone', label: 'Bütün Elanlar' },
  { to: '/admin/catalog', icon: 'bi-diagram-3', label: 'Kataloq' },
  { to: '/admin/logs', icon: 'bi-journal-text', label: 'Jurnal' },
];

const warehouseLinks = [
  { to: '/warehouse', end: true, icon: 'bi-card-list', label: 'Elanlarım' },
  { to: '/warehouse/add-part', icon: 'bi-plus-lg', label: 'Yeni elan' },
  { to: '/warehouse/sales', icon: 'bi-receipt', label: 'Satış tarixçəsi' },
  { to: '/warehouse/deleted', icon: 'bi-trash3', label: 'Silinən elanlar' },
  { to: '/warehouse/settings', icon: 'bi-gear', label: 'Profil və əlaqə' },
];

function App() {
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'light');
  const [user, setUser] = useState(() => {
    try {
      const saved = localStorage.getItem('ehisse_user');
      return saved ? JSON.parse(saved) : undefined;
    } catch {
      return undefined;
    }
  });
  const navigate = useNavigate();
  const location = useLocation();
  const navType = useNavigationType();

  // Bütün səhifələrə keçid zamanı (PUSH və ya REPLACE) ən yuxarı scroll etsin, yalnız "geri" (POP) düyməsi basıldıqda mövqeyi saxlasın
  useEffect(() => {
    if (navType !== 'POP') {
      window.scrollTo({ top: 0, left: 0, behavior: 'instant' });
    }
  }, [location.pathname, navType]);

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  const refreshUser = useCallback(() => {
    return Api.me()
      .then(u => {
        if (u?.isAuthenticated) {
          setUser(u);
          localStorage.setItem('ehisse_user', JSON.stringify(u));
        } else {
          // Əgər server təsdiqləmirsə və ya sessiya yoxdursa, yerli yaddaşı da təmizləyirik
          localStorage.removeItem('ehisse_user');
          setUser(null);
        }
      })
      .catch(() => {
        // Şəbəkə xətası olarsa mövcud saxlanmış istifadəçiyə toxunmuruq
      });
  }, []);

  useEffect(() => { refreshUser(); }, [refreshUser]);

  const toggleTheme = () => setTheme(prev => (prev === 'dark' ? 'light' : 'dark'));

  const logout = async () => {
    try { await Api.logout(); } finally {
      localStorage.removeItem('ehisse_user');
      setUser(null);
      navigate('/');
    }
  };

  const [favoritesCount, setFavoritesCount] = useState(() => {
    try {
      const raw = localStorage.getItem('ehisse_favorites');
      return raw ? JSON.parse(raw).length : 0;
    } catch {
      return 0;
    }
  });

  useEffect(() => {
    const handleFavUpdate = () => {
      try {
        const raw = localStorage.getItem('ehisse_favorites');
        setFavoritesCount(raw ? JSON.parse(raw).length : 0);
      } catch {
        setFavoritesCount(0);
      }
    };
    window.addEventListener('favorites-updated', handleFavUpdate);
    return () => window.removeEventListener('favorites-updated', handleFavUpdate);
  }, []);

  const navClass = ({ isActive }) => `text-decoration-none ${isActive ? 'active' : ''}`;

  return (
    <>
      <header className="top-header d-flex justify-content-between align-items-center w-100 flex-nowrap" style={{ padding: '0.65rem 1.5rem' }}>
        <div className="d-flex align-items-center logo-container flex-nowrap" style={{ gap: '1.5rem' }}>
          <Link to="/" className="logo text-decoration-none text-nowrap" style={{ flexShrink: 0 }}>
            <div className="logo-icon">e</div>
            - hisse<span className="text-accent">.az</span>
          </Link>
          <nav className="nav-links d-none d-lg-flex align-items-center flex-nowrap" style={{ gap: '1.2rem', whiteSpace: 'nowrap' }}>
            <NavLink to="/" end className={navClass}>Kataloq</NavLink>
            <NavLink to="/saticilar" className={navClass}>Satıcılar</NavLink>
            <NavLink to="/yardim" className={navClass}>Yardım mərkəzi</NavLink>
            <NavLink to="/elaqe" className={navClass}>Əlaqə</NavLink>
          </nav>
        </div>

        <div className="header-actions d-flex align-items-center flex-nowrap" style={{ gap: '0.65rem' }}>
          <div className="d-none d-xxl-flex align-items-center me-1 text-nowrap" style={{ gap: '12px', fontSize: '0.88rem', fontWeight: 500 }}>
            <a href="tel:+994102523226" className="text-decoration-none d-inline-flex align-items-center" style={{ color: 'var(--text-main)', whiteSpace: 'nowrap' }}>
              <i className="bi bi-telephone-fill" style={{ color: 'var(--accent)', marginRight: '5px' }}></i> +994 10 252 32 26
            </a>
            <a href="https://wa.me/994103961501" target="_blank" rel="noreferrer" className="text-decoration-none d-inline-flex align-items-center" style={{ color: 'var(--text-main)', whiteSpace: 'nowrap' }}>
              <i className="bi bi-whatsapp" style={{ color: '#25D366', marginRight: '5px' }}></i> +994 10 396 15 01
            </a>
          </div>

          {/* Desktop Sevimlilər Düyməsi və Sayğacı */}
          <Link 
            to="/favorites" 
            className="btn-outline position-relative d-none d-md-inline-flex align-items-center gap-2 text-nowrap"
            title="Seçilmiş elanlar"
            style={{ borderRadius: '20px', padding: '0.45rem 0.85rem', fontSize: '0.9rem', flexShrink: 0 }}
          >
            <i className={`bi bi-heart${favoritesCount > 0 ? '-fill text-danger' : ''}`}></i>
            <span className="d-none d-xl-inline">Seçilmişlər</span>
            {favoritesCount > 0 && (
              <span 
                className="badge rounded-pill bg-danger" 
                style={{ fontSize: '0.72rem', padding: '0.2em 0.55em' }}
              >
                {favoritesCount}
              </span>
            )}
          </Link>

          {/* Bildirişlər Mərkəzi */}
          <NotificationBell user={user} />

          <button 
            onClick={toggleTheme} 
            className="btn-outline d-inline-flex align-items-center gap-2 text-nowrap" 
            id="theme-toggle" 
            style={{ background: 'transparent', border: '1px solid var(--border-color)', color: 'var(--text-main)', borderRadius: '20px', padding: '0.45rem 0.85rem', fontSize: '0.9rem', flexShrink: 0 }}
          >
            <i className={`bi bi-${theme === 'dark' ? 'sun' : 'moon-stars'}`} id="theme-icon"></i>
            <span className="d-none d-xl-inline" id="theme-text">Rejim</span>
          </button>

          {user ? (
            <>
              {user.role === 'Admin' && (
                <Link to="/admin" className="btn-outline" style={{ color: '#8a2be2', borderColor: '#8a2be2' }}>Admin Panel</Link>
              )}
              <Link 
                to={user.role === 'Admin' ? '/admin' : '/warehouse'} 
                className="user-profile text-decoration-none d-inline-flex align-items-center gap-2 text-nowrap"
                title="Şəxsi kabinet"
                style={{ flexShrink: 0 }}
              >
                <div className="user-avatar" style={{
                  width: '36px',
                  height: '36px',
                  borderRadius: '50%',
                  backgroundColor: 'var(--accent)',
                  color: 'var(--accent-text)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  fontWeight: 'bold',
                  fontSize: '0.95rem',
                  boxShadow: '0 2px 5px rgba(0,0,0,0.2)',
                  border: '2px solid rgba(255,255,255,0.2)',
                  overflow: 'hidden',
                  flexShrink: 0
                }}>
                  {user.shopPhotoUrl ? (
                    <img
                      src={user.shopPhotoUrl}
                      alt="Profil"
                      style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                    />
                  ) : (
                    (user.fullName || user.email || user.username || 'U')[0].toUpperCase()
                  )}
                </div>
                <div className="d-none d-lg-flex flex-column text-start" style={{ lineHeight: '1.2' }}>
                  <span className="user-name" style={{ color: 'var(--text-main)', fontWeight: 600, fontSize: '0.85rem' }}>
                    {user.fullName || user.username}
                  </span>
                  <span className="user-role" style={{ color: 'var(--text-muted)', fontSize: '0.72rem' }}>
                    {user.role === 'Admin' ? 'Administrator' : user.role === 'Anbardar' ? 'Satıcı' : 'Usta / Alıcı'}
                  </span>
                </div>
              </Link>
              <button onClick={logout} className="btn-outline d-inline-flex align-items-center gap-1 text-nowrap" title="Çıxış" style={{ borderRadius: '20px', padding: '0.45rem 0.8rem', fontSize: '0.9rem', flexShrink: 0 }}>
                <i className="bi bi-box-arrow-right"></i>
                <span className="d-none d-xl-inline">Çıxış</span>
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="btn-outline text-decoration-none">Daxil ol</Link>
              <Link to="/register" className="btn-outline text-decoration-none d-none d-md-flex" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)', borderColor: 'var(--accent)' }}>Qeydiyyat</Link>
            </>
          )}
        </div>
      </header>

      <main role="main">
        <Suspense fallback={
          <div className="d-flex justify-content-center align-items-center py-5" style={{ minHeight: '50vh' }}>
            <div className="spinner-border text-accent" role="status">
              <span className="visually-hidden">Yüklənir...</span>
            </div>
          </div>
        }>
          <Routes>
            <Route path="/" element={<RequireAuth user={user}><Home user={user} /></RequireAuth>} />
            <Route path="/categories" element={<RequireAuth user={user}><Categories /></RequireAuth>} />
            <Route path="/saticilar" element={<RequireAuth user={user}><Saticilar user={user} /></RequireAuth>} />
            <Route path="/yardim" element={<Yardim />} />
            <Route path="/elaqe" element={<Elaqe />} />
            <Route path="/favorites" element={<RequireAuth user={user}><Favorites /></RequireAuth>} />
            <Route path="/details/:id" element={<RequireAuth user={user}><Details user={user} /></RequireAuth>} />
            <Route path="/elan/:id" element={<RequireAuth user={user}><ElanRedirect user={user} /></RequireAuth>} />
            <Route path="/elan-yerlesdir" element={<Navigate to="/warehouse/add-part" replace />} />
            <Route path="/elan-elave-et" element={<Navigate to="/warehouse/add-part" replace />} />
            <Route path="/yeni-elan" element={<Navigate to="/warehouse/add-part" replace />} />
            <Route path="/add-part" element={<Navigate to="/warehouse/add-part" replace />} />
            <Route path="/login" element={<Login onLogin={refreshUser} />} />
            <Route path="/register" element={<Register onRegister={refreshUser} />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/privacy" element={<Privacy />} />
            <Route path="/terms" element={<Terms />} />
            <Route path="/thank-you" element={<ThankYou />} />

            <Route path="/warehouse" element={<RequireRole roles={['Anbardar', 'Usta', 'Admin']} user={user}><PanelLayout title="Satıcı kabineti" links={warehouseLinks} /></RequireRole>}>
              <Route index element={<WarehouseIndex />} />
              <Route path="part/:id" element={<WarehousePartDetails />} />
              <Route path="ads/:id" element={<WarehousePartDetails />} />
              <Route path="add-part" element={<PartForm />} />
              <Route path="add" element={<PartForm />} />
              <Route path="edit-part/:id" element={<PartForm />} />
              <Route path="edit/:id" element={<PartForm />} />
              <Route path="sales" element={<SalesHistory />} />
              <Route path="deleted" element={<WarehouseDeleted />} />
              <Route path="settings" element={<Settings />} />
            </Route>

            <Route path="/admin" element={<RequireRole roles={['Admin']} user={user}><PanelLayout title="Admin Panel" links={adminLinks} /></RequireRole>}>
              <Route index element={<AdminIndex />} />
              <Route path="requests" element={<AdminRequests />} />
              <Route path="requests/ad/:id" element={<AdminAdReview />} />
              <Route path="requests/user/:id" element={<AdminUserReview />} />
              <Route path="users" element={<AdminUsers />} />
              <Route path="ads" element={<AdminAds />} />
              <Route path="catalog" element={<AdminCatalog />} />
              <Route path="logs" element={<AdminLogs />} />
            </Route>

            <Route path="*" element={<NotFound />} />
          </Routes>
        </Suspense>
      </main>

      <footer className="mt-5 border-top" style={{ backgroundColor: 'var(--bg-surface)', borderColor: 'var(--border-color)' }}>
        <div className="container py-4">
          <div className="row g-4 justify-content-between">
            {/* 1. Brend və Haqqında */}
            <div className="col-12 col-md-4">
              <div className="d-flex align-items-center gap-2 mb-2">
                <div className="logo-icon">e</div>
                <strong style={{ color: 'var(--text-main)', fontSize: '1.2rem' }}>- hisse<span className="text-accent">.az</span></strong>
              </div>
              <p className="text-muted small mb-3">
                Mobil telefon, planşet və noutbuk ehtiyat hissələrinin ən böyük axtarış və sifariş platforması. Ustalar və anbarlar üçün etibarlı həll.
              </p>
              <div className="footer-status d-inline-block">
                <i className="bi bi-shield-check text-accent me-1"></i> Bütün sistemlər aktivdir
              </div>
            </div>

            {/* 2. Sürətli Keçidlər */}
            <div className="col-6 col-md-2">
              <h6 className="text-main fw-bold mb-3">Keçidlər</h6>
              <ul className="list-unstyled small d-flex flex-column gap-2 mb-0">
                <li><Link to="/" className="text-muted text-decoration-none hover-accent">Kataloq</Link></li>
                <li><Link to="/categories" className="text-muted text-decoration-none hover-accent">Kateqoriyalar</Link></li>
                <li><Link to="/saticilar" className="text-muted text-decoration-none hover-accent">Satıcılar</Link></li>
                <li><Link to="/yardim" className="text-muted text-decoration-none hover-accent">Yardım və FAQ</Link></li>
              </ul>
            </div>

            {/* 3. Hüquqi və Qaydalar */}
            <div className="col-6 col-md-3">
              <h6 className="text-main fw-bold mb-3">Hüquqi</h6>
              <ul className="list-unstyled small d-flex flex-column gap-2 mb-0">
                <li><Link to="/privacy" className="text-muted text-decoration-none hover-accent">Gizlilik Politikası (Məxfilik)</Link></li>
                <li><Link to="/terms" className="text-muted text-decoration-none hover-accent">Kullanım Koşulları (İstifadə)</Link></li>
                <li><Link to="/elaqe" className="text-muted text-decoration-none hover-accent">Bizə yazın</Link></li>
              </ul>
            </div>

            {/* 4. Əlaqə məlumatları */}
            <div className="col-12 col-md-3">
              <h6 className="text-main fw-bold mb-3">Əlaqə Məlumatları</h6>
              <div className="small text-muted d-flex flex-column gap-2">
                <div>
                  <i className="bi bi-telephone-fill text-accent me-2"></i>
                  <a href="tel:+994102523226" className="text-decoration-none text-main">+994 10 252 32 26</a>
                </div>
                <div>
                  <i className="bi bi-whatsapp text-success me-2"></i>
                  <a href="https://wa.me/994103961501" target="_blank" rel="noreferrer" className="text-decoration-none text-main">+994 10 396 15 01</a>
                </div>
                <div>
                  <i className="bi bi-envelope-fill text-accent me-2"></i>
                  <a href="mailto:info@e-hisse.az" className="text-decoration-none text-main">info@e-hisse.az</a>
                </div>
                <div>
                  <i className="bi bi-geo-alt-fill text-accent me-2"></i>
                  <span>Bakı şəh., 28 May / Nizami küç.</span>
                </div>
              </div>
            </div>
          </div>

          <div className="border-top pt-3 mt-4 d-flex flex-column flex-md-row justify-content-between align-items-center text-muted small" style={{ borderColor: 'var(--border-color)' }}>
            <div>© {new Date().getFullYear()} e-hisse.az. Bütün hüquqlar qorunur.</div>
            <div className="mt-2 mt-md-0">Doğru hissəni tez tap.</div>
          </div>
        </div>
      </footer>

      {/* Sabit Mobil Alt Bar (Sticky Mobile CTA) */}
      <MobileStickyBar user={user} />

      {/* Çərəz Bildirişi (Cookie Banner) */}
      <CookieBanner />
    </>
  );
}

export default App;
