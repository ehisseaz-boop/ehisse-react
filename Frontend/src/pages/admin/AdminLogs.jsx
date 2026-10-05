import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table } from '../../components/Panel';

export default function AdminLogs() {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLogs();
  }, []);

  async function loadLogs() {
    try {
      const data = await apiFetch('/api/Admin/ActivityLogs');
      setLogs(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  const columns = [
    { header: 'Tarix', accessor: 'CreatedAt' },
    { header: 'İstifadəçi', accessor: 'UserName' },
    { header: 'Fəaliyyət', accessor: 'Action' },
    { header: 'Detal', accessor: 'Details' }
  ];

  if (loading) return <div>Yüklənir...</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Fəaliyyət Jurnalı (Logs)</h2>
      <Table columns={columns} data={logs} />
    </div>
  );
}
