import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { Table, fmtDate } from '../../components/Panel';

export function SalesHistory() {
  const [rows, setRows] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => { Api.warehouse.salesHistory().then(setRows).catch(e => setError(e.message)); }, []);

  if (error) return <ErrorBox message={error} />;
  if (!rows) return <Loading />;

  const total = rows.reduce((s, r) => s + r.price * r.quantity, 0);

  return (
    <>
      <p>Ümumi satış: <strong>{rows.length}</strong> · Təxmini məbləğ: <strong>{total.toFixed(2)} AZN</strong></p>
      <Table
        rows={rows}
        empty="Hələ satış qeydə alınmayıb."
        columns={[
          { key: 'timestamp', label: 'Tarix', render: r => fmtDate(r.timestamp) },
          { key: 'title', label: 'Məhsul', render: r => <Link to={`/details/${r.sparePartId}`}>{r.title}</Link> },
          { key: 'sku', label: 'SKU' },
          { key: 'quantity', label: 'Say' },
          { key: 'price', label: 'Qiymət', render: r => `${r.price} AZN` },
          { key: 'stock', label: 'Stok', render: r => `${r.previousStock} → ${r.newStock}` },
        ]}
      />
    </>
  );
}

export function Settings() {
  const [form, setForm] = useState(null);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');

  useEffect(() => { Api.warehouse.settings().then(setForm).catch(e => setError(e.message)); }, []);

  if (!form && error) return <ErrorBox message={error} />;
  if (!form) return <Loading />;

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.type === 'checkbox' ? e.target.checked : e.target.value });

  const submit = async (e) => {
    e.preventDefault(); setError(''); setMsg('');
    try { const r = await Api.warehouse.saveSettings(form); setMsg(r.message); }
    catch (err) { setError(err.message); }
  };

  return (
    <form onSubmit={submit} className="product-card" style={{ maxWidth: 560 }}>
      {error && <div className="alert alert-danger">{error}</div>}
      {msg && <div className="alert alert-success">{msg}</div>}
      <label>Şirkət adı</label>
      <input className="form-control mb-2" value={form.companyName || ''} onChange={set('companyName')} required />
      <label>Əlaqə şəxsi</label>
      <input className="form-control mb-2" value={form.contactPerson || ''} onChange={set('contactPerson')} required />
      <label>Mobil nömrə</label>
      <input className="form-control mb-2" value={form.phone || ''} onChange={set('phone')} placeholder="+994..." />
      <label>WhatsApp nömrəsi</label>
      <input className="form-control mb-2" value={form.whatsAppNumber || ''} onChange={set('whatsAppNumber')} placeholder="+994..." />
      <label>Ünvan</label>
      <input className="form-control mb-2" value={form.address || ''} onChange={set('address')} />
      <label className="d-flex align-items-center gap-2 my-2">
        <input type="checkbox" checked={!!form.showContactInfoInAds} onChange={set('showContactInfoInAds')} />
        Əlaqə məlumatlarını elanlarda göstər
      </label>
      <button type="submit" className="btn btn-primary bg-accent text-dark fw-bold">Yadda saxla</button>
    </form>
  );
}
