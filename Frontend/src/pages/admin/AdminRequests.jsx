import React, { useState, useEffect } from 'react';
import { api as apiFetch, imageSrc } from '../../api';
import { Table, Tabs, Modal, fmtDate } from '../../components/Panel';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function AdminRequests() {
  const [activeTab, setActiveTab] = useState('registration');
  const [loading, setLoading] = useState(true);

  const [pendingUsers, setPendingUsers] = useState([]);
  const [newAds, setNewAds] = useState([]);
  const [updateAds, setUpdateAds] = useState([]);

  // Modals & Actions
  const [selectedAd, setSelectedAd] = useState(null);
  const [rejectModalOpen, setRejectModalOpen] = useState(false);
  const [rejectType, setRejectType] = useState('ad'); // 'ad' | 'user'
  const [rejectTargetId, setRejectTargetId] = useState(null);
  const [rejectReason, setRejectReason] = useState('');
  const [notification, setNotification] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  useEffect(() => {
    loadRequests();
  }, []);

  async function loadRequests() {
    setLoading(true);
    try {
      const data = await apiFetch('/api/Admin/Requests');
      setPendingUsers(data.pendingUsers || []);
      setNewAds(data.newAdRequests || []);
      setUpdateAds(data.updateAdRequests || []);
    } catch (err) {
      console.error(err);
      showNotification('Məlumatları yükləyərkən xəta baş verdi: ' + err.message, 'danger');
    } finally {
      setLoading(false);
    }
  }

  function showNotification(msg, type = 'success') {
    setNotification({ text: msg, type });
    setTimeout(() => setNotification(''), 5000);
  }

  // ============================================
  // İSTİFADƏÇİ TƏSDİQ / İMTİNA
  // ============================================
  async function handleApproveUser(id) {
    if (!await confirmModal({
      title: 'İstifadəçini təsdiqlə',
      text: 'Bu istifadəçinin qeydiyyatını təsdiqləmək istədiyinizə əminsiniz?',
      confirmText: 'Təsdiqlə',
      icon: 'question'
    })) return;
    setActionLoading(true);
    try {
      const res = await apiFetch(`/api/Admin/ApproveUser/${id}`, { method: 'POST' });
      showNotification(res.message || 'İstifadəçi uğurla təsdiqləndi.');
      loadRequests();
    } catch (err) {
      showNotification('Xəta: ' + err.message, 'danger');
    } finally {
      setActionLoading(false);
    }
  }

  function openRejectUserModal(id) {
    setRejectType('user');
    setRejectTargetId(id);
    setRejectReason('');
    setRejectModalOpen(true);
  }

  // ============================================
  // ELAN TƏSDİQ / İMTİNA / BƏRPA
  // ============================================
  async function handleApproveAd(id) {
    if (!await confirmModal({
      title: 'Elanı təsdiqlə',
      text: 'Bu elanı təsdiq edib ictimai səhifəyə buraxmaq istədiyinizə əminsiniz?',
      confirmText: 'Təsdiq et',
      icon: 'question'
    })) return;
    setActionLoading(true);
    try {
      const res = await apiFetch(`/api/Admin/ApproveAd/${id}`, { method: 'POST' });
      showNotification(res.message || 'Elan təsdiqləndi.');
      setSelectedAd(null);
      loadRequests();
    } catch (err) {
      showNotification('Xəta: ' + err.message, 'danger');
    } finally {
      setActionLoading(false);
    }
  }

  async function handleRestoreAd(id) {
    if (!await confirmModal({
      title: 'Elanı bərpa et',
      text: 'Stoku yenilənmiş elanı bərpa edib aktivləşdirmək istədiyinizə əminsiniz?',
      confirmText: 'Bərpa et',
      icon: 'question'
    })) return;
    setActionLoading(true);
    try {
      const res = await apiFetch(`/api/Admin/RestoreAd/${id}`, { method: 'POST' });
      showNotification(res.message || 'Elan bərpa edildi və aktivləşdirildi.');
      setSelectedAd(null);
      loadRequests();
    } catch (err) {
      showNotification('Xəta: ' + err.message, 'danger');
    } finally {
      setActionLoading(false);
    }
  }

  function openRejectAdModal(id) {
    setRejectType('ad');
    setRejectTargetId(id);
    setRejectReason('');
    setRejectModalOpen(true);
  }

  async function handleRejectSubmit(e) {
    e.preventDefault();
    if (!rejectReason.trim()) {
      alertModal('Zəhmət olmasa imtina səbəbini qeyd edin.', 'Diqqət', 'warning');
      return;
    }
    setActionLoading(true);
    try {
      if (rejectType === 'user') {
        const res = await apiFetch(`/api/Admin/RejectUser/${rejectTargetId}`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ reason: rejectReason })
        });
        showNotification(res.message || 'İstifadəçi müraciəti rədd edildi.');
      } else {
        const res = await apiFetch(`/api/Admin/RejectAd/${rejectTargetId}`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ reason: rejectReason })
        });
        showNotification(res.message || 'Elan rədd edildi.');
      }
      setRejectModalOpen(false);
      setSelectedAd(null);
      loadRequests();
    } catch (err) {
      showNotification('Xəta: ' + err.message, 'danger');
    } finally {
      setActionLoading(false);
    }
  }

  const [previewIdCard, setPreviewIdCard] = useState(null);

  // Cədvəl sütunları
  const userColumns = [
    { key: 'id', label: 'ID' },
    { key: 'fullName', label: 'Ad Soyad' },
    { key: 'pin', label: 'FİN Kod', render: (u) => <strong>{u.pin || '—'}</strong> },
    { key: 'phone', label: 'Telefon' },
    { key: 'email', label: 'Email', render: (u) => u.email || '—' },
    {
      key: 'idCardImageUrl',
      label: 'Vəsiqə Şəkli',
      render: (u) => u.idCardImageUrl ? (
        <button
          onClick={() => setPreviewIdCard({ url: u.idCardImageUrl, name: u.fullName, pin: u.pin })}
          className="btn btn-sm btn-outline-info py-0 px-2"
          title="Vəsiqə şəklinə bax"
        >
          <i className="bi bi-card-image me-1"></i> Vəsiqə
        </button>
      ) : (
        <span className="text-muted small">Yoxdur</span>
      )
    },
    { key: 'role', label: 'Rol', render: (u) => <span className="badge bg-secondary">{u.role === 'Anbardar' ? 'Satıcı' : u.role}</span> },
    { key: 'createdDate', label: 'Qeydiyyat Tarixi', render: (u) => fmtDate(u.createdDate) },
    {
      key: 'actions',
      label: 'Əməliyyatlar',
      render: (u) => (
        <div className="d-flex gap-2">
          <button
            onClick={() => handleApproveUser(u.id)}
            disabled={actionLoading}
            className="btn btn-sm"
            style={{ backgroundColor: '#28a745', color: '#fff' }}
          >
            <i className="bi bi-check-lg me-1"></i> Təsdiq et
          </button>
          <button
            onClick={() => openRejectUserModal(u.id)}
            disabled={actionLoading}
            className="btn btn-sm btn-outline-danger"
          >
            <i className="bi bi-x-lg me-1"></i> İmtina et
          </button>
        </div>
      )
    }
  ];

  const adColumns = [
    { key: 'id', label: 'ID' },
    {
      key: 'image',
      label: 'Şəkil',
      render: (a) => {
        const mainImg = a.images?.find(i => i.isMain) || a.images?.[0];
        const src = mainImg ? imageSrc(mainImg.imageUrl || mainImg.url) : null;
        return src ? (
          <img src={src} alt="" style={{ width: '48px', height: '48px', objectFit: 'cover', borderRadius: '6px' }} />
        ) : (
          <div style={{ width: '48px', height: '48px', background: 'var(--border-color)', borderRadius: '6px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <i className="bi bi-image text-muted"></i>
          </div>
        );
      }
    },
    { key: 'title', label: 'Başlıq' },
    { key: 'model', label: 'Cihaz', render: (a) => `${a.brandName || ''} ${a.modelName || ''}`.trim() || '—' },
    { key: 'partTypeName', label: 'Hissə Növü' },
    { key: 'qualityTypeName', label: 'Keyfiyyət', render: (a) => <span className="badge bg-info text-dark">{a.qualityTypeName}</span> },
    { key: 'price', label: 'Qiymət', render: (a) => <strong>{a.price} AZN</strong> },
    { key: 'stockQuantity', label: 'Stok', render: (a) => <span className={`badge ${a.stockQuantity > 0 ? 'bg-success' : 'bg-danger'}`}>{a.stockQuantity} ədəd</span> },
    { key: 'sellerName', label: 'Satıcı' },
    {
      key: 'actions',
      label: 'Əməliyyatlar',
      render: (a) => (
        <button
          onClick={() => setSelectedAd(a)}
          className="btn btn-sm btn-outline"
          style={{ borderColor: 'var(--accent)', color: 'var(--accent)' }}
        >
          <i className="bi bi-eye me-1"></i> Detallara bax
        </button>
      )
    }
  ];

  // Restock Ads vs Regular Update Ads:
  // Hər iki tabda gözləyən redaktə və stok bərpa sorğuları aydın şəkildə göstərilir
  const restockAds = updateAds.filter(a => a.stockQuantity > 0 && !a.isActive);
  const regularUpdateAds = updateAds;

  const tabsConfig = [
    { key: 'registration', label: '1. Yeni Qeydiyyat Sorğuları', count: pendingUsers.length },
    { key: 'newAds', label: '2. Yeni Elan Sorğuları', count: newAds.length },
    { key: 'updateAds', label: '3. Elan Redaktə Sorğuları', count: regularUpdateAds.length },
    { key: 'userEditAds', label: '4. İstifadəçi Redaktə Sorğuları', count: 0 },
    { key: 'restockAds', label: 'Stok Bərpası Sorğuları', count: restockAds.length },
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <h3 className="section-title mb-0">Moderasiya və Gözləyən Sorğular</h3>
        <button onClick={loadRequests} className="btn btn-sm btn-outline">
          <i className="bi bi-arrow-clockwise me-1"></i> Yenilə
        </button>
      </div>

      {notification && (
        <div className={`alert alert-${notification.type} mb-4 d-flex align-items-center justify-content-between`}>
          <span>{notification.text}</span>
          <button type="button" className="btn-close" onClick={() => setNotification('')}></button>
        </div>
      )}

      <Tabs tabs={tabsConfig} active={activeTab} onChange={setActiveTab} />

      <div className="mt-4">
        {loading ? (
          <div className="text-center py-5 text-muted">Məlumatlar yüklənir...</div>
        ) : (
          <>
            {activeTab === 'registration' && (
              <Table columns={userColumns} rows={pendingUsers} empty="Gözləyən qeydiyyat müraciəti yoxdur." />
            )}

            {activeTab === 'newAds' && (
              <Table columns={adColumns} rows={newAds} empty="Təsdiq gözləyən yeni elan yoxdur." />
            )}

            {activeTab === 'updateAds' && (
              <Table columns={adColumns} rows={regularUpdateAds} empty="Redaktə yoxlamasında olan elan yoxdur." />
            )}

            {activeTab === 'userEditAds' && (
              <div className="card p-4 text-center text-muted" style={{ background: 'var(--surface-color)', border: '1px dashed var(--border-color)', borderRadius: '10px' }}>
                <i className="bi bi-person-check fs-2 mb-2 text-accent"></i>
                <h6 className="fw-bold mb-1">İstifadəçi Redaktə Sorğuları</h6>
                <p className="small mb-0">Hazırda təsdiq gözləyən istifadəçi profil redaktə sorğusu yoxdur.</p>
              </div>
            )}

            {activeTab === 'restockAds' && (
              <Table columns={adColumns} rows={restockAds} empty="Stok bərpası gözləyən elan yoxdur." />
            )}
          </>
        )}
      </div>

      {/* Elan Detalları Modalı */}
      <Modal open={!!selectedAd && !rejectModalOpen} title="Elan Təfərrüatları və Moderasiya" onClose={() => setSelectedAd(null)}>
        {selectedAd && (
          <div>
            <div className="row g-3">
              <div className="col-12 col-md-6">
                <p className="mb-1 text-muted">Başlıq:</p>
                <h6>{selectedAd.title}</h6>

                <p className="mb-1 text-muted mt-3">Kateqoriya / Brend / Model:</p>
                <div>
                  <strong>{selectedAd.categoryName || '—'}</strong> &gt; {selectedAd.brandName} &gt; {selectedAd.modelName}
                </div>

                <p className="mb-1 text-muted mt-3">Hissə növü və Keyfiyyət:</p>
                <div>
                  <span className="badge bg-secondary me-2">{selectedAd.partTypeName}</span>
                  <span className="badge bg-info text-dark">{selectedAd.qualityTypeName}</span>
                </div>

                <p className="mb-1 text-muted mt-3">Qiymət və Zəmanət:</p>
                <div>
                  <strong className="text-accent fs-5">{selectedAd.price} AZN</strong>
                  <span className="ms-3 text-muted">Zəmanət: {selectedAd.warrantyMonths} ay</span>
                </div>

                <p className="mb-1 text-muted mt-3">Stok və Minimum Sifariş:</p>
                <div>
                  <strong>{selectedAd.stockQuantity} ədəd</strong> (Min. sifariş: {selectedAd.minOrderQuantity} ədəd)
                </div>
              </div>

              <div className="col-12 col-md-6">
                <p className="mb-1 text-muted">Satıcı Məlumatları:</p>
                <div className="p-3 rounded" style={{ background: 'var(--surface-color)', border: '1px solid var(--border-color)' }}>
                  <div><strong>{selectedAd.sellerName}</strong></div>
                  <div>Telefon: {selectedAd.sellerPhone || '—'}</div>
                  {selectedAd.sellerWhatsApp && (
                    <div>WhatsApp: <a href={`https://wa.me/${selectedAd.sellerWhatsApp.replace(/\+/g, '')}`} target="_blank" rel="noreferrer">{selectedAd.sellerWhatsApp}</a></div>
                  )}
                </div>

                <p className="mb-1 text-muted mt-3">Təsvir:</p>
                <div className="p-2 rounded" style={{ background: 'var(--surface-color)', minHeight: '80px', whiteSpace: 'pre-wrap' }}>
                  {selectedAd.description || 'Təsvir daxil edilməyib.'}
                </div>
              </div>

              {/* Şəkillər Qalereyası */}
              {selectedAd.images && selectedAd.images.length > 0 && (
                <div className="col-12 mt-3">
                  <p className="mb-2 text-muted">Yüklənmiş Şəkillər:</p>
                  <div className="d-flex gap-2 flex-wrap">
                    {selectedAd.images.map((img, idx) => {
                      const src = imageSrc(img.imageUrl || img.url);
                      return (
                        <a key={img.id || idx} href={src} target="_blank" rel="noreferrer">
                          <img
                            src={src}
                            alt=""
                            style={{
                              width: '80px',
                              height: '80px',
                              objectFit: 'cover',
                              borderRadius: '8px',
                              border: img.isMain ? '2px solid var(--accent)' : '1px solid var(--border-color)'
                            }}
                          />
                        </a>
                      );
                    })}
                  </div>
                </div>
              )}
            </div>

            {/* Əməliyyat Düymələri */}
            <div className="d-flex gap-2 justify-content-end mt-4 pt-3 border-top">
              {activeTab === 'restockAds' ? (
                <button
                  onClick={() => handleRestoreAd(selectedAd.id)}
                  disabled={actionLoading}
                  className="btn"
                  style={{ backgroundColor: '#28a745', color: '#fff' }}
                >
                  <i className="bi bi-arrow-repeat me-1"></i> Bərpanı Təsdiq et və Aktivləşdir
                </button>
              ) : (
                <button
                  onClick={() => handleApproveAd(selectedAd.id)}
                  disabled={actionLoading}
                  className="btn"
                  style={{ backgroundColor: '#28a745', color: '#fff' }}
                >
                  <i className="bi bi-check-lg me-1"></i> Elanı Təsdiq et
                </button>
              )}

              <button
                onClick={() => openRejectAdModal(selectedAd.id)}
                disabled={actionLoading}
                className="btn btn-outline-danger"
              >
                <i className="bi bi-x-lg me-1"></i> İmtina et
              </button>

              <button
                onClick={() => setSelectedAd(null)}
                className="btn btn-outline-secondary"
              >
                Bağla
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* İmtina Səbəbi Modalı */}
      <Modal open={rejectModalOpen} title="İmtina Səbəbi Daxil Edin" onClose={() => setRejectModalOpen(false)}>
        <form onSubmit={handleRejectSubmit}>
          <div className="mb-3">
            <label className="form-label text-muted">
              {rejectType === 'user' ? 'İstifadəçiyə bildiriləcək səbəb:' : 'Satıcıya bildiriləcək imtina səbəbi:'}
            </label>
            <textarea
              className="form-control"
              rows="4"
              value={rejectReason}
              onChange={(e) => setRejectReason(e.target.value)}
              placeholder="Qaydalara uyğunsuzluq, məlumatların çatışmazlığı və s."
              required
            ></textarea>
          </div>
          <div className="d-flex justify-content-end gap-2">
            <button type="button" className="btn btn-outline-secondary" onClick={() => setRejectModalOpen(false)}>
              Ləğv et
            </button>
            <button type="submit" disabled={actionLoading} className="btn btn-danger">
              <i className="bi bi-send me-1"></i> İmtina Qərarını Təsdiqlə
            </button>
          </div>
        </form>
      </Modal>

      {/* Vəsiqə Şəkli Önbaxış Modalı */}
      <Modal open={!!previewIdCard} title={`Vəsiqə Şəkli: ${previewIdCard?.name || ''}`} onClose={() => setPreviewIdCard(null)}>
        {previewIdCard && (
          <div className="text-center p-2">
            <div className="mb-2 text-muted">
              <strong>FİN Kod:</strong> <span className="badge bg-dark ms-1" style={{ fontSize: '1rem', letterSpacing: '2px' }}>{previewIdCard.pin || 'Qeyd edilməyib'}</span>
            </div>
            <div style={{ maxHeight: '75vh', overflow: 'auto' }}>
              <img 
                src={imageSrc(previewIdCard.url)} 
                alt="Vəsiqə" 
                style={{ maxWidth: '100%', maxHeight: '65vh', objectFit: 'contain', borderRadius: '8px', border: '1px solid var(--border-color)' }} 
              />
            </div>
            <div className="mt-3">
              <a 
                href={imageSrc(previewIdCard.url)} 
                target="_blank" 
                rel="noreferrer" 
                className="btn btn-sm btn-outline-info me-2"
              >
                <i className="bi bi-box-arrow-up-right me-1"></i> Yeni tabda aç
              </a>
              <button 
                type="button" 
                className="btn btn-sm btn-secondary" 
                onClick={() => setPreviewIdCard(null)}
              >
                Bağla
              </button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
