import React, { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import {
  getNotifications,
  markAsRead,
  markAllAsRead,
  clearNotifications,
  initDefaultNotifications
} from '../utils/notifications';

export default function NotificationBell({ user }) {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState([]);
  const dropdownRef = useRef(null);

  const userId = user?.id || 'anon';

  // Yükləmə və dinləyici
  useEffect(() => {
    if (user?.id) {
      initDefaultNotifications(user);
    }
    const update = () => {
      setNotifications(getNotifications(userId));
    };

    update();
    window.addEventListener('ehisse_notification_change', update);
    return () => window.removeEventListener('ehisse_notification_change', update);
  }, [userId, user]);

  // Dropdown kənara kliklədikdə bağlansın
  useEffect(() => {
    function handleClickOutside(event) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const unreadCount = notifications.filter(n => !n.isRead).length;

  const handleNotificationClick = (item) => {
    if (!item.isRead) {
      markAsRead(userId, item.id);
    }
    setIsOpen(false);
  };

  const formatTime = (isoString) => {
    try {
      const date = new Date(isoString);
      const now = new Date();
      const diffMins = Math.floor((now - date) / (1000 * 60));
      if (diffMins < 1) return 'İndicə';
      if (diffMins < 60) return `${diffMins} dəq əvvəl`;
      const diffHours = Math.floor(diffMins / 60);
      if (diffHours < 24) return `${diffHours} saat əvvəl`;
      return date.toLocaleDateString('az-AZ', { day: '2-digit', month: '2-digit' });
    } catch {
      return '';
    }
  };

  return (
    <div className="position-relative" ref={dropdownRef}>
      {/* Zəng düyməsi */}
      <button
        type="button"
        className="btn-outline position-relative d-flex align-items-center justify-content-center"
        onClick={() => setIsOpen(!isOpen)}
        title="Bildirişlər"
        style={{
          width: '38px',
          height: '38px',
          padding: 0,
          borderRadius: '50%',
          border: '1px solid var(--border-color)',
          background: isOpen ? 'var(--bg-surface-hover)' : 'transparent',
          color: 'var(--text-main)',
          fontSize: '1.1rem'
        }}
      >
        <i className={`bi ${unreadCount > 0 ? 'bi-bell-fill text-accent' : 'bi-bell'}`}></i>
        {unreadCount > 0 && (
          <span
            className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
            style={{
              fontSize: '0.65rem',
              padding: '0.25rem 0.45rem',
              boxShadow: '0 0 8px rgba(220, 53, 69, 0.6)'
            }}
          >
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      {/* Açılan Bildiriş Paneli */}
      {isOpen && (
        <div
          className="shadow-lg position-absolute end-0 mt-2 rounded-3 border"
          style={{
            width: '340px',
            maxWidth: '92vw',
            zIndex: 1050,
            background: 'var(--bg-surface)',
            borderColor: 'var(--border-color)',
            boxShadow: '0 15px 35px rgba(0,0,0,0.5)',
            overflow: 'hidden'
          }}
        >
          {/* Panel Başlığı */}
          <div
            className="p-3 border-bottom d-flex align-items-center justify-content-between"
            style={{ borderColor: 'var(--border-color)' }}
          >
            <div className="d-flex align-items-center gap-2">
              <strong className="text-main" style={{ fontSize: '0.95rem' }}>Bildirişlər</strong>
              {unreadCount > 0 && (
                <span className="badge bg-accent text-dark" style={{ fontSize: '0.7rem', fontWeight: 600 }}>
                  {unreadCount} yeni
                </span>
              )}
            </div>
            {notifications.length > 0 && (
              <div className="d-flex align-items-center gap-2">
                {unreadCount > 0 && (
                  <button
                    type="button"
                    className="btn btn-link p-0 text-accent text-decoration-none small"
                    style={{ fontSize: '0.78rem' }}
                    onClick={() => markAllAsRead(userId)}
                  >
                    Oxunmuş et
                  </button>
                )}
                <button
                  type="button"
                  className="btn btn-link p-0 text-muted text-decoration-none small"
                  style={{ fontSize: '0.78rem' }}
                  onClick={() => clearNotifications(userId)}
                  title="Hamısını sil"
                >
                  <i className="bi bi-trash"></i>
                </button>
              </div>
            )}
          </div>

          {/* Bildirişlər Siyahısı */}
          <div style={{ maxHeight: '360px', overflowY: 'auto' }}>
            {notifications.length === 0 ? (
              <div className="p-4 text-center text-muted">
                <i className="bi bi-bell-slash fs-2 d-block mb-2 opacity-50"></i>
                <small>Heç bir yeni bildiriş yoxdur.</small>
              </div>
            ) : (
              <div className="list-group list-group-flush">
                {notifications.map((item) => {
                  const content = (
                    <div
                      className={`p-3 border-bottom d-flex gap-3 align-items-start transition-all ${
                        !item.isRead ? 'bg-opacity-10' : ''
                      }`}
                      style={{
                        background: !item.isRead ? 'var(--bg-surface-hover)' : 'transparent',
                        borderColor: 'var(--border-color)',
                        cursor: 'pointer'
                      }}
                      onClick={() => handleNotificationClick(item)}
                    >
                      <div
                        style={{
                          width: '32px',
                          height: '32px',
                          borderRadius: '8px',
                          background: item.type === 'success' ? 'rgba(0, 210, 106, 0.15)' :
                                      item.type === 'danger' ? 'rgba(220, 53, 69, 0.15)' :
                                      item.type === 'warning' ? 'rgba(245, 158, 11, 0.15)' : 'rgba(59, 130, 246, 0.15)',
                          color: item.type === 'success' ? '#00d26a' :
                                 item.type === 'danger' ? '#ef4444' :
                                 item.type === 'warning' ? '#f59e0b' : '#3b82f6',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          flexShrink: 0,
                          fontSize: '0.95rem'
                        }}
                      >
                        <i className={`bi ${item.icon || 'bi-bell'}`}></i>
                      </div>
                      <div className="flex-grow-1 min-w-0">
                        <div className="d-flex align-items-center justify-content-between mb-1">
                          <strong
                            className={`small text-truncate ${!item.isRead ? 'text-main fw-bold' : 'text-muted'}`}
                            style={{ fontSize: '0.85rem' }}
                          >
                            {item.title}
                          </strong>
                          <span className="text-muted" style={{ fontSize: '0.7rem' }}>
                            {formatTime(item.createdAt)}
                          </span>
                        </div>
                        <p className="m-0 text-muted small" style={{ fontSize: '0.8rem', lineHeight: '1.35' }}>
                          {item.message}
                        </p>
                      </div>
                      {!item.isRead && (
                        <span
                          style={{
                            width: '8px',
                            height: '8px',
                            borderRadius: '50%',
                            background: 'var(--accent)',
                            display: 'inline-block',
                            marginTop: '6px',
                            flexShrink: 0
                          }}
                        ></span>
                      )}
                    </div>
                  );

                  return item.link ? (
                    <Link
                      key={item.id}
                      to={item.link}
                      className="text-decoration-none"
                    >
                      {content}
                    </Link>
                  ) : (
                    <div key={item.id}>{content}</div>
                  );
                })}
              </div>
            )}
          </div>

          {/* Alt Hissə */}
          <div
            className="p-2 text-center border-top small text-muted"
            style={{ background: 'var(--input-bg)', borderColor: 'var(--border-color)', fontSize: '0.75rem' }}
          >
            e-hisse.az bildiriş mərkəzi
          </div>
        </div>
      )}
    </div>
  );
}
