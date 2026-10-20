import React, { useState, useEffect } from 'react';
import { api as apiFetch, imageSrc } from '../../api';
import { Table, statusBadge, Modal, fmtDate } from '../../components/Panel';
import { Link } from 'react-router-dom';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function AdminAds() {
  const [ads, setAds] = useState([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState('');
  const [search, setSearch] = useState('');
  const [notification, setNotification] = useState('');

  // Deaktivasiya modalı
  const [deactivateModalOpen, setDeactivateModalOpen] = useState(false);
  const [deactivateAdId, setDeactivateAdId] = useState(null);
  const [deactivateReason, setDeactivateReason] = useState('');

  useEffect(() => {
    loadAds();
  }, []);

  async function loadAds() {
    setLoading(true);
    try {
      const data = await apiFetch('/api/Admin/AllAds');
      setAds(data || []);
    } catch (err) {
      console.error(err);
      notify('Elanları yükləyərkən xəta: ' + err.message, 'danger');
    } finally {
      setLoading(false);
    }
  }

  function notify(text, type = 'success') {
    setNotification({ text, type });
    setTimeout(() => setNotification(''), 4000);
  }

  function openDeactivateModal(id) {
    setDeactivateAdId(id);
    setDeactivateReason('');
    setDeactivateModalOpen(true);
  }

  async function handleDeactivateSubmit(e) {
    e.preventDefault();
    if (!deactivateReason.trim()) {
      alertModal('Zəhmət olmasa səbəb daxil edin.', 'Diqqət', 'warning');
      return;
    }
    try {
      await apiFetch(`/api/Admin/DeactivateAd/${deactivateAdId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reason: deactivateReason.trim() })
      });
      notify('Elan məcburi deaktiv edildi.');
      setDeactivateModalOpen(false);
      loadAds();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleRestore(id) {
    if (!await confirmModal({
      title: 'Elanı bərpa et',
      text: 'Bu elanı bərpa edib aktivləşdirmək istədiyinizə əminsiniz?',
      confirmText: 'Bərpa et',
      icon: 'question'
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/RestoreAd/${id}`, { method: 'POST' });
      notify(res.message || 'Elan bərpa edildi.');
      loadAds();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  const filteredAds = ads.filter(a => {
    if (statusFilter === 'active' && (!a.isActive || a.stockQuantity <= 0)) return false;
    if (statusFilter === 'pending' && a.moderationStatus !== 'Pending') return false;
    if (statusFilter === 'outOfStock' && a.stockQuantity > 0) return false;
    if (statusFilter === 'rejected' && a.moderationStatus !== 'Rejected') return false;
    if (statusFilter === 'inactive' && a.isActive) return false;

    if (search) {
      const q = search.toLowerCase();
      const match = (a.title && a.title.toLowerCase().includes(q)) ||
                    (a.sku && a.sku.toLowerCase().includes(q)) ||
                    (a.modelName && a.modelName.toLowerCase().includes(q)) ||
                    (a.sellerName && a.sellerName.toLowerCase().includes(q));
      if (!match) return false;
    }
    return true;
  });

  const columns = [
    { key: 'id', label: 'ID' },
    {
      key: 'image',
      label: 'Şəkil',
      render: a => {
        const mainImg = a.images?.find(i => i.isMain) || a.images?.[0];
        const src = mainImg ? imageSrc(mainImg.imageUrl || mainImg.url) : null;
        return src ? (
          <img src={src} alt="" style={{ width: '42px', height: '42px', objectFit: 'cover', borderRadius: '6px' }} />
        ) : (
          <div style={{ width: '42px', height: '42px', background: 'var(--border-color)', borderRadius: '6px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <i className="bi bi-image text-muted"></i>
          </div>
        );
      }
    },
    {
      key: 'title',
      label: 'Başlıq',
      render: a => (
        <div>
          <Link to={`/details/${a.id}`} target="_blank" className="text-decoration-none fw-semibold" style={{ color: 'var(--accent)' }}>
            {a.title}
          </Link>
          <div className="text-muted small">SKU: {a.sku || '—'}</div>
        </div>
      )
    },
    { key: 'model', label: 'Cihaz', render: a => `${a.brandName || ''} ${a.modelName || ''}`.trim() || '—' },
    { key: 'partTypeName', label: 'Hissə Növü' },
    { key: 'qualityTypeName', label: 'Keyfiyyət', render: a => <span className="badge bg-secondary">{a.qualityTypeName}</span> },
    { key: 'price', label: 'Qiymət', render: a => <strong>{a.price} AZN</strong> },
    {
      key: 'stock',
      label: 'Stok',
      render: a => (
        <span className={`badge ${a.stockQuantity > 0 ? 'bg-success' : 'bg-danger'}`}>
          {a.stockQuantity > 0 ? `${a.stockQuantity} ədəd` : 'Bitib (0)'}
        </span>
      )
    },
    { key: 'sellerName', label: 'Satıcı' },
    {
      key: 'status',
      label: 'Vəziyyət',
      render: a => {
        if (a.moderationStatus === 'Pending') return <span className="badge bg-warning text-dark">Yoxlamada</span>;
        if (a.moderationStatus === 'Rejected') return <span className="badge bg-danger">Rədd edilib</span>;
        if (a.stockQuantity <= 0) return <span className="badge bg-secondary">Stok bitib</span>;
        if (!a.isActive) return <span className="badge bg-dark">Deaktiv</span>;
        return <span className="badge bg-success">Aktiv</span>;
      }
    },
    {
      key: 'actions',
      label: 'Əməliyyat',
      render: a => (
        <div className="d-flex gap-1 flex-wrap">
          {a.isActive && a.stockQuantity > 0 && (
            <button
              onClick={() => openDeactivateModal(a.id)}
              className="btn btn-sm btn-outline-danger py-0 px-2"
              title="Məcburi Deaktiv et"
            >
              <i className="bi bi-pause-circle me-1"></i> Deaktiv et
            </button>
          )}

          {!a.isActive && a.stockQuantity > 0 && a.moderationStatus === 'Approved' && (
            <button
              onClick={() => handleRestore(a.id)}
              className="btn btn-sm btn-outline-success py-0 px-2"
              title="Aktivləşdir"
            >
              <i className="bi bi-play-circle me-1"></i> Bərpa et
            </button>
          )}
        </div>
      )
    }
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h3 className="section-title mb-1">Bütün Elanlar</h3>
          <p className="text-muted m-0">Sistemdəki bütün ehtiyat hissəsi elanlarının reyestri</p>
        </div>
        <button onClick={loadAds} className="btn btn-sm btn-outline">
          <i className="bi bi-arrow-clockwise me-1"></i> Yenilə
        </button>
      </div>

      {notification && (
        <div className={`alert alert-${notification.type} mb-4 d-flex align-items-center justify-content-between`}>
          <span>{notification.text}</span>
          <button type="button" className="btn-close" onClick={() => setNotification('')}></button>
        </div>
      )}

      {/* Filtrlər */}
      <div className="row g-2 mb-3">
        <div className="col-12 col-md-5">
          <input
            type="text"
            className="form-control form-control-sm"
            placeholder="Axtar (Başlıq, SKU, model, satıcı)..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <div className="col-12 col-md-4">
          <select className="form-select form-select-sm" value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
            <option value="">Bütün Statuslar</option>
            <option value="active">Yalnız Aktiv (Dərc olunmuş)</option>
            <option value="pending">Yoxlamada olanlar</option>
            <option value="outOfStock">Stoku bitmişlər (0 ədəd)</option>
            <option value="inactive">Qeyri-aktivlər</option>
            <option value="rejected">Rədd edilmişlər</option>
          </select>
        </div>
        <div className="col-12 col-md-3 text-md-end text-muted small align-self-center">
          Tapılan: {filteredAds.length} elan
        </div>
      </div>

      {loading ? (
        <div className="text-center py-5 text-muted">Yüklənir...</div>
      ) : (
        <Table columns={columns} rows={filteredAds} empty="Axtarışa uyğun elan tapılmadı." />
      )}

      {/* Məcburi Deaktivasiya Modalı */}
      <Modal open={deactivateModalOpen} title="Elanı Məcburi Deaktiv Et" onClose={() => setDeactivateModalOpen(false)}>
        <form onSubmit={handleDeactivateSubmit}>
          <div className="mb-3">
            <label className="form-label text-muted">Deaktivasiya səbəbini daxil edin:</label>
            <textarea
              className="form-control"
              rows="3"
              placeholder="Qayda pozuntusu, şikayət, uyğunsuzluq..."
              value={deactivateReason}
              onChange={e => setDeactivateReason(e.target.value)}
              required
            ></textarea>
          </div>
          <div className="d-flex justify-content-end gap-2">
            <button type="button" className="btn btn-outline-secondary" onClick={() => setDeactivateModalOpen(false)}>
              Ləğv et
            </button>
            <button type="submit" className="btn btn-danger">
              <i className="bi bi-pause-circle me-1"></i> Deaktiv Et
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
