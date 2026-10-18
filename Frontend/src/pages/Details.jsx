import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Api, imageSrc } from '../api';
import { Loading, ErrorBox } from './Categories';
import { usePageSeo } from '../utils/seo';
import { toggleFavorite, isFavorite } from '../utils/favorites';

const isToday = (dateString) => {
    if (!dateString) return false;
    const date = new Date(dateString);
    const today = new Date();
    return date.getDate() === today.getDate() &&
           date.getMonth() === today.getMonth() &&
           date.getFullYear() === today.getFullYear();
};

const formatDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('az-AZ', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
};

const cleanText = (text) => {
    if (!text) return '';
    return String(text)
        .replace(/\u00c6\u008f/g, 'Ə')
        .replace(/\u00c6\s*\u008f/g, 'Ə')
        .replace(/\u00e6\u008f/g, 'ə')
        .replace(/\u00c6[\s\u0080-\u009f]*liyev/gi, 'Əliyev')
        .replace(/\u00c6[\s\u0080-\u009f]*hmədli/gi, 'Əhmədli')
        .replace(/\u00c6[\s\u0080-\u009f]*sas/gi, 'Əsas')
        .replace(/Æ\x8f/g, 'Ə')
        .replace(/Æ\s*liyev/gi, 'Əliyev')
        .replace(/Æ\s*hmədli/gi, 'Əhmədli')
        .replace(/Æ\s*sas/gi, 'Əsas')
        .replace(/Æ/g, 'Ə')
        .replace(/Â·/g, '·')
        .replace(/Â/g, '')
        .replace(/\?stifad/g, 'İstifad')
        .replace(/\?\?+/g, ' ');
};

const getCleanTitle = (part) => {
    if (!part) return '';
    let t = part.title || '';
    const brand = part.brandName || '';
    const model = part.modelName || '';
    
    // Təkrarlanan brend və ya model sözlərini çıxar
    let full = t;
    if (!full.toLowerCase().includes(brand.toLowerCase()) && brand) {
        full = `${brand} ${full}`;
    }
    if (!full.toLowerCase().includes(model.toLowerCase()) && model) {
        full = `${full} ${model}`;
    }
    // Eyni sözün ardıcıl təkrarını aradan qaldır (məs: iPhone 15 Pro Max iPhone 15 Pro Max -> iPhone 15 Pro Max)
    const words = full.split(' ');
    const uniqueWords = [];
    for (let i = 0; i < words.length; i++) {
        if (i === 0 || words[i].toLowerCase() !== words[i - 1].toLowerCase()) {
            uniqueWords.push(words[i]);
        }
    }
    // Model adının dublikatını təmizlə
    let res = uniqueWords.join(' ');
    if (model && res.indexOf(model) !== res.lastIndexOf(model)) {
        res = res.replace(model, '').trim();
        res = `${model} ${res}`;
    }
    res = res.replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar')
             .replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar')
             .replace(/telefon\s+üçün\s+/gi, '');
    return cleanText(res.replace(/\s+/g, ' ').trim());
};

