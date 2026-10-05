import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table } from '../../components/Panel';
import { Link } from 'react-router-dom';

export default function AdminAds() {
  const [ads, setAds] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAds();
  }, []);

  async function loadAds() {
    try {
      const data = await apiFetch('/api/Admin/AllAds');
      setAds(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  const columns = [
    { header: 'ID', accessor: 'Id' },
    { header: 'Şəkil', accessor: (p) => p.MainImage ? <img src={p.MainImage} className="w-12 h-12 object-cover" /> : 'Yoxdur' },
    { header: 'Başlıq', accessor: (p) => <Link to={`/part/${p.Id}`} className="text-blue-600 hover:underline">{p.Title}</Link> },
    { header: 'Qiymət', accessor: (p) => `${p.Price} AZN` },
    { header: 'Bölmə', accessor: 'CategoryName' },
    { header: 'Mağaza', accessor: 'WarehouseName' },
    { header: 'Tarix', accessor: 'CreatedAt' }
  ];

  if (loading) return <div>Yüklənir...</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Bütün Elanlar</h2>
      <Table columns={columns} data={ads} />
    </div>
  );
}
