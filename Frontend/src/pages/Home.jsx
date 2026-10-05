import React, { useState, useEffect } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Api, imageSrc } from '../api';

const isToday = (dateString) => {
    if (!dateString) return false;
    const date = new Date(dateString);
    const today = new Date();
    return date.getDate() === today.getDate() &&
           date.getMonth() === today.getMonth() &&
           date.getFullYear() === today.getFullYear();
};

function Home() {
  const [searchParams] = useSearchParams();
  const initialCategory = searchParams.get('categoryId') ? Number(searchParams.get('categoryId')) : null;
  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [activeCategoryId, setActiveCategoryId] = useState(initialCategory);
  
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  const fetchProducts = (search = '', categoryId = null, p = page, pSize = pageSize) => {
    setLoading(true);
    Api.catalog({ searchString: search, categoryId, pageNumber: p, pageSize: pSize })
      .then(data => {
        if (data) {
          setProducts(data.items || []);
          setTotalPages(data.totalPages || 1);
          setTotalItems(data.totalItems || 0);
        } else {
          setProducts([]);
        }
      })
      .catch(err => { console.error("Error fetching products:", err); setProducts([]); })
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    Api.categories()
      .then(data => setCategories(data || []))
      .catch(err => console.error("Error fetching categories:", err));

    setActiveCategoryId(initialCategory);
    fetchProducts('', initialCategory);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialCategory]);

  const getCategoryIcon = (name) => {
      if (name.includes('Telefon')) return 'bi-phone';
      if (name.includes('Planşet')) return 'bi-tablet';
      if (name.includes('Noutbuk')) return 'bi-laptop';
      if (name.includes('Saat')) return 'bi-smartwatch';
      if (name.includes('Qulaqlıq')) return 'bi-earbuds';
      if (name.includes('Televizor')) return 'bi-tv';
      if (name.includes('Alətlər')) return 'bi-tools';
      return 'bi-box';
  }

  return (
    <div className="container">
      <section className="hero-section text-center" style={{ justifyContent: 'center' }}>
        <div className="hero-content" style={{ width: '100%', maxWidth: '900px', margin: '0 auto', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <div className="hero-subtitle" style={{ justifyContent: 'center' }}>Ustalar və anbarlar üçün</div>
          <h1 className="hero-title">Doğru hissəni<br/><span className="text-accent">tez tap.</span></h1>
          <p className="hero-desc">Telefon, planşet və noutbuk ehtiyat hissələri.<br/>Yoxlanılmış satıcılardan, real stokla.</p>
          
          <form className="w-100" style={{ maxWidth: '900px' }} onSubmit={(e) => { e.preventDefault(); fetchProducts(searchTerm, activeCategoryId); }}>
            <div className="search-box mb-2 d-flex align-items-center" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)', borderRadius: '50px', padding: '0.5rem 1rem' }}>
              <i className="bi bi-search text-muted me-2 flex-shrink-0"></i>
              <input type="text" value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} placeholder="Telefon,Tablet ve s.." style={{ flex: 1, background: 'transparent', border: 'none', color: 'var(--text-main)', outline: 'none', minWidth: 0, textOverflow: 'ellipsis' }} />
              
              <button type="button" className="btn p-0 position-relative ms-2 flex-shrink-0" data-bs-toggle="modal" data-bs-target="#detailedSearchModal" style={{ border: 'none', outline: 'none' }}>
                <i className="bi bi-sliders text-accent" style={{ fontSize: '1.2rem' }}></i>
              </button>
              
              <button type="submit" className="btn btn-sm btn-primary bg-accent text-dark fw-bold px-3 ms-2 ms-md-3 rounded-pill flex-shrink-0">Axtar</button>
            </div>
          </form>
          
          <div className="hero-stats justify-content-center w-100 mt-3">
            <span>{products.length}+</span> aktiv hissə
            <span className="dot">•</span>
            <span>10+</span> təsdiqli satıcı
            <span className="dot">•</span>
            <span>Bu gün</span> yenilənib
          </div>
        </div>
      </section>

      <section>
        <div className="section-header">
          <div>
            <div className="section-label">Kateqoriya</div>
            <h2 className="section-title">Avadanlığını seç</h2>
          </div>
          <Link to="/categories" className="link-accent">Hamısına bax <i className="bi bi-arrow-right"></i></Link>
        </div>
        
        <div className="category-grid">
          {categories.slice(0, 4).map((cat, index) => {
            const isActive = activeCategoryId === cat.id;
            return (
            <a key={cat.id} href={`/?categoryId=${cat.id}`} onClick={(e) => { e.preventDefault(); const newId = activeCategoryId === cat.id ? null : cat.id; setActiveCategoryId(newId); fetchProducts(searchTerm, newId); }} className={`category-card ${isActive ? 'active' : ''}`}>
              <div className="category-icon" style={{ color: isActive ? 'var(--accent)' : '#fff', background: isActive ? 'transparent' : 'rgba(255,255,255,0.05)' }}>
                <i className={`bi ${getCategoryIcon(cat.name)}`}></i>
              </div>
              <div className="category-info">
                <h3>{cat.name.replace(" ehtiyat hissələri", "")}</h3>
                <p>Kəşf et</p>
              </div>
              <i className="bi bi-arrow-up-right" style={{ position: 'absolute', top: '1.5rem', right: '1.5rem', color: 'var(--text-muted)' }}></i>
            </a>
          )})}
        </div>
      </section>

      <section>
        <div className="section-header" style={{ alignItems: 'center' }}>
          <div>
            <div className="section-label">
              {searchTerm || activeCategoryId ? 'Axtarış nəticələri' : 'Anbarda olanlar'}
            </div>
            <h2 className="section-title">
              {searchTerm || activeCategoryId ? 'Tapılan hissələr' : 'Populyar hissələr'}
            </h2>
          </div>
          <div style={{ display: 'flex', gap: '0.5rem' }}>
            <button className="btn-outline" style={{ padding: '0.5rem', borderColor: 'var(--text-muted)' }}><i className="bi bi-grid-fill text-accent"></i></button>
            <button className="btn-outline" style={{ padding: '0.5rem' }}><i className="bi bi-list"></i></button>
          </div>
        </div>

        <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
          <div className="d-flex gap-2 overflow-auto hide-scrollbar pb-1 w-100">
            <button className="btn-outline" style={{ borderRadius: '20px', fontSize: '0.85rem' }}><i className="bi bi-sliders"></i> Filtrlər</button>
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm(''); fetchProducts('', activeCategoryId); }} className="btn-outline bg-accent text-main text-decoration-none" style={{ borderRadius: '20px', fontSize: '0.85rem', border: 'none', padding: '0.5rem 1.5rem', color: '#000' }}>Hamisi</a>
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm('Ekran'); fetchProducts('Ekran', activeCategoryId); }} className="btn-outline text-main text-decoration-none" style={{ borderRadius: '20px', fontSize: '0.85rem' }}>Ekran</a>
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm('Batareya'); fetchProducts('Batareya', activeCategoryId); }} className="btn-outline text-main text-decoration-none" style={{ borderRadius: '20px', fontSize: '0.85rem' }}>Batareya</a>
          </div>
          <div id="sort-dropdown-container" style={{ fontSize: '0.85rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
            Sırala: <strong style={{ color: 'var(--text-main)' }}>Ən çox baxılan</strong> <i className="bi bi-chevron-down"></i>
          </div>
        </div>

        <div className="products-grid">
            {loading ? (
                <div className="text-center w-100 py-5">
                    <div className="spinner-border text-accent" role="status">
                        <span className="visually-hidden">Yüklənir...</span>
                    </div>
                </div>
            ) : products.length > 0 ? (
                products.map(part => {
                    const mainImage = (part.images || []).find(img => img.isMain) || (part.images || [])[0];
                    const isNew = isToday(part.createdDate);
                    return (
                        <div key={part.id} className="product-card">
                            <div className="product-card-top">
                                <div>
                                    {part.viewCount > 50 && <span className="badge-accent" style={{ marginBottom: '0.5rem', marginRight: '0.5rem' }}>Tövsiyə</span>}
                                    {isNew && <span className="badge-accent" style={{ marginBottom: '0.5rem', backgroundColor: '#28a745', color: '#fff' }}>Yeni</span>}
                                </div>
                                <button className="btn-fav add-to-fav" data-id={part.id}><i className="bi bi-heart"></i></button>
                            </div>
                            
                            <Link to={`/details/${part.id}`} className="product-image text-decoration-none">
                                {mainImage ? (
                                    <img src={imageSrc(mainImage.url)} alt={part.title} />
                                ) : (
                                    <div style={{ width: '80px', height: '80px', border: '4px solid var(--accent)', borderRadius: '50%', boxShadow: '0 0 20px var(--accent)', opacity: 0.8 }}></div>
                                )}
                            </Link>
                            
                            <div className="product-meta">
                                <span className="product-category">{part.partTypeName || "DIGƏR"}</span>
                                <span className="product-stock">Stokda {part.stockQuantity} ədəd</span>
                            </div>
                            
                            <Link to={`/details/${part.id}`} className="text-decoration-none text-main"><h3 className="product-title">{part.brandName} {part.modelName} {part.title}</h3></Link>
                            <p className="product-desc">{part.modelName} - {part.qualityTypeName}</p>
                            
                            <div className="product-condition">
                                {part.qualityTypeName}
                            </div>
                            
                            <div className="product-bottom">
                                <div>
                                    <div className="product-price">{Math.floor(part.price)} <span>AZN</span></div>
                                    <div className="product-seller">
                                        <i className="bi bi-shop"></i> {part.sellerName}
                                    </div>
                                </div>
                                <Link to={`/details/${part.id}`} className="btn-cart text-decoration-none"><i className="bi bi-arrow-right"></i></Link>
                            </div>
                        </div>
                    );
                })
            ) : (
                <div className="text-center py-5" style={{ width: '100%', margin: '2rem 0', border: '1px dashed rgba(255,255,255,0.1)', borderRadius: '10px' }}>
                    <i className="bi bi-search text-muted" style={{ fontSize: '3rem' }}></i>
                    <h3 className="mt-3 text-main">Nəticə tapılmadı</h3>
                </div>
            )}
        </div>
        
        {/* Pagination UI */}
        {!loading && totalPages > 1 && (
            <div className="d-flex justify-content-between align-items-center mt-4">
              <div>
                <select 
                  className="form-select form-select-sm" 
                  value={pageSize} 
                  onChange={(e) => {
                    const newSize = Number(e.target.value);
                    setPageSize(newSize);
                    setPage(1);
                    fetchProducts(searchTerm, activeCategoryId, 1, newSize);
                  }}
                  style={{ width: 'auto', display: 'inline-block' }}
                >
                  <option value="10">10 / səhifə</option>
                  <option value="20">20 / səhifə</option>
                  <option value="50">50 / səhifə</option>
                </select>
              </div>
              
              <ul className="pagination pagination-sm mb-0">
                <li className={`page-item ${page === 1 ? 'disabled' : ''}`}>
                  <button className="page-link" onClick={() => {
                    const prev = Math.max(1, page - 1);
                    setPage(prev);
                    fetchProducts(searchTerm, activeCategoryId, prev, pageSize);
                  }}>Əvvəlki</button>
                </li>
                <li className="page-item disabled">
                  <span className="page-link">{page} / {totalPages}</span>
                </li>
                <li className={`page-item ${page === totalPages ? 'disabled' : ''}`}>
                  <button className="page-link" onClick={() => {
                    const next = Math.min(totalPages, page + 1);
                    setPage(next);
                    fetchProducts(searchTerm, activeCategoryId, next, pageSize);
                  }}>Növbəti</button>
                </li>
              </ul>
            </div>
        )}
      </section>
    </div>
  );
}

export default Home;
