import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table } from '../../components/Panel';

export default function AdminCategories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState('');
  const [icon, setIcon] = useState('');

  useEffect(() => {
    loadCategories();
  }, []);

  async function loadCategories() {
    try {
      const data = await apiFetch('/api/Admin/Categories');
      setCategories(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  async function handleCreate(e) {
    e.preventDefault();
    try {
      await apiFetch('/api/Admin/CreateCategory', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: name, Icon: icon })
      });
      setName('');
      setIcon('');
      loadCategories();
    } catch (err) {
      alert('Xəta: ' + err.message);
    }
  }

  async function handleDelete(id) {
    if (!confirm('Silmək istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/DeleteCategory/${id}`, { method: 'POST' });
      loadCategories();
    } catch (err) {
      alert('Xəta: ' + err.message);
    }
  }

  const columns = [
    { header: 'ID', accessor: 'Id' },
    { header: 'Ad', accessor: 'Name' },
    { header: 'İkon', accessor: 'Icon' },
    { header: 'Əməliyyatlar', accessor: (c) => <button onClick={() => handleDelete(c.Id)} className="text-red-600 hover:underline">Sil</button> }
  ];

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Bölmələr (Kateqoriyalar)</h2>
      
      <form onSubmit={handleCreate} className="mb-8 p-4 bg-gray-50 border rounded flex space-x-4">
        <input 
          type="text" placeholder="Kateqoriya adı" required
          value={name} onChange={e => setName(e.target.value)}
          className="border p-2 rounded flex-1"
        />
        <input 
          type="text" placeholder="İkon (CSS class)"
          value={icon} onChange={e => setIcon(e.target.value)}
          className="border p-2 rounded flex-1"
        />
        <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded">Əlavə et</button>
      </form>

      {loading ? <div>Yüklənir...</div> : <Table columns={columns} data={categories} />}
    </div>
  );
}