function Details() {
  const { id } = useParams();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [activeImg, setActiveImg] = useState(0);
  const [showPhone, setShowPhone] = useState(false);
  const [, setFavSync] = useState(0);

  useEffect(() => {
    const handleFav = () => setFavSync(v => v + 1);
    window.addEventListener('favorites-updated', handleFav);
    return () => window.removeEventListener('favorites-updated', handleFav);
  }, []);

  useEffect(() => {
    setData(null); setError(''); setActiveImg(0); setShowPhone(false);
    Api.details(id).then(setData).catch(e => setError(e.message));
  }, [id]);

  const displayTitle = data?.part ? getCleanTitle(data.part) : '';
  const sellerCompany = cleanText(data?.seller?.companyName);
  const sellerContact = cleanText(data?.seller?.contactPerson);

  usePageSeo({
    title: displayTitle ? `${displayTitle} (${data?.part?.qualityTypeName || 'Ehtiyat hissəsi'}) - ${Math.floor(data?.part?.price || 0)} AZN` : 'Məhsul detalları',
    description: `${displayTitle || 'Ehtiyat hissəsi'}. Qiymət: ${Math.floor(data?.part?.price || 0)} AZN. Satıcı: ${sellerCompany || sellerContact || 'e-hisse.az'}. Stokda: ${data?.part?.stockQuantity || 0} ədəd.`
  });

  if (error) return <div className="container py-4"><ErrorBox message={error} /></div>;
  if (!data) return <div className="container py-4"><Loading /></div>;

  const { part, seller } = data;
  const images = part.images || [];

  const getMapSrc = () => {
    if (part.latitude && part.longitude) {
      return `https://maps.google.com/maps?q=${part.latitude},${part.longitude}&t=&z=14&ie=UTF8&iwloc=&output=embed`;
    }
    const address = part.locationAddress || seller?.address;
    if (address) {
      return `https://maps.google.com/maps?q=${encodeURIComponent(address)}&t=&z=14&ie=UTF8&iwloc=&output=embed`;
    }
    return null;
  };
  const mapSrc = getMapSrc();

  const rawPhone = seller?.phone || '';
  const maskedPhone = rawPhone && rawPhone.length > 5
    ? rawPhone.slice(0, rawPhone.length - 4) + '••••'
    : rawPhone;

  const sellerAddress = cleanText(part.locationAddress || seller?.address);

  // WhatsApp nömrəsi (whatsapp sahəsi və ya telefon)
  const waTarget = seller?.whatsAppNumber || seller?.phone || '';
  const waCleanNumber = waTarget.replace(/\D/g, '');

  return (
    <div className="container py-4">
      {/* Geri linki */}
      <div className="mb-3">
        <Link to="/" className="link-accent d-inline-flex align-items-center gap-1 text-decoration-none fw-medium">
          <i className="bi bi-arrow-left"></i> Kataloqa qayıt
        </Link>
      </div>

      <div className="row g-4 align-items-start">
        {/* SOL TƏRƏF: Şəkil (Optimal ölçüdə) + Şəklin altında Məhsul haqqında məlumatlar */}
        <div className="col-lg-8">
          {/* 1. Əsas Şəkil və Qalereya */}
          <div className="product-card p-3 mb-4" style={{ borderRadius: '12px' }}>
            <div
              className="product-image d-flex align-items-center justify-content-center"
              style={{
                height: 340,
                maxHeight: 360,
                borderRadius: '8px',
                overflow: 'hidden',
                background: 'rgba(0,0,0,0.25)',
                padding: '10px'
              }}
            >
              {images[activeImg] ? (
                <img
                  src={imageSrc(images[activeImg].url)}
                  alt={`${displayTitle} - Şəkil ${activeImg + 1}`}
                  loading="lazy"
                  decoding="async"
                  style={{ maxHeight: '100%', maxWidth: '100%', objectFit: 'contain' }}
                />
              ) : (
                <i className="bi bi-image text-muted" style={{ fontSize: '4rem' }}></i>
              )}
            </div>

            {images.length > 1 && (
              <div className="d-flex gap-2 mt-3 overflow-auto pb-1">
                {images.map((img, i) => (
                  <img
                    key={i}
                    src={imageSrc(img.url)}
                    alt={`${displayTitle} - kiçik önbaxış ${i + 1}`}
                    loading="lazy"
                    decoding="async"
                    onClick={() => setActiveImg(i)}
                    style={{
                      width: 64,
                      height: 64,
                      objectFit: 'cover',
                      cursor: 'pointer',
                      border: i === activeImg ? '2px solid var(--accent)' : '1px solid var(--border-color)',
                      borderRadius: 8,
                      opacity: i === activeImg ? 1 : 0.7,
                      transition: 'all 0.2s'
                    }}
                  />
                ))}
              </div>
            )}
          </div>

          {/* 2. ŞƏKLİN ALTINDA: Məhsulun Parametrləri və Haqqında */}
          <div className="product-card p-4 mb-4" style={{ borderRadius: '12px' }}>
            <div className="mb-3">
              <span className="text-muted small d-block mb-1">
                {data.categoryName} · {part.partTypeName ? part.partTypeName.replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar').replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar').replace(/telefon\s+üçün\s+/gi, '') : ''}
              </span>
              <h2 className="product-title m-0" style={{ fontSize: '1.5rem', fontWeight: 700, lineHeight: 1.3 }}>
                {displayTitle}
              </h2>
            </div>

            {/* Parametrlər cədvəl/grid şəklində */}
            <div
              className="p-3 mb-4 rounded"
              style={{ background: 'var(--bg-main, #141414)', border: '1px solid var(--border-color)' }}
            >
              <div className="row g-3" style={{ fontSize: '0.95rem' }}>
                <div className="col-6 col-sm-4">
                  <div className="text-muted small">Vəziyyəti</div>
                  <strong className="text-accent">{part.qualityTypeName || 'Standart'}</strong>
                </div>
                <div className="col-6 col-sm-4">
                  <div className="text-muted small">Stok sayı</div>
                  <strong className="text-main">{part.stockQuantity} ədəd</strong>
                </div>
                <div className="col-6 col-sm-4">
                  <div className="text-muted small">Zəmanət</div>
                  <strong className="text-main">{data.warrantyMonths > 0 ? `${data.warrantyMonths} ay` : 'Yoxdur'}</strong>
                </div>
                {data.sku && (
                  <div className="col-6 col-sm-4">
                    <div className="text-muted small">SKU Kodu</div>
                    <strong className="text-main">{data.sku}</strong>
                  </div>
                )}
                {sellerAddress && (
                  <div className="col-12 col-sm-8">
                    <div className="text-muted small">Yerləşdiyi ünvan</div>
                    <strong className="text-main">{sellerAddress}</strong>
                  </div>
                )}
              </div>
            </div>

            {/* Təsvir / Haqqında */}
            <div className="mb-4">
              <h4 className="fw-semibold text-main mb-2" style={{ fontSize: '1.15rem' }}>
                <i className="bi bi-file-text text-accent me-2"></i>Haqqında
              </h4>
              <p
                className="product-desc m-0"
                style={{ fontSize: '0.98rem', lineHeight: '1.6', whiteSpace: 'pre-line' }}
              >
                {data.description || 'Məhsul haqqında əlavə təsvir qeyd edilməyib.'}
              </p>
            </div>

            <hr style={{ borderColor: 'var(--border-color)', margin: '1.5rem 0' }} />

            {/* Elan statistikası və tarixi */}
            <div className="d-flex flex-wrap justify-content-between align-items-center text-muted small">
              <div className="d-flex gap-3">
                <span>№ {part.id}</span>
                <span>•</span>
                <span>Baxış: {part.viewCount}</span>
                <span>•</span>
                <span>Yerləşdirilib: {formatDate(part.createdDate)}</span>
              </div>
              {isToday(part.createdDate) && (
                <span className="badge bg-success">Bu gün yenilənib</span>
              )}
            </div>
          </div>
        </div>

        {/* SAĞ TƏRƏF (STICKY): Böyük Qiymət, Əlaqə düymələri, Satıcı kartı və Xəritə */}
        <div className="col-lg-4" style={{ position: 'sticky', top: '1rem' }}>
          <div className="product-card p-4" style={{ borderRadius: '12px' }}>
            {/* BÖYÜK QİYMƏT */}
            <div className="mb-4 pb-2 border-bottom d-flex justify-content-between align-items-center" style={{ borderColor: 'var(--border-color)' }}>
              <div>
                <div className="text-muted small mb-1">Qiymət</div>
                <div className="d-flex align-items-baseline gap-2">
                  <span className="text-accent fw-bold" style={{ fontSize: '2.5rem', lineHeight: 1 }}>
                    {data.discountPrice || part.price}
                  </span>
                  <span className="text-main fw-bold" style={{ fontSize: '1.5rem' }}>
                    {data.currency || 'AZN'}
                  </span>
                  {data.discountPrice && (
                    <s className="text-muted ms-2" style={{ fontSize: '1.4rem' }}>{part.price} AZN</s>
                  )}
                </div>
              </div>
              <button
                type="button"
                className={`btn d-flex align-items-center justify-content-center p-2 rounded-circle`}
                style={{
                  width: '44px',
                  height: '44px',
                  backgroundColor: 'var(--bg-surface-hover)',
                  border: '1px solid var(--border-color)',
                  color: isFavorite(part.id) ? '#ff4d4d' : 'var(--text-muted)',
                  fontSize: '1.3rem',
                  cursor: 'pointer'
                }}
                onClick={() => toggleFavorite(part)}
                title={isFavorite(part.id) ? "Sevimlilərdən çıxar" : "Sevimlilərə əlavə et"}
                aria-label="Sevimlilər"
              >
                <i className={`bi bi-heart${isFavorite(part.id) ? '-fill' : ''}`}></i>
              </button>
            </div>

            {/* 1. Zəng et (Nömrəni göstər) - Zəng ikonası ilə */}
            {seller?.phone ? (
              <div className="mb-2">
                {!showPhone ? (
                  <button
                    type="button"
                    onClick={() => setShowPhone(true)}
                    className="btn w-100 py-3 fw-bold d-flex align-items-center justify-content-center gap-2"
                    style={{
                      backgroundColor: '#28a745',
                      color: '#ffffff',
                      borderRadius: '8px',
                      fontSize: '1.05rem',
                      border: 'none',
                      transition: 'all 0.2s'
                    }}
                  >
                    <i className="bi bi-telephone-fill" style={{ fontSize: '1.2rem' }}></i>
                    <span>Zəng et ({maskedPhone})</span>
                  </button>
                ) : (
                  <a
                    href={`tel:${seller.phone}`}
                    className="btn w-100 py-3 fw-bold d-flex align-items-center justify-content-center gap-2 text-decoration-none"
                    style={{
                      backgroundColor: '#28a745',
                      color: '#ffffff',
                      borderRadius: '8px',
                      fontSize: '1.1rem',
                      border: 'none'
                    }}
                  >
                    <i className="bi bi-telephone-fill" style={{ fontSize: '1.2rem' }}></i>
                    <span>{seller.phone}</span>
                  </a>
                )}
              </div>
            ) : null}

            {/* 2. WhatsApp ilə Əlaqə */}
            {waCleanNumber ? (
              <a
                href={`https://wa.me/${waCleanNumber}?text=${encodeURIComponent(`Salam, bu ehtiyat hissəsi elanı ilə maraqlanıram: ${displayTitle}. Elan kodu: ${part.id}. Link: ${window.location.origin}/elan/${part.id}`)}`}
                target="_blank"
                rel="noreferrer"
                className="btn w-100 py-3 fw-bold d-flex align-items-center justify-content-center gap-2 mb-3 text-decoration-none"
                style={{
                  backgroundColor: '#25D366',
                  color: '#ffffff',
                  borderRadius: '8px',
                  fontSize: '1.05rem',
                  border: 'none'
                }}
              >
                <i className="bi bi-whatsapp" style={{ fontSize: '1.25rem' }}></i>
                <span>WhatsApp ilə yaz</span>
              </a>
            ) : null}

            {/* Satıcı profili kartı */}
            {seller && (
              <div
                className="p-3 mb-3 rounded"
                style={{ background: 'var(--bg-main, #141414)', border: '1px solid var(--border-color)' }}
              >
                <div className="d-flex justify-content-between align-items-start mb-2">
                  <div>
                    <h5 className="m-0 text-main fw-bold">{sellerCompany}</h5>
                    <span className="text-muted small">{sellerContact || 'Satıcı'}</span>
                  </div>
                  <span className="badge bg-secondary">
                    {seller.rating ? `★ ${seller.rating}` : 'Təsdiqli'}
                  </span>
                </div>

                {sellerAddress && (
                  <div className="text-muted small mt-2 d-flex align-items-start gap-1">
                    <i className="bi bi-geo-alt text-accent mt-1"></i>
                    <span>{sellerAddress}</span>
                  </div>
                )}
              </div>
            )}

            {/* Xəritə (Sağ tərəfdə, əlaqə və satıcı məlumatının altında) */}
            {mapSrc && (
              <div className="mt-3">
                <div className="d-flex justify-content-between align-items-center mb-2">
                  <span className="text-muted small fw-semibold">
                    <i className="bi bi-geo-alt text-accent me-1"></i>Xəritədə yeri
                  </span>
                  <a
                    href={
                      part.latitude && part.longitude
                        ? `https://www.google.com/maps?q=${part.latitude},${part.longitude}`
                        : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(sellerAddress || '')}`
                    }
                    target="_blank"
                    rel="noreferrer"
                    className="small text-accent text-decoration-none"
                  >
                    Böyüt <i className="bi bi-box-arrow-up-right"></i>
                  </a>
                </div>
                <div style={{ borderRadius: '8px', overflow: 'hidden', border: '1px solid var(--border-color)' }}>
                  <iframe
                    title="Xəritə"
                    width="100%"
                    height="190"
                    frameBorder="0"
                    style={{ border: 0, display: 'block' }}
                    src={mapSrc}
                    allowFullScreen
                  />
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Details;

