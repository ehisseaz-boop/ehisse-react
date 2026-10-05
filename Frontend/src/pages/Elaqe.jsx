import React, { useEffect, useState } from 'react';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';

function Elaqe() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => { Api.contact().then(setData).catch(e => setError(e.message)); }, []);

  if (error) return <div className="container"><ErrorBox message={error} /></div>;
  if (!data) return <div className="container"><Loading /></div>;

  return (
    <div className="container">
      <section>
        <h2 className="section-title">{data.title}</h2>
        <p className="text-muted">{data.subtitle}</p>
        <div className="products-grid">
          <div className="product-card">
            <h5><i className="bi bi-geo-alt text-accent"></i> Ünvan</h5>
            <p className="text-muted">{data.address}</p>
          </div>
          <div className="product-card">
            <h5><i className="bi bi-telephone text-accent"></i> Telefon &amp; WhatsApp</h5>
            <p className="mb-1"><a href={`tel:${data.phone.replace(/\s/g, '')}`} className="text-decoration-none">{data.phone}</a></p>
            <p className="mb-0">
              <a href={`https://wa.me/${data.whatsApp.replace(/\D/g, '')}`} target="_blank" rel="noreferrer" className="text-decoration-none">
                <i className="bi bi-whatsapp" style={{ color: '#25D366' }}></i> {data.whatsApp}
              </a>
            </p>
          </div>
          <div className="product-card">
            <h5><i className="bi bi-envelope text-accent"></i> E-poçt</h5>
            <p><a href={`mailto:${data.email}`} className="text-decoration-none">{data.email}</a></p>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Elaqe;
