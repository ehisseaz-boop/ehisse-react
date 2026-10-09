import React, { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { Table, statusBadge } from '../../components/Panel';

function WarehouseIndex() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');

  const load = useCallback(() => {
    Api.warehouse.list().then(setData).catch(e => setError(e.message));
  }, []);

  useEffect(() => { load(); }, [load]);

  const run = async (fn, okMsg) => {
    setMsg('');
    try { await fn(); if (okMsg) setMsg(okMsg); load(); }
    catch (e) { setMsg(e.message); }
  };

  const changePrice = (p) => {
    const v = window.prompt('Yeni qiymət (AZN):', p.price);
    if (v === null) return;
    const n = Number(String(v).replace(',', '.'));
    if (!(n > 0)) { setMsg('Qiymət sıfırdan böyük olmalıdır.'); return; }
    run(() => Api.warehouse.updatePrice(p.id, n), 'Qiymət yeniləndi.');
  };

  if (error) return <ErrorBox message={error} />;
  if (!data) return <Loading />;

  const parts = data.parts;
  const columns = [
    { key: 'image', label: '', render: p => p.image ? <img src={imageSrc(p.image)} alt="" style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 6 }} /> : null },
    { key: 'title', label: 'Elan', render: p => <><Link to={`/details/${p.id}`} className="text-decoration-none">{p.brandName} {p.modelName} {p.title}</Link><div className="text-muted" style={{ fontSize: '0.8rem' }}>{p.sku} · {p.partTypeName} · {p.qualityTypeName}</div></> },
    { key: 'price', label: 'Qiymət', render: p => <a href="#" onClick={e => { e.preventDefault(); changePrice(p); }} title="Qiyməti dəyiş">{p.price} AZN</a> },
    { key: 'stockQuantity', label: 'Stok' },
    { key: 'viewCount', label: 'Baxış' },
    { key: 'status', label: 'Status', render: p => <>{statusBadge(p.moderationStatus)}{p.moderationStatus === 'Rejected' && p.rejectionReason && <div className="text-muted" style={{ fontSize: '0.75rem' }}>{p.rejectionReason}</div>}<div style={{ fontSize: '0.75rem' }}>{p.isActive ? 'Aktiv' : 'Deaktiv'}</div></> },
    {
      key: 'actions', label: '', render: p => (
        <div className="d-flex gap-1 flex-wrap">
          <button className="btn-outline" title="Satış qeyd et" disabled={p.stockQuantity <= 0} onClick={() => run(() => Api.warehouse.recordSale(p.id), 'Satış qeydə alındı.')}><i className="bi bi-cart-check"></i></button>
          <Link to={`/warehouse/edit-part/${p.id}`} className="btn-outline" title="Redaktə"><i className="bi bi-pencil"></i></Link>
          <button className="btn-outline" title={p.isActive ? 'Deaktiv et' : 'Aktiv et'} onClick={() => run(() => Api.warehouse.toggleActive(p.id))}><i className={`bi bi-${p.isActive ? 'eye-slash' : 'eye'}`}></i></button>
          <button className="btn-outline" title="Sil" onClick={() => window.confirm('Elanı silmək istədiyinizə əminsiniz?') && run(() => Api.warehouse.remove(p.id), 'Elan silindi.')}><i className="bi bi-trash"></i></button>
        </div>
      )
    },
  ];

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div>
          <strong>{data.seller.companyName}</strong> · {parts.length} elan · Aktiv: {parts.filter(p => p.isActive).length}
        </div>
        <Link to="/warehouse/add-part" className="btn-outline bg-accent text-decoration-none" style={{ color: '#000' }}><i className="bi bi-plus-lg"></i> Yeni elan</Link>
      </div>
      {msg && <div className="alert alert-info">{msg}</div>}
      <Table columns={columns} rows={parts} empty="Hələ elanınız yoxdur." />
    </>
  );
}

export default WarehouseIndex;
