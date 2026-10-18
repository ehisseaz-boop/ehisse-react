import React, { useState, useEffect, useCallback } from 'react';
import { Routes, Route, Link, NavLink, useNavigate, Navigate } from 'react-router-dom';
import './index.css';
import { Api } from './api';
import Home from './pages/Home';
import Categories from './pages/Categories';
import Saticilar from './pages/Saticilar';
import Yardim from './pages/Yardim';
import Elaqe from './pages/Elaqe';
import Details from './pages/Details';
import ElanRedirect from './pages/ElanRedirect';
import Login from './pages/Login';
import Register from './pages/Register';
import ForgotPassword from './pages/ForgotPassword';
import NotFound from './pages/NotFound';

import AdminIndex from './pages/admin/AdminIndex';
import AdminRequests from './pages/admin/AdminRequests';
import AdminAdReview from './pages/admin/AdminAdReview';
import AdminUserReview from './pages/admin/AdminUserReview';
import AdminUsers from './pages/admin/AdminUsers';
import AdminAds from './pages/admin/AdminAds';
import AdminCatalog from './pages/admin/AdminCatalog';
import AdminLogs from './pages/admin/AdminLogs';

import WarehouseIndex from './pages/warehouse/WarehouseIndex';
import WarehousePartDetails from './pages/warehouse/WarehousePartDetails';
import PartForm from './pages/warehouse/PartForm';
import { SalesHistory, Settings } from './pages/warehouse/WarehouseExtra';

import WarehouseDeleted from './pages/warehouse/WarehouseDeleted';

import Privacy from './pages/Privacy';
import Terms from './pages/Terms';
import ThankYou from './pages/ThankYou';
import CookieBanner from './components/CookieBanner';
import MobileStickyBar from './components/MobileStickyBar';

import { RequireRole, PanelLayout } from './components/Panel';

const adminLinks = [
  { to: '/admin', end: true, icon: 'bi-speedometer2', label: 'İdarə paneli' },
  { to: '/admin/requests', icon: 'bi-inbox', label: 'Müraciətlər' },
  { to: '/admin/users', icon: 'bi-people', label: 'İstifadəçilər' },
  { to: '/admin/ads', icon: 'bi-megaphone', label: 'Elanlar' },
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
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'dark');
  // undefined → hələ yüklənir, null → daxil olmayıb
  const [user, setUser] = useState(undefined);
  const navigate = useNavigate();

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  const refreshUser = useCallback(() => {
    return Api.me().then(u => setUser(u?.isAuthenticated ? u : null)).catch(() => setUser(null));
  }, []);

  useEffect(() => { refreshUser(); }, [refreshUser]);

  const toggleTheme = () => setTheme(prev => (prev === 'dark' ? 'light' : 'dark'));

  const logout = async () => {
    try { await Api.logout(); } finally { setUser(null); navigate('/'); }
  };

  const navClass = ({ isActive }) => `text-decoration-none ${isActive ? 'active' : ''}`;

  return (
    <>
      <header className="top-header d-flex justify-content-between align-items-center w-100">
        <div className="d-flex align-items-center logo-container">
          <Link to="/" className="logo text-decoration-none">
            <div className="logo-icon">e</div>
            - hisse<span className="text-accent">.az</span>
          </Link>
          <nav className="nav-links d-none d-lg-flex">
            <NavLink to="/" end className={navClass}>Kataloq</NavLink>
            <NavLink to="/saticilar" className={navClass}>Satıcılar</NavLink>
            <NavLink to="/yardim" className={navClass}>Yardım mərkəzi</NavLink>
            <NavLink to="/elaqe" className={navClass}>Əlaqə</NavLink>
          </nav>
        </div>

        <div className="header-actions d-flex align-items-center" style={{ gap: '1rem' }}>
          <div className="d-none d-xl-flex align-items-center me-2" style={{ gap: '15px', fontSize: '0.95rem', fontWeight: 500 }}>
            <a href="tel:+994102523226" className="text-decoration-none" style={{ color: 'var(--text-main)' }}>
              <i className="bi bi-telephone-fill" style={{ color: 'var(--accent)', marginRight: '4px' }}></i> +994 10 252 32 26
            </a>
            <a href="https://wa.me/994103961501" target="_blank" rel="noreferrer" className="text-decoration-none" style={{ color: 'var(--text-main)' }}>
              <i className="bi bi-whatsapp" style={{ color: '#25D366', marginRight: '4px' }}></i> +994 10 396 15 01
            </a>
          </div>

          <button onClick={toggleTheme} className="btn-outline d-flex align-items-center gap-2" id="theme-toggle" style={{ background: 'transparent', border: '1px solid var(--border-color)', color: 'var(--text-main)', borderRadius: '20px' }}>
            <i className={`bi bi-${theme === 'dark' ? 'sun' : 'moon-stars'}`} id="theme-icon"></i>
            <span className="d-none d-md-inline" id="theme-text">Rejim</span>
          </button>

          {user ? (
            <>
              {user.role === 'Admin' && (
                <Link to="/admin" className="btn-outline" style={{ color: '#8a2be2', borderColor: '#8a2be2' }}>Admin Panel</Link>
              )}
              <Link 
                to={user.role === 'Admin' ? '/admin' : '/warehouse'} 
                className="user-profile text-decoration-none d-flex align-items-center gap-2"
                title="Şəxsi kabinet"
              >
                <div className="user-avatar" style={{
                  width: '38px',
                  height: '38px',
                  borderRadius: '50%',
                  backgroundColor: 'var(--accent)',
                  color: 'var(--accent-text)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  fontWeight: 'bold',
                  fontSize: '1rem',
                  boxShadow: '0 2px 5px rgba(0,0,0,0.2)',
                  border: '2px solid rgba(255,255,255,0.2)'
                }}>
                  {(user.fullName || user.email || user.username || 'U')[0].toUpperCase()}
                </div>
                <div className="d-none d-md-flex flex-column text-start" style={{ lineHeight: '1.2' }}>
                  <span className="user-name" style={{ color: 'var(--text-main)', fontWeight: 600, fontSize: '0.9rem' }}>
                    {user.fullName || user.username}
                  </span>
                  <span className="user-role" style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>
                    {user.role === 'Admin' ? 'Administrator' : user.role === 'Anbardar' ? 'Satıcı / Anbardar' : 'Usta / Alıcı'}
                  </span>
                </div>
              </Link>
              <button onClick={logout} className="btn-outline" title="Çıxış">
                <i className="bi bi-box-arrow-right"></i>
                <span className="d-none d-md-inline">Çıxış</span>
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
        <Routes>
          <Route path="/" element={<Home user={user} />} />
          <Route path="/categories" element={<Categories />} />
          <Route path="/saticilar" element={<Saticilar />} />
          <Route path="/yardim" element={<Yardim />} />
          <Route path="/elaqe" element={<Elaqe />} />
          <Route path="/details/:id" element={<Details user={user} />} />
          <Route path="/elan/:id" element={<ElanRedirect user={user} />} />
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

          <Route path="/warehouse" element={<RequireRole roles={['Anbardar', 'Usta']} user={user}><PanelLayout title="Satıcı kabineti" links={warehouseLinks} /></RequireRole>}>
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
