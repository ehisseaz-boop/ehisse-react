import React, { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { Table } from '../../components/Panel';

function WarehouseDeleted() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');
  const [busyId, setBusyId] = useState(null);

  const load = useCallback(() => {
    Api.warehouse.deletedAds()
      .then(setData)
      .catch(e => setError(e.message));
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleRestore = async (id) => {
    setMsg('');
    setBusyId(id);
    try {
      const res = await Api.warehouse.restorePart(id);
      setMsg(res.message || 'Elan uğurla bərpa edildi.');
      load();
    } catch (err) {
      setMsg(err.message || 'Elan bərpa edilərkən xəta baş verdi.');
    } finally {
      setBusyId(null);
    }
  };

  if (error) return <ErrorBox message={error} />;
  if (!data) return <Loading />;

  const parts = data.parts || [];

  const columns = [
    {
      key: 'image',
      label: '',
      render: p => p.image ? (
        <img
          src={imageSrc(p.image)}
          alt=""
          style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 6, opacity: 0.7 }}
        />
      ) : null
    },
    {
      key: 'title',
      label: 'Silinmiş elan',
      render: p => (
        <>
          <strong className="text-muted">{p.brandName} {p.modelName} {p.title}</strong>
          <div className="text-muted" style={{ fontSize: '0.8rem' }}>
            {p.sku ? `${p.sku} · ` : ''}{p.partTypeName} · {p.qualityTypeName}
          </div>
        </>
      )
    },
    { key: 'price', label: 'Qiymət', render: p => <span>{p.price} AZN</span> },
    { key: 'stockQuantity', label: 'Stok' },
    {
      key: 'deletedDate',
      label: 'Silinmə tarixi',
      render: p => p.deletedDate ? new Date(p.deletedDate).toLocaleDateString('az-AZ', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—'
    },
    {
      key: 'actions',
      label: 'Əməliyyat',
      render: p => (
        <button
          className="btn btn-sm btn-outline d-inline-flex align-items-center gap-1"
          style={{ borderColor: 'var(--accent)', color: 'var(--accent)', borderRadius: '6px' }}
          disabled={busyId === p.id}
          onClick={() => handleRestore(p.id)}
          title="Elanı bərpa et və aktiv siyahıya qaytar"
        >
          <i className="bi bi-arrow-counterclockwise"></i>
          {busyId === p.id ? 'Bərpa olunur...' : 'Bərpa et'}
        </button>
      )
    }
  ];

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div>
          <h4 className="m-0 text-main">
            <i className="bi bi-trash3 text-accent me-2"></i>Silinmiş elanlar (Zibil qutusu)
          </h4>
          <span className="text-muted small">Cəmi: {parts.length} silinmiş elan</span>
        </div>
        <Link to="/warehouse" className="btn-outline text-decoration-none">
          <i className="bi bi-arrow-left me-1"></i> Elanlarıma qayıt
        </Link>
      </div>

      {msg && <div className="alert alert-info">{msg}</div>}

      <Table
        columns={columns}
        rows={parts}
        empty="Zibil qutusu boşdur. Hər hansı silinmiş elanınız yoxdur."
      />
    </>
  );
}

export default WarehouseDeleted;
