import React, { useEffect, useState } from 'react';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';

function Saticilar() {
  const [sellers, setSellers] = useState(null);
  const [search, setSearch] = useState('');
  const [error, setError] = useState('');

  const load = (q) => {
    setSellers(null);
    setError('');
    Api.sellers(q).then(setSellers).catch(e => setError(e.message));
  };

  useEffect(() => { load(''); }, []);

  return (
    <div className="container">
      <section>
        <div className="section-header">
          <div>
            <div className="section-label">Təsdiqli</div>
            <h2 className="section-title">Satıcılar</h2>
          </div>
          <form onSubmit={e => { e.preventDefault(); load(search); }} className="d-flex gap-2">
            <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Satıcı axtar..." className="form-control" />
            <button type="submit" className="btn-outline">Axtar</button>
          </form>
        </div>

        {error ? <ErrorBox message={error} /> : !sellers ? <Loading /> : sellers.length === 0 ? (
          <div className="text-center py-5 text-muted">Satıcı tapılmadı</div>
        ) : (
          <div className="products-grid">
            {sellers.map(s => (
              <div key={s.id} className="product-card">
                <h3 className="product-title"><i className="bi bi-shop text-accent"></i> {s.companyName}</h3>
                <p className="product-desc">{s.contactPerson}</p>
                {s.address && <p className="product-desc"><i className="bi bi-geo-alt"></i> {s.address}</p>}
                {s.phone && <a href={`tel:${s.phone}`} className="text-decoration-none d-block"><i className="bi bi-telephone"></i> {s.phone}</a>}
                {s.whatsAppNumber && (
                  <a href={`https://wa.me/${s.whatsAppNumber.replace(/\D/g, '')}`} target="_blank" rel="noreferrer" className="text-decoration-none d-block">
                    <i className="bi bi-whatsapp"></i> {s.whatsAppNumber}
                  </a>
                )}
                <div className="product-meta mt-2">
                  <span className="product-stock">Aktiv elan: {s.activeAds} / {s.totalAds}</span>
                  <span>★ {s.rating}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

export default Saticilar;
