import React, { useEffect } from 'react';
import { Navigate, NavLink, Outlet, useLocation } from 'react-router-dom';
import { Loading } from '../pages/Categories';

// Giriş tələb edən səhifələr üçün qoruma
export function RequireAuth({ user, children }) {
  const location = useLocation();
  if (user === undefined) return <div className="container"><Loading /></div>;
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname + location.search }} />;
  return children;
}

// Rol əsaslı qoruma: user === undefined → hələ yüklənir, null → daxil olmayıb
export function RequireRole({ user, roles, children }) {
  const location = useLocation();
  if (user === undefined) return <div className="container"><Loading /></div>;
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname + location.search }} />;
  if (roles && !roles.includes(user.role)) {
    return <div className="container text-center py-5"><h3>Bu bölməyə giriş icazəniz yoxdur.</h3></div>;
  }
  return children;
}

export function PanelLayout({ title, links }) {
  return (
    <div className="container">
      <section className="panel-section">
        <h2 className="section-title mb-3">{title}</h2>
        <div className="panel-nav mb-4">
          {links.map(l => (
            <NavLink key={l.to} to={l.to} end={l.end}
              className={({ isActive }) => `panel-nav-link ${isActive ? 'active' : ''}`}>
              <i className={`bi ${l.icon}`}></i> {l.label}
            </NavLink>
          ))}
        </div>
        <Outlet />
      </section>
    </div>
  );
}

// Moderasiya (Pending/Approved/Rejected) və elanın vahid vəziyyəti (Active/OutOfStock/...) üçün nişan
const STATUS_MAP = {
  Approved: ['Təsdiqlənib', 'success'],
  Pending: ['Yoxlamadadır', 'warning'],
  Rejected: ['Rədd edilib', 'danger'],
  Active: ['Aktiv', 'success'],
  OutOfStock: ['Stok bitib', 'secondary'],
  Deactivated: ['Deaktiv edilib', 'secondary'],
  Deleted: ['Silinib', 'dark'],
  Blocked: ['Bloklanıb', 'danger'],
};

export const statusBadge = (s, textOverride) => {
  const [text, tone] = STATUS_MAP[s] || [s, 'secondary'];
  return <span className={`status-pill status-${tone}`}>{textOverride || text}</span>;
};

export const fmtDate = (d) => (d ? new Date(d).toLocaleString('az-AZ', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '');

export function Table({ columns, rows, empty = 'Məlumat yoxdur', onRowClick }) {
  if (!rows || !rows.length) return <div className="empty-state"><i className="bi bi-inbox"></i><p>{empty}</p></div>;
  return (
    <div className="panel-table-wrap">
      <table className="panel-table">
        <thead>
          <tr>{columns.map(c => <th key={c.key}>{c.label}</th>)}</tr>
        </thead>
        <tbody>
          {rows.map((r, i) => (
            <tr key={r.id ?? i} onClick={onRowClick ? () => onRowClick(r) : undefined} className={onRowClick ? 'clickable' : ''}>
              {columns.map(c => <td key={c.key}>{c.render ? c.render(r) : r[c.key]}</td>)}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export function Modal({ open, title, children, footer, onClose }) {
  useEffect(() => {
    if (!open) return;
    document.body.style.overflow = 'hidden';
    const onKey = (e) => {
      if (e.key === 'Escape') {
        if (document.querySelector('.swal2-container')) return;
        onClose?.();
      }
    };
    window.addEventListener('keydown', onKey);
    return () => {
      document.body.style.overflow = 'unset';
      window.removeEventListener('keydown', onKey);
    };
  }, [open, onClose]);

  if (!open) return null;
  return (
    <div className="app-modal-backdrop" onMouseDown={(e) => { if (e.target === e.currentTarget) onClose?.(); }}>
      <div className="app-modal" role="dialog" aria-modal="true">
        <div className="app-modal-header">
          <h5 className="m-0">{title}</h5>
          <button type="button" className="app-modal-close" onClick={onClose} aria-label="Bağla"><i className="bi bi-x-lg"></i></button>
        </div>
        <div className="app-modal-body">{children}</div>
        {footer && <div className="app-modal-footer">{footer}</div>}
      </div>
    </div>
  );
}

export function StatCard({ icon, label, value, tone = 'accent', to }) {
  const inner = (
    <div className={`stat-card stat-${tone}`}>
      <div className="stat-icon"><i className={`bi ${icon}`}></i></div>
      <div>
        <div className="stat-value">{value ?? '—'}</div>
        <div className="stat-label">{label}</div>
      </div>
    </div>
  );
  return to ? <NavLink to={to} className="text-decoration-none">{inner}</NavLink> : inner;
}

export function Tabs({ tabs, active, onChange }) {
  return (
    <div className="panel-tabs">
      {tabs.map(t => (
        <button key={t.key} type="button" className={`panel-tab ${active === t.key ? 'active' : ''}`} onClick={() => onChange(t.key)}>
          {t.label}{typeof t.count === 'number' && <span className="panel-tab-count">{t.count}</span>}
        </button>
      ))}
    </div>
  );
}
