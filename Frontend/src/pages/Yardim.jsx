import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';

function Yardim() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [open, setOpen] = useState(0);
  const [q, setQ] = useState('');

  useEffect(() => { Api.help().then(setData).catch(e => setError(e.message)); }, []);

  if (error) return <div className="container"><ErrorBox message={error} /></div>;
  if (!data) return <div className="container"><Loading /></div>;

  const faqs = data.faqs.filter(f => !q || (f.question + ' ' + f.answer).toLowerCase().includes(q.toLowerCase()));

  return (
    <div className="container">
      <section className="text-center">
        <h1 className="section-title"><i className="bi bi-question-circle text-accent"></i> {data.title}</h1>
        <p className="text-muted">{data.subtitle}</p>
        <input className="form-control mx-auto" style={{ maxWidth: 500 }} placeholder="Sual axtar..." value={q} onChange={e => setQ(e.target.value)} />
      </section>

      <section>
        <div className="products-grid">
          {data.topics.map((t, i) => (
            <div key={i} className="product-card">
              <i className={`bi ${t.icon} text-accent`} style={{ fontSize: '2rem' }}></i>
              <h3 className="product-title mt-2">{t.title}</h3>
              <p className="product-desc">{t.description}</p>
            </div>
          ))}
        </div>
      </section>

      <section>
        <h2 className="section-title">Tez-tez verilən suallar</h2>
        <div className="d-flex flex-column gap-2">
          {faqs.map((f, i) => (
            <div key={i} className="product-card" style={{ cursor: 'pointer' }} onClick={() => setOpen(open === i ? -1 : i)}>
              <h3 className="product-title">{f.question} <i className={`bi bi-chevron-${open === i ? 'up' : 'down'}`}></i></h3>
              {open === i && <p className="product-desc">{f.answer}</p>}
            </div>
          ))}
          {faqs.length === 0 && <p className="text-muted">Nəticə tapılmadı.</p>}
        </div>
      </section>

      <section className="text-center">
        <h4>Cavab tapmadınız?</h4>
        <p className="text-muted">Bizimlə birbaşa əlaqə saxlayın, məmnuniyyətlə kömək edərik.</p>
        <Link to="/elaqe" className="btn-outline text-decoration-none">Əlaqə</Link>
      </section>
    </div>
  );
}

export default Yardim;
