import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, statusBadge, Modal, fmtDate } from '../../components/Panel';

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [roleFilter, setRoleFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [search, setSearch] = useState('');
  const [notification, setNotification] = useState('');

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
    if (!window.confirm('Bu istifadəçini təsdiqləmək istədiyinizə əminsiniz?')) return;
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
      alert('Zəhmət olmasa bloklama səbəbini qeyd edin.');
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
    if (!window.confirm('İstifadəçini blokdan çıxarmaq istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/UnblockUser/${id}`, { method: 'POST' });
      notify('İstifadəçi blokdan çıxarıldı.');
      loadUsers();
    } catch (err) {
      notify('Xəta: ' + err.message, 'danger');
    }
  }

  const filteredUsers = users.filter(u => {
    if (roleFilter && u.role !== roleFilter) return false;
    if (statusFilter && u.approvalStatus !== statusFilter) return false;
    if (search) {
      const q = search.toLowerCase();
      const match = (u.fullName && u.fullName.toLowerCase().includes(q)) ||
                    (u.username && u.username.toLowerCase().includes(q)) ||
                    (u.phone && u.phone.includes(q)) ||
                    (u.email && u.email.toLowerCase().includes(q));
      if (!match) return false;
    }
    return true;
  });

  const columns = [
    { key: 'id', label: 'ID' },
    { key: 'fullName', label: 'Ad Soyad' },
    { key: 'username', label: 'İstifadəçi Adı' },
    { key: 'email', label: 'Email', render: u => u.email || '—' },
    { key: 'phone', label: 'Telefon' },
    {
      key: 'role',
      label: 'Rol',
      render: u => (
        <span className={`badge ${u.role === 'Admin' ? 'bg-primary' : u.role === 'Anbardar' ? 'bg-info text-dark' : 'bg-warning text-dark'}`}>
          {u.role}
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

      {/* Filtrlər */}
      <div className="row g-2 mb-3">
        <div className="col-12 col-md-4">
          <input
            type="text"
            className="form-control form-control-sm"
            placeholder="Axtar (Ad, istifadəçi adı, telefon, email)..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <div className="col-6 col-md-3">
          <select className="form-select form-select-sm" value={roleFilter} onChange={e => setRoleFilter(e.target.value)}>
            <option value="">Bütün Rollar</option>
            <option value="Admin">Administrator</option>
            <option value="Usta">Usta</option>
            <option value="Anbardar">Anbardar</option>
          </select>
        </div>
        <div className="col-6 col-md-3">
          <select className="form-select form-select-sm" value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
            <option value="">Bütün Statuslar</option>
            <option value="Approved">Təsdiqlənib</option>
            <option value="Pending">Gözləmədə</option>
            <option value="Rejected">Rədd edilib</option>
          </select>
        </div>
        <div className="col-12 col-md-2 text-md-end text-muted small align-self-center">
          Tapılan: {filteredUsers.length} istifadəçi
        </div>
      </div>

      {loading ? (
        <div className="text-center py-5 text-muted">Yüklənir...</div>
      ) : (
        <Table columns={columns} rows={filteredUsers} empty="Axtarışa uyğun istifadəçi tapılmadı." />
      )}

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
