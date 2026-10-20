import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, statusBadge, Modal, fmtDate } from '../../components/Panel';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [roleFilter, setRoleFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [search, setSearch] = useState('');
  const [notification, setNotification] = useState('');

  // Detallı baxış modalı
  const [selectedUser, setSelectedUser] = useState(null);
  const [previewIdCard, setPreviewIdCard] = useState(null);

  // Bloklama modalı
  const [blockModalOpen, setBlockModalOpen] = useState(false);
  const [blockUserId, setBlockUserId] = useState(null);
  const [blockReason, setBlockReason] = useState('');

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    setLoading(true);
    try {
      const data = await apiFetch('/api/Admin/AllUsers');
      setUsers(data || []);
    } catch (err) {
      console.error(err);
      notify('İstifadəçiləri yükləyərkən xəta: ' + err.message, 'danger');
    } finally {
      setLoading(false);
    }
  }

  function notify(text, type = 'success') {
    setNotification({ text, type });
    setTimeout(() => setNotification(''), 4000);
  }

  async function handleApprove(id) {
    if (!await confirmModal({
      title: 'İstifadəçini təsdiqlə',
      text: 'Bu istifadəçini təsdiqləmək istədiyinizə əminsiniz?',
      confirmText: 'Təsdiqlə',
      icon: 'question'
    })) return;
    try {
      await apiFetch(`/api/Admin/ApproveUser/${id}`, { method: 'POST' });
      notify('İstifadəçi təsdiqləndi.');
      loadUsers();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  function openBlockModal(id) {
    setBlockUserId(id);
    setBlockReason('');
    setBlockModalOpen(true);
  }

  async function handleBlockSubmit(e) {
    e.preventDefault();
    if (!blockReason.trim()) {
      alertModal('Zəhmət olmasa bloklama səbəbini qeyd edin.', 'Diqqət', 'warning');
      return;
    }
    try {
      await apiFetch(`/api/Admin/BlockUser/${blockUserId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reason: blockReason.trim() })
      });
      notify('İstifadəçi bloklandı.');
      setBlockModalOpen(false);
      loadUsers();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleUnblock(id) {
    if (!await confirmModal({
      title: 'Blokdan çıxar',
      text: 'İstifadəçini blokdan çıxarmaq istədiyinizə əminsiniz?',
      confirmText: 'Blokdan çıxar',
      icon: 'question'
    })) return;
    try {
      await apiFetch(`/api/Admin/UnblockUser/${id}`, { method: 'POST' });
      notify('İstifadəçi blokdan çıxarıldı.');
      loadUsers();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  async function handleDeleteUser(user) {
    const confirmText = `${user.fullName || user.username} adlı istifadəçini sistemdən tamamilə silmək istədiyinizə əminsiniz?\n\nQEYD: İstifadəçi silindikdən sonra onun məlumatları və elanları təmizlənəcək və o yenidən sıfırdan qeydiyyatdan keçə biləcək.`;
    if (!await confirmModal({
      title: 'İstifadəçini sil',
      text: confirmText,
      confirmText: 'Bəli, tamamilə sil',
      isDanger: true
    })) return;
    try {
      const res = await apiFetch(`/api/Admin/DeleteUser/${user.id}`, { method: 'POST' });
      notify(res.message || 'İstifadəçi sistemdən silindi.');
      loadUsers();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  const filteredUsers = users.filter(u => {
    if (roleFilter && u.role !== roleFilter) return false;
    if (statusFilter && u.approvalStatus !== statusFilter) return false;
    if (search) {
      const q = search.trim().toLowerCase();
      const match = (u.fullName && u.fullName.toLowerCase().includes(q)) ||
                    (u.firstName && u.firstName.toLowerCase().includes(q)) ||
                    (u.lastName && u.lastName.toLowerCase().includes(q)) ||
                    (u.username && u.username.toLowerCase().includes(q)) ||
                    (u.phone && u.phone.toLowerCase().includes(q)) ||
                    (u.email && u.email.toLowerCase().includes(q)) ||
                    (u.pin && u.pin.toLowerCase().includes(q));
      if (!match) return false;
    }
    return true;
  });

  const columns = [
    { key: 'id', label: 'ID' },
    {
      key: 'fullName',
      label: 'Ad Soyad',
      render: u => (
        <button
          onClick={() => setSelectedUser(u)}
          className="btn btn-link p-0 text-decoration-none fw-bold text-start"
          style={{ color: 'var(--accent, #5c9e00)' }}
          title="Detallı məlumata bax"
        >
          {u.fullName || `${u.firstName} ${u.lastName}`}
        </button>
      )
    },
    {
      key: 'pin',
      label: 'FİN Kod',
      render: u => u.pin ? <span className="badge bg-secondary" style={{ letterSpacing: '1px' }}>{u.pin}</span> : '—'
    },
    { key: 'phone', label: 'Telefon' },
    { key: 'email', label: 'Email', render: u => u.email || '—' },
    {
      key: 'role',
      label: 'Rol',
      render: u => (
        <span className={`badge ${u.role === 'Admin' ? 'bg-primary' : u.role === 'Anbardar' ? 'bg-info text-dark' : 'bg-warning text-dark'}`}>
          {u.role === 'Anbardar' ? 'Satıcı' : u.role}
        </span>
      )
    },
    {
      key: 'approvalStatus',
      label: 'Moderasiya',
      render: u => statusBadge(u.approvalStatus)
    },
    {
      key: 'isBlocked',
      label: 'Vəziyyət',
      render: u => (
        <span className={`badge ${u.isBlocked ? 'bg-danger' : 'bg-success'}`}>
          {u.isBlocked ? 'Bloklanıb' : 'Normal'}
        </span>
      )
    },
    { key: 'createdDate', label: 'Qeydiyyat', render: u => fmtDate(u.createdDate) },
    {
      key: 'actions',
      label: 'Əməliyyatlar',
      render: u => (
        <div className="d-flex gap-1 flex-wrap">
          <button 
            onClick={() => setSelectedUser(u)} 
            className="btn btn-sm btn-outline-info py-0 px-2" 
            title="Detallara bax"
          >
            <i className="bi bi-eye"></i>
          </button>

          {u.idCardImageUrl && (
            <button
              onClick={() => setPreviewIdCard({ url: u.idCardImageUrl, name: u.fullName, pin: u.pin })}
              className="btn btn-sm btn-outline-warning py-0 px-2"
              title="Vəsiqə şəklinə bax"
            >
              <i className="bi bi-card-image"></i>
            </button>
          )}

          {u.role !== 'Admin' && u.approvalStatus === 'Pending' && (
            <button onClick={() => handleApprove(u.id)} className="btn btn-sm btn-success py-0 px-2" title="Təsdiqlə">
              <i className="bi bi-check-lg"></i>
            </button>
          )}

          {u.role !== 'Admin' && (
            u.isBlocked ? (
              <button onClick={() => handleUnblock(u.id)} className="btn btn-sm btn-outline-success py-0 px-2" title="Blokdan çıxar">
                <i className="bi bi-unlock"></i>
              </button>
            ) : (
              <button onClick={() => openBlockModal(u.id)} className="btn btn-sm btn-outline-danger py-0 px-2" title="Blokla">
                <i className="bi bi-slash-circle"></i>
              </button>
            )
          )}

          {u.role !== 'Admin' && (
            <button 
              onClick={() => handleDeleteUser(u)} 
              className="btn btn-sm btn-outline-danger py-0 px-2" 
              title="İstifadəçini tam sil (Yenidən qeydiyyat üçün)"
            >
              <i className="bi bi-trash"></i>
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
          <h3 className="section-title mb-1">Bütün İstifadəçilər</h3>
          <p className="text-muted m-0">Sistemdəki administratorlar, ustalar və anbardarların idarə edilməsi</p>
        </div>
        <button onClick={loadUsers} className="btn btn-sm btn-outline">
          <i className="bi bi-arrow-clockwise me-1"></i> Yenilə
        </button>
      </div>

      {notification && (
        <div className={`alert alert-${notification.type} mb-4 d-flex align-items-center justify-content-between`}>
          <span>{notification.text}</span>
          <button type="button" className="btn-close" onClick={() => setNotification('')}></button>
        </div>
      )}

      {/* Filtrlər və Sürətli Axtarış */}
      <div className="row g-2 mb-3">
        <div className="col-12 col-md-5">
          <input
            type="text"
            className="form-control form-control-sm"
            placeholder="Sürətli axtarış (Ad, soyad, telefon, FİN kod, email)..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <div className="col-6 col-md-2">
          <select className="form-select form-select-sm" value={roleFilter} onChange={e => setRoleFilter(e.target.value)}>
            <option value="">Bütün Rollar</option>
            <option value="Admin">Administrator</option>
            <option value="Usta">Usta</option>
            <option value="Anbardar">Satıcı</option>
          </select>
        </div>
        <div className="col-6 col-md-2">
          <select className="form-select form-select-sm" value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
            <option value="">Bütün Statuslar</option>
            <option value="Approved">Təsdiqlənib</option>
            <option value="Pending">Gözləmədə</option>
            <option value="Rejected">Rədd edilib</option>
          </select>
        </div>
        <div className="col-12 col-md-3 text-md-end text-muted small align-self-center">
          Tapılan: {filteredUsers.length} istifadəçi
        </div>
      </div>

      {loading ? (
        <div className="text-center py-5 text-muted">Yüklənir...</div>
      ) : (
        <Table columns={columns} rows={filteredUsers} empty="Axtarışa uyğun istifadəçi tapılmadı." />
      )}

      {/* İstifadəçi Detalları Modalı */}
      <Modal open={!!selectedUser} title={`İstifadəçi Detalları: ${selectedUser?.fullName || ''}`} onClose={() => setSelectedUser(null)}>
        {selectedUser && (
          <div>
            <div className="row g-3">
              <div className="col-12 col-md-6">
                <div className="p-3 rounded" style={{ backgroundColor: 'var(--bg-main)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3 text-primary"><i className="bi bi-person-badge me-2"></i>Şəxsi Məlumatlar</h6>
                  <p className="mb-2"><strong>Ad, Soyad:</strong> {selectedUser.fullName || `${selectedUser.firstName} ${selectedUser.lastName}`}</p>
                  <p className="mb-2">
                    <strong>FİN Kod:</strong>{' '}
                    <span className="badge bg-dark ms-1" style={{ fontSize: '0.9rem', letterSpacing: '2px' }}>
                      {selectedUser.pin || 'Qeyd edilməyib'}
                    </span>
                  </p>
                  <p className="mb-2"><strong>Telefon:</strong> <a href={`tel:${selectedUser.phone}`} className="text-decoration-none">{selectedUser.phone}</a></p>
                  <p className="mb-2"><strong>Email:</strong> {selectedUser.email || '—'}</p>
                  <p className="mb-2"><strong>İstifadəçi Adı:</strong> {selectedUser.username}</p>
                  <p className="mb-2"><strong>Rol:</strong> <span className="badge bg-secondary">{selectedUser.role}</span></p>
                  <p className="mb-0"><strong>Qeydiyyat Tarixi:</strong> {fmtDate(selectedUser.createdDate)}</p>
                </div>
              </div>

              <div className="col-12 col-md-6">
                <div className="p-3 rounded h-100 d-flex flex-column" style={{ backgroundColor: 'var(--bg-main)', border: '1px solid var(--border-color)' }}>
                  <h6 className="mb-3 text-info"><i className="bi bi-card-image me-2"></i>Şəxsiyyət Vəsiqəsi</h6>
                  {selectedUser.idCardImageUrl ? (
                    <div className="text-center my-auto">
                      <img 
                        src={selectedUser.idCardImageUrl} 
                        alt="Şəxsiyyət vəsiqəsi" 
                        style={{ maxWidth: '100%', maxHeight: '180px', objectFit: 'contain', borderRadius: '6px', border: '1px solid var(--border-color)', cursor: 'pointer' }}
                        onClick={() => setPreviewIdCard({ url: selectedUser.idCardImageUrl, name: selectedUser.fullName, pin: selectedUser.pin })}
                      />
                      <div className="mt-2">
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-info"
                          onClick={() => setPreviewIdCard({ url: selectedUser.idCardImageUrl, name: selectedUser.fullName, pin: selectedUser.pin })}
                        >
                          <i className="bi bi-arrows-fullscreen me-1"></i> Şəkilə böyük bax
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div className="text-muted text-center my-auto py-4">
                      <i className="bi bi-image" style={{ fontSize: '2rem' }}></i>
                      <p className="mt-2 mb-0 small">Vəsiqə şəkli yüklənməyib.</p>
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="mt-4 pt-3 border-top d-flex justify-content-between align-items-center">
              <div>
                <strong>Status:</strong> {statusBadge(selectedUser.approvalStatus)}
                <span className={`badge ms-2 ${selectedUser.isBlocked ? 'bg-danger' : 'bg-success'}`}>
                  {selectedUser.isBlocked ? 'Bloklanıb' : 'Aktiv'}
                </span>
              </div>
              <button type="button" className="btn btn-secondary btn-sm" onClick={() => setSelectedUser(null)}>
                Bağla
              </button>
            </div>
          </div>
        )}
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
                src={previewIdCard.url} 
                alt="Vəsiqə" 
                style={{ maxWidth: '100%', maxHeight: '65vh', objectFit: 'contain', borderRadius: '8px', border: '1px solid var(--border-color)' }} 
              />
            </div>
            <div className="mt-3">
              <a 
                href={previewIdCard.url} 
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

      {/* Bloklama Modalı */}
      <Modal open={blockModalOpen} title="İstifadəçini Blokla" onClose={() => setBlockModalOpen(false)}>
        <form onSubmit={handleBlockSubmit}>
          <div className="mb-3">
            <label className="form-label text-muted">Bloklama səbəbini qeyd edin:</label>
            <textarea
              className="form-control"
              rows="3"
              placeholder="Qayda pozuntusu, saxta profil və s."
              value={blockReason}
              onChange={e => setBlockReason(e.target.value)}
              required
            ></textarea>
          </div>
          <div className="d-flex justify-content-end gap-2">
            <button type="button" className="btn btn-outline-secondary" onClick={() => setBlockModalOpen(false)}>
              Ləğv et
            </button>
            <button type="submit" className="btn btn-danger">
              <i className="bi bi-slash-circle me-1"></i> Blokla
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
