import React, { useState, useEffect, useRef } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Api, imageSrc } from '../api';
import { Modal } from '../components/Panel';
import { usePageSeo } from '../utils/seo';
import VisualSearchModal, { getCategoryIcon } from '../components/VisualSearchModal';
import { toggleFavorite, isFavorite } from '../utils/favorites';

const isToday = (dateString) => {
    if (!dateString) return false;
    const date = new Date(dateString);
    const today = new Date();
    return date.getDate() === today.getDate() &&
           date.getMonth() === today.getMonth() &&
           date.getFullYear() === today.getFullYear();
};

export function cleanPartTypeName(name) {
  if (!name) return '';
  let clean = name.trim();
  // "Telefon üçün ekranlar" -> "Ekranlar"
  clean = clean.replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar');
  clean = clean.replace(/telefon\s+üçün\s+ekran/gi, 'Ekranlar');
  // "Telefon üçün batareyalar" -> "Batareyalar"
  clean = clean.replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar');
  clean = clean.replace(/telefon\s+üçün\s+batareya/gi, 'Batareyalar');
  // "Telefon üçün sensorlar və ekran şüşələri" -> "Sensor və ekran şüşələri"
  clean = clean.replace(/telefon\s+üçün\s+sensorlar/gi, 'Sensorlar');
  clean = clean.replace(/telefon\s+üçün\s+/gi, '');
  return clean;
}

function formatCleanTitle(part) {
  let model = (part.modelName || '').trim();
  let brand = (part.brandName || '').trim();
  let partType = cleanPartTypeName(part.partTypeName || '');

  // Əgər model adı artıq brendlə başlamırsa (məs: "15 Pro" -> "iPhone 15 Pro")
  let fullModel = model;
  if (brand && !model.toLowerCase().startsWith(brand.toLowerCase())) {
    fullModel = `${brand} ${model}`;
  }

  // Təmiz ad formalaşdırırıq: məs. "iPhone 16 Pro Max Batareyalar"
  if (fullModel && partType) {
    return `${fullModel} ${partType}`;
  }
  let fallback = part.title || fullModel || 'Ehtiyat hissəsi';
  return fallback.replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar')
                 .replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar')
                 .replace(/telefon\s+üçün\s+/gi, '');
}

