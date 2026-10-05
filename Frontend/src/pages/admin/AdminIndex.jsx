import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table } from '../../components/Panel';

export default function AdminIndex() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  async function loadStats() {
    try {
      const data = await apiFetch('/api/Admin/Index');
      setStats(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <div>Yüklənir...</div>;
  if (!stats) return <div>Xəta baş verdi.</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Admin Panel</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <div className="bg-white p-4 rounded shadow">
          <h3 className="text-gray-500">Ümumi İstifadəçilər</h3>
          <p className="text-3xl font-bold">{stats.CountUsers}</p>
        </div>
        <div className="bg-white p-4 rounded shadow">
          <h3 className="text-gray-500">Ümumi Elanlar</h3>
          <p className="text-3xl font-bold">{stats.CountParts}</p>
        </div>
        <div className="bg-white p-4 rounded shadow">
          <h3 className="text-gray-500">Aktiv Müraciətlər</h3>
          <p className="text-3xl font-bold text-orange-500">{stats.ActiveRequests}</p>
        </div>
      </div>
    </div>
  );
}
