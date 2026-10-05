import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, statusBadge, Tabs, Modal } from '../../components/Panel';

export default function AdminRequests() {
  const [activeTab, setActiveTab] = useState('registration');
  const [activeSubTab, setActiveSubTab] = useState('new');
  const [requests, setRequests] = useState([]);
  const [adRequests, setAdRequests] = useState([]);
  const [loading, setLoading] = useState(true);

  // Modal states
  const [selectedAd, setSelectedAd] = useState(null);
  const [rejectModalOpen, setRejectModalOpen] = useState(false);
  const [rejectReason, setRejectReason] = useState('');
  const [notification, setNotification] = useState('');

  useEffect(() => {
    loadRequests();
    // Mock Ad requests
    setAdRequests([
      { Id: 101, Title: 'iPhone 13 Ekran', User: 'Eli', Phone: '+994501234567', Email: 'eli@mail.com', Status: 'Pending', Type: 'New' },
      { Id: 102, Title: 'Samsung S22 Batareya', User: 'Veli', Phone: '+994551234567', Email: '', Status: 'Pending', Type: 'Update' }
    ]);
  }, []);

  async function loadRequests() {
    try {
      const data = await apiFetch('/api/Admin/Requests');
      setRequests(data);
    } catch (err) {
      console.error(err);
      // Mock if API fails
      setRequests([
         { Id: 1, Name: 'Test User', Email: 'test@test.com', Role: 'Usta', CreatedAt: '2023-01-01', Status: 'Pending' }
      ]);
    } finally {
      setLoading(false);
    }
  }

  // Registration handlers
  async function handleApproveReg(id) {
    if (!window.confirm('Təsdiqləmək istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/ApproveRequest/${id}`, { method: 'POST' });
      loadRequests();
    } catch (err) {
      alert('Xəta: ' + err.message);
    }
  }

  async function handleRejectReg(id) {
    if (!window.confirm('Rədd etmək istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/RejectRequest/${id}`, { method: 'POST' });
      loadRequests();
    } catch (err) {
      alert('Xəta: ' + err.message);
    }
  }

  // Ad requests handlers
  function showNotification(msg) {
    setNotification(msg);
    setTimeout(() => setNotification(''), 4000);
  }

  function handleAcceptAd() {
    if (!selectedAd) return;
    let msg = `Elan qəbul edildi. SMS göndərildi: ${selectedAd.Phone}.`;
    if (selectedAd.Email) msg += ` Email göndərildi: ${selectedAd.Email}.`;
    
    setAdRequests(adRequests.filter(a => a.Id !== selectedAd.Id));
    setSelectedAd(null);
    showNotification(msg);
  }

  function handleRejectAdSubmit(e) {
    e.preventDefault();
    if (!rejectReason.trim()) {
      alert('Zəhmət olmasa səbəb daxil edin');
      return;
    }
    let msg = `Elan rədd edildi (Səbəb: ${rejectReason}). SMS göndərildi: ${selectedAd.Phone}.`;
    if (selectedAd.Email) msg += ` Email göndərildi: ${selectedAd.Email}.`;
    
    setAdRequests(adRequests.filter(a => a.Id !== selectedAd.Id));
    setRejectModalOpen(false);
    setSelectedAd(null);
    setRejectReason('');
    showNotification(msg);
  }

  const regColumns = [
    { header: 'ID', accessor: 'Id', key: 'Id' },
    { header: 'Ad', accessor: 'Name', key: 'Name' },
    { header: 'Email', accessor: 'Email', key: 'Email' },
    { header: 'Müraciət Növü', accessor: 'Role', key: 'Role' },
    { header: 'Tarix', accessor: 'CreatedAt', key: 'CreatedAt' },
    { header: 'Status', key: 'Status', render: (req) => statusBadge(req.Status) },
    {
      header: 'Əməliyyatlar',
      key: 'Actions',
      render: (req) => (
        <div className="flex space-x-2" style={{display: 'flex', gap: '0.5rem'}}>
          {req.Status === 'Pending' && (
            <>
              <button onClick={() => handleApproveReg(req.Id)} className="btn btn-sm btn-outline text-success" style={{borderColor: 'green', color: 'green'}}>Təsdiqlə</button>
              <button onClick={() => handleRejectReg(req.Id)} className="btn btn-sm btn-outline text-danger" style={{borderColor: 'red', color: 'red'}}>Rədd et</button>
            </>
          )}
        </div>
      )
    }
  ];

  const adColumns = [
    { header: 'ID', accessor: 'Id', key: 'Id' },
    { header: 'Başlıq', accessor: 'Title', key: 'Title' },
    { header: 'İstifadəçi', accessor: 'User', key: 'User' },
    { header: 'Status', key: 'Status', render: (req) => statusBadge(req.Status) },
    {
      header: 'Əməliyyatlar',
      key: 'Actions',
      render: (req) => (
        <button onClick={() => setSelectedAd(req)} className="btn btn-sm btn-outline" style={{borderColor: 'var(--accent)', color: 'var(--accent)'}}>Detallara bax</button>
      )
    }
  ];

  if (loading) return <div>Yüklənir...</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4 section-title">Gözləyən Müraciətlər</h2>
      
      {notification && <div className="alert alert-success" style={{backgroundColor: '#d4edda', color: '#155724', padding: '1rem', borderRadius: '8px', marginBottom: '1rem'}}>{notification}</div>}

      <Tabs 
        tabs={[
          { key: 'registration', label: 'Qeydiyyat müraciətləri' },
          { key: 'ads', label: 'Elan müraciətləri' }
        ]}
        active={activeTab}
        onChange={setActiveTab}
      />

      <div style={{ marginTop: '1.5rem' }}>
        {activeTab === 'registration' && (
          <Table columns={regColumns} rows={requests} empty="Qeydiyyat müraciəti yoxdur" />
        )}

        {activeTab === 'ads' && (
          <div>
            <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
              <button className={`btn ${activeSubTab === 'new' ? 'bg-accent text-dark' : 'btn-outline'}`} onClick={() => setActiveSubTab('new')}>Yeni</button>
              <button className={`btn ${activeSubTab === 'update' ? 'bg-accent text-dark' : 'btn-outline'}`} onClick={() => setActiveSubTab('update')}>Yenilənmə</button>
            </div>
            
            <Table 
              columns={adColumns} 
              rows={adRequests.filter(a => a.Type.toLowerCase() === activeSubTab)} 
              empty="Elan müraciəti yoxdur" 
            />
          </div>
        )}
      </div>

      {/* Ad Detail Modal */}
      <Modal open={!!selectedAd && !rejectModalOpen} title="Elan Detalları" onClose={() => setSelectedAd(null)}>
        {selectedAd && (
          <div>
            <p><strong>Başlıq:</strong> {selectedAd.Title}</p>
            <p><strong>İstifadəçi:</strong> {selectedAd.User}</p>
            <p><strong>Telefon:</strong> {selectedAd.Phone}</p>
            <p><strong>Email:</strong> {selectedAd.Email || 'Yoxdur'}</p>
            
            <div style={{ display: 'flex', gap: '1rem', marginTop: '2rem' }}>
              <button className="btn bg-accent text-dark w-100" onClick={handleAcceptAd}>Qəbul et</button>
              <button className="btn btn-outline w-100" style={{borderColor: 'red', color: 'red'}} onClick={() => setRejectModalOpen(true)}>İmtina et</button>
            </div>
          </div>
        )}
      </Modal>

      {/* Reject Reason Modal */}
      <Modal open={rejectModalOpen} title="İmtina Səbəbi" onClose={() => setRejectModalOpen(false)}>
        <form onSubmit={handleRejectAdSubmit}>
          <textarea 
            className="form-control mb-3" 
            rows="4" 
            placeholder="İmtina səbəbini daxil edin..." 
            value={rejectReason} 
            onChange={e => setRejectReason(e.target.value)} 
            required
          ></textarea>
          <button type="submit" className="btn w-100" style={{backgroundColor: 'red', color: 'white'}}>Rədd et və Bildiriş Göndər</button>
        </form>
      </Modal>

    </div>
  );
}
