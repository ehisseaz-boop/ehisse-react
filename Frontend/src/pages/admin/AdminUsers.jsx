import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table, statusBadge } from '../../components/Panel';

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    try {
      const data = await apiFetch('/api/Admin/AllUsers');
      setUsers(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!confirm('İstifadəçini silmək istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/DeleteUser/${id}`, { method: 'POST' });
      loadUsers();
    } catch (err) {
      alert('Xəta: ' + err.message);
    }
  }

  const columns = [
    { header: 'ID', accessor: 'Id' },
    { header: 'Ad', accessor: 'Name' },
    { header: 'Email', accessor: 'Email' },
    { header: 'Rol', accessor: 'Role' },
    { header: 'Telefon', accessor: 'PhoneNumber' },
    {
      header: 'Əməliyyatlar',
      accessor: (u) => (
        <button onClick={() => handleDelete(u.Id)} className="text-red-600 hover:underline">Sil</button>
      )
    }
  ];

  if (loading) return <div>Yüklənir...</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Bütün İstifadəçilər</h2>
      <Table columns={columns} data={users} />
    </div>
  );
}
