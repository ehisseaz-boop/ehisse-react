import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getFavorites, toggleFavorite } from '../utils/favorites';
import { imageSrc } from '../api';
import { usePageSeo } from '../utils/seo';

export default function Favorites() {
  usePageSeo({
    title: 'Seçilmişlər',
    description: 'Yadda saxladığınız və bəyəndiyiniz ehtiyat hissələri siyahısı.'
  });

  const [items, setItems] = useState([]);

  const loadFavorites = () => {
    setItems(getFavorites());
  };

  useEffect(() => {
    loadFavorites();
    const handleUpdate = () => loadFavorites();
    window.addEventListener('favorites-updated', handleUpdate);
    return () => window.removeEventListener('favorites-updated', handleUpdate);
  }, []);

  const handleRemove = (e, part) => {
    e.preventDefault();
    e.stopPropagation();
    toggleFavorite(part);
  };

  return (
    <div className="container py-4">
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h2 className="section-title mb-1">
            <i className="bi bi-heart-fill text-danger me-2"></i>Seçilmişlər
          </h2>
          <p className="text-muted m-0">
            {items.length > 0 
              ? `Yadda saxladığınız ${items.length} elan` 
              : 'Hələ heç bir elanı sevimlilərə əlavə etməmisiniz'}
          </p>
        </div>
        <Link to="/" className="btn btn-outline">
          <i className="bi bi-arrow-left me-1"></i> Kataloqa qayıt
        </Link>
      </div>

      {items.length === 0 ? (
        <div 
          className="text-center py-5 rounded" 
          style={{ background: 'var(--bg-surface)', border: '1px dashed var(--border-color)', margin: '2rem 0' }}
        >
          <div style={{ fontSize: '3.5rem', color: 'var(--text-muted)' }}>
            <i className="bi bi-heart"></i>
          </div>
          <h4 className="mt-3 text-main">Seçilmiş elan yoxdur</h4>
          <p className="text-muted small mb-4">
            Bəyəndiyiniz elanların üzərindəki ürək ikonuna basaraq onları bura əlavə edə bilərsiniz.
          </p>
          <Link to="/" className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill">
            Elanlara bax
          </Link>
        </div>
      ) : (
        <div className="products-grid">
          {items.map(part => {
            const mainImg = (part.images || []).find(i => i.isMain) || (part.images || [])[0];
            const imgSrc = mainImg ? imageSrc(mainImg.imageUrl || mainImg.url) : null;

            return (
              <div key={part.id} className="product-card position-relative">
                <div className="product-card-top">
                  <div></div>
                  <button 
                    type="button" 
                    className="btn-fav active" 
                    title="Sevimlilərdən sil"
                    onClick={(e) => handleRemove(e, part)}
                  >
                    <i className="bi bi-heart-fill text-danger"></i>
                  </button>
                </div>

                <Link to={`/details/${part.id}`} className="product-image text-decoration-none">
                  {imgSrc ? (
                    <img src={imgSrc} alt={part.title} loading="lazy" />
                  ) : (
                    <div style={{ width: '80px', height: '80px', border: '4px solid var(--accent)', borderRadius: '50%', boxShadow: '0 0 20px var(--accent)', opacity: 0.8 }}></div>
                  )}
                </Link>

                <div className="product-meta">
                  <span className="product-category">{part.partTypeName || "HİSSƏ"}</span>
                  <span className="product-stock">Stokda {part.stockQuantity} ədəd</span>
                </div>

                <Link to={`/details/${part.id}`} className="text-decoration-none text-main">
                  <h3 className="product-title">{part.title}</h3>
                </Link>

                <div className="product-condition">
                  {part.qualityTypeName || 'Orijinal'}
                </div>

                <div className="product-bottom">
                  <div>
                    <div className="product-price">{Math.floor(part.price)} <span>AZN</span></div>
                    {part.sellerName && (
                      <div className="product-seller">
                        <i className="bi bi-shop"></i> {part.sellerName}
                      </div>
                    )}
                  </div>
                  <Link to={`/details/${part.id}`} className="btn-cart text-decoration-none">
                    <i className="bi bi-arrow-right"></i>
                  </Link>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
