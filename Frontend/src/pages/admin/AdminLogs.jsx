import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, Tabs, fmtDate } from '../../components/Panel';

export default function AdminLogs() {
  const [activeTab, setActiveTab] = useState('activity');
  const [activityLogs, setActivityLogs] = useState([]);
  const [saleAudits, setSaleAudits] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLogs();
  }, []);

  async function loadLogs() {
    setLoading(true);
    try {
      const [act, sales] = await Promise.all([
        apiFetch('/api/Admin/ActivityLogs'),
        apiFetch('/api/Admin/SaleAudits')
      ]);
      setActivityLogs(act || []);
      setSaleAudits(sales || []);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  const activityColumns = [
    { key: 'id', label: 'ID' },
    { key: 'date', label: 'Tarix', render: l => fmtDate(l.date) },
    { key: 'userId', label: 'İstifadəçi ID' },
    {
      key: 'action',
      label: 'Fəaliyyət',
      render: l => <span className="badge bg-secondary">{l.action}</span>
    },
    { key: 'entityName', label: 'Obyekt Növü', render: l => l.entityName || '—' },
    { key: 'entityId', label: 'Obyekt ID', render: l => l.entityId || '—' },
    { key: 'details', label: 'Təfərrüat' }
  ];

  const saleColumns = [
    { key: 'id', label: 'Audit ID' },
    { key: 'timestamp', label: 'Satış Tarixi', render: s => fmtDate(s.timestamp) },
    { key: 'sparePartId', label: 'Elan ID' },
    { key: 'title', label: 'Məhsul Başlığı', render: s => <strong>{s.title || '—'}</strong> },
    { key: 'sku', label: 'SKU', render: s => s.sku || '—' },
    { key: 'price', label: 'Qiymət', render: s => `${s.price} AZN` },
    { key: 'sellerUserId', label: 'Satıcı ID' },
    { key: 'previousStock', label: 'Əvvəlki Stok' },
    {
      key: 'newStock',
      label: 'Yeni Stok',
      render: s => (
        <span className={`badge ${s.newStock > 0 ? 'bg-success' : 'bg-danger'}`}>
          {s.newStock} ədəd {s.newStock === 0 && '(Bitdi)'}
        </span>
      )
    }
  ];

  const tabsConfig = [
    { key: 'activity', label: 'Admin və Sistem Fəaliyyətləri', count: activityLogs.length },
    { key: 'sales', label: 'Satış Audit Qeydləri', count: saleAudits.length }
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h3 className="section-title mb-1">Audit və Əməliyyat Jurnalı</h3>
          <p className="text-muted m-0">Sistem hərəkətləri və satıcıların əl ilə qeydə aldığı satışların dəyişməz arxivi</p>
        </div>
        <button onClick={loadLogs} className="btn btn-sm btn-outline">
          <i className="bi bi-arrow-clockwise me-1"></i> Yenilə
        </button>
      </div>

      <Tabs tabs={tabsConfig} active={activeTab} onChange={setActiveTab} />

      <div className="mt-4">
        {loading ? (
          <div className="text-center py-5 text-muted">Jurnal yüklənir...</div>
        ) : (
          <>
            {activeTab === 'activity' && (
              <Table columns={activityColumns} rows={activityLogs} empty="Fəaliyyət jurnalı boşdur." />
            )}

            {activeTab === 'sales' && (
              <Table columns={saleColumns} rows={saleAudits} empty="Hələ ki, qeydə alınmış satış yoxdur." />
            )}
          </>
        )}
      </div>
    </div>
  );
}
