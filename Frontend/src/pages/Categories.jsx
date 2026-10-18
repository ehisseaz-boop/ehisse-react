import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../api';
import { usePageSeo } from '../utils/seo';

export function Loading() {
  return (
    <div className="text-center w-100 py-5">
      <div className="spinner-border text-accent" role="status"><span className="visually-hidden">Yüklənir...</span></div>
    </div>
  );
}

export function ErrorBox({ message }) {
  return <div className="text-center py-5" style={{ color: '#ff6b6b' }}>{message}</div>;
}

function Categories() {
  usePageSeo({
    title: 'Kateqoriyalar',
    description: 'Mobil telefon, planşet və noutbuk ehtiyat hissələri kateqoriyaları.'
  });

  const [items, setItems] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    Api.allCategories().then(setItems).catch(e => setError(e.message));
  }, []);

  return (
    <div className="container">
      <section>
        <div className="section-header">
          <div>
            <div className="section-label">Kateqoriyalar</div>
            <h2 className="section-title">Bütün kateqoriyalar</h2>
          </div>
        </div>
        {error ? <ErrorBox message={error} /> : !items ? <Loading /> : (
          <div className="category-grid">
            {items.filter(c => c.isActive).map(cat => (
              <Link key={cat.id} to={`/?categoryId=${cat.id}`} className="category-card">
                <div className="category-icon"><i className="bi bi-box"></i></div>
                <div className="category-info">
                  <h3>{cat.name.replace(' ehtiyat hissələri', '')}</h3>
                  <p>{cat.count} elan</p>
                </div>
              </Link>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

export default Categories;
