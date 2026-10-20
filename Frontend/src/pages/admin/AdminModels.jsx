import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { Table } from '../../components/Panel';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function AdminModels() {
  const [brands, setBrands] = useState([]);
  const [loading, setLoading] = useState(true);
  const [brandName, setBrandName] = useState('');

  useEffect(() => {
    loadBrands();
  }, []);

  async function loadBrands() {
    try {
      const data = await apiFetch('/api/Admin/ModelsAndBrands');
      setBrands(data.Brands || []);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  async function handleCreateBrand(e) {
    e.preventDefault();
    try {
      await apiFetch('/api/Admin/CreateBrand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: brandName })
      });
      setBrandName('');
      loadBrands();
    } catch (err) {
      alertModal('Xəta: ' + err.message, 'Xəta', 'error');
    }
  }

  async function handleDeleteBrand(id) {
    if (!await confirmModal('Markanı silmək istədiyinizə əminsiniz?')) return;
    try {
      await apiFetch(`/api/Admin/DeleteBrand/${id}`, { method: 'POST' });
      loadBrands();
    } catch (err) {
      alertModal('Xəta: ' + err.message, 'Xəta', 'error');
    }
  }

  const columns = [
    { header: 'ID', accessor: 'Id' },
    { header: 'Ad', accessor: 'Name' },
    { header: 'Əməliyyatlar', accessor: (b) => <button onClick={() => handleDeleteBrand(b.Id)} className="text-red-600 hover:underline">Sil</button> }
  ];

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Markalar və Modellər</h2>
      
      <form onSubmit={handleCreateBrand} className="mb-8 p-4 bg-gray-50 border rounded flex space-x-4">
        <input 
          type="text" placeholder="Yeni marka adı" required
          value={brandName} onChange={e => setBrandName(e.target.value)}
          className="border p-2 rounded flex-1"
        />
        <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded">Əlavə et</button>
      </form>

      {loading ? <div>Yüklənir...</div> : <Table columns={columns} data={brands} />}
    </div>
  );
}