function ProductCardImageSlider({ part }) {
  const images = (part.images && part.images.length > 0) ? part.images : [];
  const [currentIdx, setCurrentIdx] = useState(0);

  if (images.length === 0) {
    return (
      <Link to={`/details/${part.id}`} className="product-image text-decoration-none">
        <div style={{ width: '80px', height: '80px', border: '4px solid var(--accent)', borderRadius: '50%', boxShadow: '0 0 20px var(--accent)', opacity: 0.8 }}></div>
      </Link>
    );
  }

  const nextImg = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIdx((prev) => (prev + 1) % images.length);
  };

  const prevImg = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIdx((prev) => (prev - 1 + images.length) % images.length);
  };

  const currentImg = images[currentIdx] || images[0];

  return (
    <div className="product-image position-relative overflow-hidden group">
      <Link to={`/details/${part.id}`} className="w-100 h-100 d-flex align-items-center justify-content-center text-decoration-none">
        <img 
          src={imageSrc(currentImg.url)} 
          alt={`${formatCleanTitle(part)} - ehtiyat hissəsi e-hisse.az`} 
          loading="lazy" 
          decoding="async" 
        />
      </Link>

      {images.length > 1 && (
        <>
          <button
            type="button"
            onClick={prevImg}
            className="btn btn-sm btn-dark position-absolute top-50 start-0 translate-middle-y shadow-sm p-0 d-flex align-items-center justify-content-center"
            style={{
              width: '24px',
              height: '24px',
              borderRadius: '50%',
              opacity: 0.85,
              zIndex: 3,
              marginLeft: '4px',
              backgroundColor: 'rgba(0,0,0,0.65)',
              border: 'none',
              color: '#fff'
            }}
            title="Əvvəlki şəkil"
          >
            <i className="bi bi-chevron-left" style={{ fontSize: '12px' }}></i>
          </button>

          <button
            type="button"
            onClick={nextImg}
            className="btn btn-sm btn-dark position-absolute top-50 end-0 translate-middle-y shadow-sm p-0 d-flex align-items-center justify-content-center"
            style={{
              width: '24px',
              height: '24px',
              borderRadius: '50%',
              opacity: 0.85,
              zIndex: 3,
              marginRight: '4px',
              backgroundColor: 'rgba(0,0,0,0.65)',
              border: 'none',
              color: '#fff'
            }}
            title="Növbəti şəkil"
          >
            <i className="bi bi-chevron-right" style={{ fontSize: '12px' }}></i>
          </button>

          <div
            className="position-absolute bottom-0 start-50 translate-middle-x d-flex gap-1 mb-1"
            style={{ zIndex: 3 }}
          >
            {images.map((_, idx) => (
              <span
                key={idx}
                style={{
                  width: idx === currentIdx ? '10px' : '5px',
                  height: '5px',
                  borderRadius: '3px',
                  backgroundColor: idx === currentIdx ? 'var(--accent)' : 'rgba(255,255,255,0.4)',
                  transition: 'all 0.2s'
                }}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
}

function Home() {
  usePageSeo({
    title: 'Doğru hissəni tez tap - Ehtiyat hissələri kataloqu',
    description: 'Telefon, planşet və noutbuk ehtiyat hissələri. Yoxlanılmış satıcılardan, real stokla ekran, batareya, korpus və aksesuarlar.'
  });

  const [searchParams, setSearchParams] = useSearchParams();
  const initialCategory = searchParams.get('categoryId') ? Number(searchParams.get('categoryId')) : null;
  const initialSeller = searchParams.get('sellerId') ? Number(searchParams.get('sellerId')) : null;
  const initialSort = searchParams.get('sort') || sessionStorage.getItem('home_sortOrder') || '';
  const [categories, setCategories] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [activeCategoryId, setActiveCategoryId] = useState(initialCategory);
  const [activeSellerId, setActiveSellerId] = useState(initialSeller);
  const [, setFavSync] = useState(0);

  useEffect(() => {
    const handleFavChange = () => setFavSync(prev => prev + 1);
    window.addEventListener('favorites-updated', handleFavChange);
    return () => window.removeEventListener('favorites-updated', handleFavChange);
  }, []);

  // Detallı axtarış modalı və filtrlər
  const [showFilterModal, setShowFilterModal] = useState(false);
  const [showVisualCatalog, setShowVisualCatalog] = useState(false);
  const [filterBrandId, setFilterBrandId] = useState('');
  const [filterModelId, setFilterModelId] = useState('');
  const [filterPartTypeId, setFilterPartTypeId] = useState('');
  const [filterQualityTypeId, setFilterQualityTypeId] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [sortOrder, setSortOrder] = useState(initialSort);

  const handleSortChange = (newSort) => {
    setSortOrder(newSort);
    sessionStorage.setItem('home_sortOrder', newSort);
    const newParams = new URLSearchParams(searchParams);
    if (newSort) {
      newParams.set('sort', newSort);
    } else {
      newParams.delete('sort');
    }
    setSearchParams(newParams);
    setPage(1);
    fetchProducts(searchTerm, activeCategoryId, activeSellerId, 1, pageSize, { sortOrder: newSort });
  };

  const openFilterModal = () => {
    if (!activeCategoryId && categories.length > 0) {
      const phoneCat = categories.find(c => c.name.toLowerCase().includes('telefon')) || categories[0];
      if (phoneCat) setActiveCategoryId(phoneCat.id);
    }
    setShowFilterModal(true);
  };

  const handleVisualCatalogSelect = ({ categoryId, brandId, modelId, partTypeId }) => {
    setActiveCategoryId(categoryId);
    setFilterBrandId(brandId ? String(brandId) : '');
    setFilterModelId(modelId ? String(modelId) : '');
    setFilterPartTypeId(partTypeId ? String(partTypeId) : '');
    setPage(1);

    const newParams = new URLSearchParams(searchParams);
    if (categoryId) newParams.set('categoryId', categoryId);
    else newParams.delete('categoryId');
    setSearchParams(newParams);

    fetchProducts(searchTerm, categoryId, activeSellerId, 1, pageSize, {
      brandId: brandId ? Number(brandId) : undefined,
      modelId: modelId ? Number(modelId) : undefined,
      partTypeId: partTypeId ? Number(partTypeId) : undefined
    });

    // Səhifənin elanlar hissəsinə sürüşdür
    const section = document.getElementById('catalog-section');
    if (section) section.scrollIntoView({ behavior: 'smooth' });
  };

  // Dropdown məlumatları
  const [brands, setBrands] = useState([]);
  const [models, setModels] = useState([]);
  const [partTypes, setPartTypes] = useState([]);
  const [qualityTypes, setQualityTypes] = useState([]);
  
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  const categoryScrollRef = useRef(null);

  const scrollCategories = (direction) => {
    if (categoryScrollRef.current) {
      const scrollAmount = direction === 'left' ? -260 : 260;
      categoryScrollRef.current.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    }
  };

  const fetchProducts = (
    search = searchTerm,
    categoryId = activeCategoryId,
    sellerId = activeSellerId,
    p = page,
    pSize = pageSize,
    extra = {}
  ) => {
    setLoading(true);
    const bId = extra.brandId !== undefined ? extra.brandId : filterBrandId;
    const mId = extra.modelId !== undefined ? extra.modelId : filterModelId;
    const ptId = extra.partTypeId !== undefined ? extra.partTypeId : filterPartTypeId;
    const qtId = extra.qualityTypeId !== undefined ? extra.qualityTypeId : filterQualityTypeId;
    const mnP = extra.minPrice !== undefined ? extra.minPrice : minPrice;
    const mxP = extra.maxPrice !== undefined ? extra.maxPrice : maxPrice;
    const sOrder = extra.sortOrder !== undefined ? extra.sortOrder : sortOrder;

    Api.catalog({
      searchString: search,
      categoryId: categoryId || undefined,
      sellerId: sellerId || undefined,
      brandId: bId ? Number(bId) : undefined,
      modelId: mId ? Number(mId) : undefined,
      partTypeId: ptId ? Number(ptId) : undefined,
      qualityTypeId: qtId ? Number(qtId) : undefined,
      minPrice: mnP ? Number(mnP) : undefined,
      maxPrice: mxP ? Number(mxP) : undefined,
      sortOrder: sOrder || undefined,
      pageNumber: p,
      pageSize: pSize
    })
      .then(data => {
        if (data) {
          let items = data.items || [];
          if (sOrder === 'views_desc') {
            items = [...items].sort((a, b) => (b.viewCount || 0) - (a.viewCount || 0));
          } else if (sOrder === 'price_asc') {
            items = [...items].sort((a, b) => (a.price || 0) - (b.price || 0));
          } else if (sOrder === 'price_desc') {
            items = [...items].sort((a, b) => (b.price || 0) - (a.price || 0));
          }
          setProducts(items);
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

    Api.qualityTypes()
      .then(data => setQualityTypes(data || []))
      .catch(() => setQualityTypes([]));

    setActiveCategoryId(initialCategory);
    setActiveSellerId(initialSeller);
    fetchProducts('', initialCategory, initialSeller, 1, pageSize, { sortOrder: initialSort });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialCategory, initialSeller]);

  // Modal daxilində kateqoriya dəyişəndə brendlər və hissə növlərini çək
  useEffect(() => {
    if (!activeCategoryId) {
      setBrands([]);
      setPartTypes([]);
      setModels([]);
      return;
    }
    Api.brandsByCategory(activeCategoryId).then(setBrands).catch(() => setBrands([]));
    Api.partTypesByCategory(activeCategoryId).then(setPartTypes).catch(() => setPartTypes([]));
  }, [activeCategoryId]);

  // Modal daxilində brend dəyişəndə modelləri çək
  useEffect(() => {
    if (!activeCategoryId || !filterBrandId) {
      setModels([]);
      return;
    }
    Api.modelsByBrand(activeCategoryId, filterBrandId).then(setModels).catch(() => setModels([]));
  }, [activeCategoryId, filterBrandId]);

  const handleApplyFilter = (e) => {
    if (e) e.preventDefault();
    setShowFilterModal(false);
    setPage(1);
    fetchProducts(searchTerm, activeCategoryId, activeSellerId, 1, pageSize);
  };

  const handleResetFilter = () => {
    setActiveCategoryId(null);
    setFilterBrandId('');
    setFilterModelId('');
    setFilterPartTypeId('');
    setFilterQualityTypeId('');
    setMinPrice('');
    setMaxPrice('');
    setSortOrder('');
    sessionStorage.removeItem('home_sortOrder');
    setShowFilterModal(false);
    setPage(1);
    setSearchParams({});
    fetchProducts(searchTerm, null, activeSellerId, 1, pageSize, {
      brandId: '', modelId: '', partTypeId: '', qualityTypeId: '', minPrice: '', maxPrice: '', sortOrder: ''
    });
  };

  const hasActiveFilters = Boolean(
    activeCategoryId || filterBrandId || filterModelId || filterPartTypeId || filterQualityTypeId || minPrice || maxPrice
  );

  return (
    <div className="container">
      <section className="hero-section text-center" style={{ justifyContent: 'center' }}>
        <div className="hero-content" style={{ width: '100%', maxWidth: '900px', margin: '0 auto', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <div className="hero-subtitle" style={{ justifyContent: 'center' }}>Ustalar və anbarlar üçün</div>
          <h1 className="hero-title">Doğru hissəni<br/><span className="text-accent">tez tap.</span></h1>
          <p className="hero-desc">Telefon, planşet və noutbuk ehtiyat hissələri.<br/>Yoxlanılmış satıcılardan, real stokla.</p>
          
          <form className="w-100" style={{ maxWidth: '900px' }} onSubmit={(e) => { e.preventDefault(); setPage(1); fetchProducts(searchTerm, activeCategoryId, activeSellerId, 1, pageSize); }}>
            <div className="search-box mb-2 d-flex align-items-center" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)', borderRadius: '50px', padding: '0.5rem 1rem' }}>
              <i className="bi bi-search text-muted me-2 flex-shrink-0"></i>
              <input type="text" value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} placeholder="Brend, model və ya detal adı yazın (məsələn, iPhone 16 ekran)..." style={{ flex: 1, background: 'transparent', border: 'none', color: 'var(--text-main)', outline: 'none', minWidth: 0, textOverflow: 'ellipsis' }} />
              
              <button 
                type="button" 
                className="btn p-0 position-relative ms-2 flex-shrink-0" 
                onClick={openFilterModal}
                title="Detallı axtarış və filtrlər"
                style={{ border: 'none', outline: 'none' }}
              >
                <i className="bi bi-sliders text-accent" style={{ fontSize: '1.25rem' }}></i>
                {hasActiveFilters && (
                  <span className="position-absolute top-0 start-100 translate-middle p-1 bg-danger border border-light rounded-circle" style={{ width: '8px', height: '8px' }}></span>
                )}
              </button>
              
              <button type="submit" className="btn btn-sm btn-primary bg-accent text-dark fw-bold px-3 ms-2 ms-md-3 rounded-pill flex-shrink-0">Axtar</button>
            </div>
          </form>

          {/* İlk ekranda CTA Butonları */}
          <div className="d-flex flex-wrap justify-content-center gap-3 my-3">
            <button 
              type="button"
              onClick={() => setShowVisualCatalog(true)} 
              className="btn btn-primary bg-accent text-dark fw-bold px-4 py-2 rounded-pill shadow-sm"
            >
              <i className="bi bi-grid-fill me-2"></i> Kataloq
            </button>
            <button 
              type="button"
              onClick={openFilterModal} 
              className="btn btn-outline px-4 py-2 rounded-pill"
              style={{ borderColor: 'var(--border-color)', color: 'var(--text-main)' }}
            >
              <i className="bi bi-sliders me-2 text-accent"></i> Filtrlər
            </button>
            <Link 
              to="/warehouse/add-part" 
              className="btn btn-outline px-4 py-2 rounded-pill text-decoration-none"
              style={{ borderColor: 'var(--border-color)', color: 'var(--text-main)' }}
            >
              <i className="bi bi-plus-circle me-2 text-accent"></i> Elan yerləşdir
            </Link>
          </div>
          
          <div className="hero-stats justify-content-center w-100 mt-2 mb-3">
            <span>{totalItems || products.length}+</span> aktiv hissə
            <span className="dot">•</span>
            <span>10+</span> təsdiqli satıcı
            <span className="dot">•</span>
            <span>Bu gün</span> yenilənib
          </div>

          {/* Vizual Kateqoriya İkonları Karuseli (Tap.az üslubunda) */}
          <div className="w-100 mt-2">
            <div className="categories-carousel-wrapper">
              <button 
                type="button" 
                className="carousel-nav-btn prev-btn d-none d-md-flex" 
                onClick={() => scrollCategories('left')}
                title="Geri"
              >
                <i className="bi bi-chevron-left"></i>
              </button>

              <div className="home-visual-categories" ref={categoryScrollRef}>
                {/* Kataloq açan düymə */}
                <div 
                  className="home-cat-item katalog-btn"
                  onClick={() => setShowVisualCatalog(true)}
                >
                  <div className="home-cat-icon">
                    <i className="bi bi-grid-fill"></i>
                  </div>
                  <span className="home-cat-title">Kataloq</span>
                </div>

                {/* Kateqoriyalar */}
                {categories.map(cat => {
                  const isActive = activeCategoryId === cat.id;
                  return (
                    <div 
                      key={cat.id}
                      className={`home-cat-item ${isActive ? 'active' : ''}`}
                      onClick={() => {
                        if (isActive) {
                          setActiveCategoryId(null);
                          fetchProducts(searchTerm, null, activeSellerId, 1, pageSize);
                        } else {
                          setActiveCategoryId(cat.id);
                          fetchProducts(searchTerm, cat.id, activeSellerId, 1, pageSize);
                          const section = document.getElementById('catalog-section');
                          if (section) section.scrollIntoView({ behavior: 'smooth' });
                        }
                      }}
                    >
                      <div className="home-cat-icon">
                        <i className={`bi ${getCategoryIcon(cat.name)}`}></i>
                      </div>
                      <span className="home-cat-title">{cat.name.replace(' ehtiyat hissələri', '')}</span>
                    </div>
                  );
                })}
              </div>

              <button 
                type="button" 
                className="carousel-nav-btn next-btn d-none d-md-flex" 
                onClick={() => scrollCategories('right')}
                title="İrəli"
              >
                <i className="bi bi-chevron-right"></i>
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Tap.az Üslubunda Vizual Axtarış və Kataloq Modalı */}
      <VisualSearchModal
        open={showVisualCatalog}
        onClose={() => setShowVisualCatalog(false)}
        categories={categories}
        onSelectResult={handleVisualCatalogSelect}
      />

      {/* Detallı Axtarış Modalı */}
      <Modal
        open={showFilterModal}
        title="Detallı axtarış və filtrlər"
        onClose={() => setShowFilterModal(false)}
        footer={
          <div className="d-flex justify-content-between w-100">
            <button type="button" className="btn btn-outline" onClick={handleResetFilter}>
              <i className="bi bi-arrow-counterclockwise me-1"></i> Sıfırla
            </button>
            <div className="d-flex gap-2">
              <button type="button" className="btn btn-outline" onClick={() => setShowFilterModal(false)}>Bağla</button>
              <button type="button" className="btn btn-primary bg-accent text-dark fw-bold px-4" onClick={handleApplyFilter}>
                Tətbiq et
              </button>
            </div>
          </div>
        }
      >
        <div className="row g-3">
          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Kateqoriya</label>
            <select
              className="form-select"
              value={activeCategoryId || ''}
              onChange={(e) => {
                const val = e.target.value ? Number(e.target.value) : null;
                setActiveCategoryId(val);
                setFilterBrandId('');
                setFilterModelId('');
                setFilterPartTypeId('');
              }}
            >
              <option value="">Bütün kateqoriyalar</option>
              {categories.map(c => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Brend / Marka</label>
            <select
              className="form-select"
              value={filterBrandId}
              onChange={(e) => {
                setFilterBrandId(e.target.value);
                setFilterModelId('');
              }}
              disabled={!activeCategoryId || brands.length === 0}
            >
              <option value="">{activeCategoryId ? 'Bütün brendlər' : 'Əvvəlcə kateqoriya seçin'}</option>
              {brands.map(b => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Model</label>
            <select
              className="form-select"
              value={filterModelId}
              onChange={(e) => setFilterModelId(e.target.value)}
              disabled={!filterBrandId || models.length === 0}
            >
              <option value="">{filterBrandId ? 'Bütün modellər' : 'Əvvəlcə brend seçin'}</option>
              {models.map(m => (
                <option key={m.id} value={m.id}>{m.name}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Ehtiyat hissəsi növü</label>
            <select
              className="form-select"
              value={filterPartTypeId}
              onChange={(e) => setFilterPartTypeId(e.target.value)}
              disabled={!activeCategoryId || partTypes.length === 0}
            >
              <option value="">{activeCategoryId ? 'Bütün hissə növləri' : 'Əvvəlcə kateqoriya seçin'}</option>
              {partTypes.map(p => (
                <option key={p.id} value={p.id}>{cleanPartTypeName(p.name)}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Keyfiyyət növü</label>
            <select
              className="form-select"
              value={filterQualityTypeId}
              onChange={(e) => setFilterQualityTypeId(e.target.value)}
            >
              <option value="">Bütün keyfiyyətlər</option>
              {qualityTypes.map(q => (
                <option key={q.id} value={q.id}>{q.name}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Sıralama</label>
            <select
              className="form-select"
              value={sortOrder}
              onChange={(e) => setSortOrder(e.target.value)}
            >
              <option value="">Əvvəlcə ən yeni</option>
              <option value="price_asc">Əvvəlcə ən ucuz</option>
              <option value="price_desc">Əvvəlcə ən baha</option>
              <option value="views_desc">Əvvəlcə ən çox baxılan</option>
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Min. qiymət (AZN)</label>
            <input
              type="number"
              className="form-control"
              placeholder="0"
              min="0"
              value={minPrice}
              onChange={(e) => setMinPrice(e.target.value)}
            />
          </div>

          <div className="col-md-6">
            <label className="form-label text-main fw-semibold">Maks. qiymət (AZN)</label>
            <input
              type="number"
              className="form-control"
              placeholder="1000"
              min="0"
              value={maxPrice}
              onChange={(e) => setMaxPrice(e.target.value)}
            />
          </div>
        </div>
      </Modal>

      <section id="catalog-section">
        {(searchTerm || activeCategoryId) && (
          <div className="section-header" style={{ alignItems: 'center' }}>
            <div>
              <div className="section-label">Axtarış nəticələri</div>
              <h2 className="section-title">Tapılan hissələr</h2>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button className="btn-outline" style={{ padding: '0.5rem', borderColor: 'var(--text-muted)' }}><i className="bi bi-grid-fill text-accent"></i></button>
              <button className="btn-outline" style={{ padding: '0.5rem' }}><i className="bi bi-list"></i></button>
            </div>
          </div>
        )}

        {activeSellerId && (
          <div className="d-flex align-items-center justify-content-between p-2 px-3 rounded mb-3" style={{ background: 'var(--accent-dark)', border: '1px solid var(--accent)' }}>
            <span className="small fw-bold" style={{ color: 'var(--accent)' }}>
              <i className="bi bi-shop me-2"></i>Seçilmiş satıcı/anbarın elanları (Satıcı #{activeSellerId})
            </span>
            <button className="btn btn-sm btn-outline" style={{ borderRadius: '20px', fontSize: '0.8rem', padding: '0.2rem 0.8rem' }} onClick={() => { setActiveSellerId(null); setSearchParams({}); fetchProducts(searchTerm, activeCategoryId, null); }}>
              <i className="bi bi-x-circle me-1"></i> Filtri sıfırla
            </button>
          </div>
        )}

        <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
          <div className="d-flex gap-2 overflow-auto hide-scrollbar pb-1 w-100">
            <button 
              type="button" 
              className={`btn-outline ${hasActiveFilters ? 'border-accent text-accent' : ''}`} 
              style={{ borderRadius: '20px', fontSize: '0.85rem' }}
              onClick={openFilterModal}
            >
              <i className="bi bi-sliders me-1"></i> Filtrlər {hasActiveFilters && '(Aktiv)'}
            </button>
            {hasActiveFilters && (
              <button
                type="button"
                className="btn-outline text-danger"
                style={{ borderRadius: '20px', fontSize: '0.85rem' }}
                onClick={handleResetFilter}
              >
                <i className="bi bi-x-circle me-1"></i> Filtrləri təmizlə
              </button>
            )}
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm(''); setPage(1); fetchProducts('', activeCategoryId, activeSellerId, 1, pageSize); }} className={`btn-outline ${!searchTerm ? 'bg-accent text-dark fw-bold' : 'text-main'} text-decoration-none`} style={{ borderRadius: '20px', fontSize: '0.85rem', padding: '0.5rem 1.5rem' }}>Hamisi</a>
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm('Ekran'); setPage(1); fetchProducts('Ekran', activeCategoryId, activeSellerId, 1, pageSize); }} className={`btn-outline ${searchTerm === 'Ekran' ? 'bg-accent text-dark fw-bold' : 'text-main'} text-decoration-none`} style={{ borderRadius: '20px', fontSize: '0.85rem' }}>Ekran</a>
            <a href="#" onClick={(e) => { e.preventDefault(); setSearchTerm('Batareya'); setPage(1); fetchProducts('Batareya', activeCategoryId, activeSellerId, 1, pageSize); }} className={`btn-outline ${searchTerm === 'Batareya' ? 'bg-accent text-dark fw-bold' : 'text-main'} text-decoration-none`} style={{ borderRadius: '20px', fontSize: '0.85rem' }}>Batareya</a>
          </div>
          <div 
            id="sort-dropdown-container" 
            className="d-flex align-items-center gap-2 flex-shrink-0"
            style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}
          >
            <span>Sırala:</span>
            <select
              className="form-select form-select-sm"
              style={{
                width: 'auto',
                minWidth: '155px',
                borderRadius: '20px',
                backgroundColor: 'var(--input-bg)',
                color: 'var(--text-main)',
                borderColor: 'var(--input-border)',
                fontSize: '0.85rem',
                cursor: 'pointer'
              }}
              value={sortOrder}
              onChange={(e) => handleSortChange(e.target.value)}
            >
              <option value="" style={{ backgroundColor: 'var(--bg-surface)', color: 'var(--text-main)' }}>Əvvəlcə ən yeni</option>
              <option value="price_asc" style={{ backgroundColor: 'var(--bg-surface)', color: 'var(--text-main)' }}>Əvvəlcə ən ucuz</option>
              <option value="price_desc" style={{ backgroundColor: 'var(--bg-surface)', color: 'var(--text-main)' }}>Əvvəlcə ən baha</option>
              <option value="views_desc" style={{ backgroundColor: 'var(--bg-surface)', color: 'var(--text-main)' }}>Əvvəlcə ən çox baxılan</option>
            </select>
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
                                    {isNew && <span className="badge-accent" style={{ marginBottom: '0.5rem', backgroundColor: '#28a745', color: '#fff' }}>Yeni</span>}
                                </div>
                                <button 
                                    type="button"
                                    className={`btn-fav add-to-fav ${isFavorite(part.id) ? 'active' : ''}`}
                                    onClick={(e) => {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        toggleFavorite(part);
                                    }}
                                    title={isFavorite(part.id) ? "Sevimlilərdən çıxar" : "Sevimlilərə əlavə et"}
                                    aria-label="Sevimlilərə əlavə et"
                                >
                                    <i className={`bi bi-heart${isFavorite(part.id) ? '-fill text-danger' : ''}`}></i>
                                </button>
                            </div>
                            
                            <ProductCardImageSlider part={part} />
                            
                            <div className="product-meta">
                                <span className="product-category">{part.partTypeName || "DIGƏR"}</span>
                                <span className="product-stock">Stokda {part.stockQuantity} ədəd</span>
                            </div>
                            
                            <Link to={`/details/${part.id}`} className="text-decoration-none text-main">
                                <h3 className="product-title">{formatCleanTitle(part)}</h3>
                            </Link>
                            
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
                    fetchProducts(searchTerm, activeCategoryId, activeSellerId, 1, newSize);
                  }}
                  style={{ width: 'auto', display: 'inline-block' }}
                >
                  <option value="10">10 / səhifə</option>
                  <option value="20">20 / səhifə</option>
                  <option value="50">50 / səhifə</option>
                </select>
              </div>
              
              <ul className="pagination pagination-sm mb-0 align-items-center">
                {page > 1 && (
                  <li className="page-item">
                    <button className="page-link" onClick={() => {
                      const prev = page - 1;
                      setPage(prev);
                      fetchProducts(searchTerm, activeCategoryId, activeSellerId, prev, pageSize);
                      window.scrollTo({ top: 0, behavior: 'smooth' });
                    }}>Əvvəlki</button>
                  </li>
                )}

                {(() => {
                  const windowSize = 4;
                  let start = page;
                  if (start + windowSize - 1 > totalPages) {
                    start = Math.max(1, totalPages - windowSize + 1);
                  }
                  const end = Math.min(totalPages, start + windowSize - 1);
                  const pages = [];
                  for (let i = start; i <= end; i++) {
                    pages.push(i);
                  }

                  return (
                    <>
                      {pages.map(p => (
                        <li key={p} className={`page-item ${p === page ? 'active' : ''}`}>
                          <button
                            className="page-link"
                            style={p === page ? { backgroundColor: 'var(--accent)', borderColor: 'var(--accent)', color: 'var(--accent-text, #000)', fontWeight: 'bold' } : {}}
                            onClick={() => {
                              if (p !== page) {
                                setPage(p);
                                fetchProducts(searchTerm, activeCategoryId, activeSellerId, p, pageSize);
                                window.scrollTo({ top: 0, behavior: 'smooth' });
                              }
                            }}
                          >
                            {p}
                          </button>
                        </li>
                      ))}
                      {end < totalPages && (
                        <li className="page-item disabled">
                          <span className="page-link border-0">..</span>
                        </li>
                      )}
                    </>
                  );
                })()}

                {page < totalPages && (
                  <li className="page-item">
                    <button className="page-link" onClick={() => {
                      const next = page + 1;
                      setPage(next);
                      fetchProducts(searchTerm, activeCategoryId, activeSellerId, next, pageSize);
                      window.scrollTo({ top: 0, behavior: 'smooth' });
                    }}>Növbəti</button>
                  </li>
                )}
              </ul>
            </div>
        )}
      </section>
    </div>
  );
}

export default Home;
