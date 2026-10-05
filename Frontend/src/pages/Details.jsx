import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Api, imageSrc } from '../api';
import { Loading, ErrorBox } from './Categories';

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

function Details() {
  const { id } = useParams();
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [activeImg, setActiveImg] = useState(0);

  useEffect(() => {
    setData(null); setError(''); setActiveImg(0);
    Api.details(id).then(setData).catch(e => setError(e.message));
  }, [id]);

  if (error) return <div className="container"><ErrorBox message={error} /></div>;
  if (!data) return <div className="container"><Loading /></div>;

  const { part, seller } = data;
  const images = part.images || [];

  const getMapSrc = () => {
    if (part.latitude && part.longitude) {
      return `https://maps.google.com/maps?q=${part.latitude},${part.longitude}&t=&z=13&ie=UTF8&iwloc=&output=embed`;
    }
    const address = part.locationAddress || seller?.address;
    if (address) {
      return `https://maps.google.com/maps?q=${encodeURIComponent(address)}&t=&z=13&ie=UTF8&iwloc=&output=embed`;
    }
    return null;
  };
  const mapSrc = getMapSrc();

  return (
    <div className="container">
      <section>
        <Link to="/" className="link-accent"><i className="bi bi-arrow-left"></i> Kataloqa qayıt</Link>
        <div className="d-flex flex-column flex-md-row gap-4 mt-3">
          <div style={{ flex: 1 }}>
            <div className="product-image" style={{ minHeight: 300 }}>
              {images[activeImg] ? <img src={imageSrc(images[activeImg].url)} alt={part.title} style={{ maxWidth: '100%' }} /> : <i className="bi bi-image" style={{ fontSize: '4rem' }}></i>}
            </div>
            {images.length > 1 && (
              <div className="d-flex gap-2 mt-2">
                {images.map((img, i) => (
                  <img key={i} src={imageSrc(img.url)} alt="" onClick={() => setActiveImg(i)}
                    style={{ width: 64, height: 64, objectFit: 'cover', cursor: 'pointer', border: i === activeImg ? '2px solid var(--accent)' : '1px solid var(--border-color)', borderRadius: 8 }} />
                ))}
              </div>
            )}
          </div>

          <div style={{ flex: 1 }}>
            <span className="product-category">{data.categoryName} Â· {part.partTypeName}</span>
            <h1 className="section-title">{part.brandName} {part.modelName} {part.title}</h1>
            <div className="product-condition">{part.qualityTypeName}</div>
            <div className="product-price my-3">
              {data.discountPrice ? <><s>{part.price}</s> {data.discountPrice}</> : part.price} <span>{data.currency || 'AZN'}</span>
            </div>
            <p className="product-stock">Stokda {part.stockQuantity} ədəd · Baxış: {part.viewCount}{isToday(part.createdDate) ? <span className="badge-accent ms-2" style={{ backgroundColor: '#28a745', color: '#fff', fontSize: '0.75rem', padding: '0.2rem 0.5rem', display: 'inline-block', verticalAlign: 'middle' }}>Yeni</span> : <span className="ms-2">· Yerləşdirilib: {formatDate(part.createdDate)}</span>}</p>
            {data.sku && <p className="text-muted">SKU: {data.sku}</p>}
            {data.warrantyMonths > 0 && <p>Zəmanət: {data.warrantyMonths} ay</p>}
            {data.description && <p className="product-desc">{data.description}</p>}

            {seller && (
              <div className="product-card mt-3">
                <h3 className="product-title"><i className="bi bi-shop text-accent"></i> {seller.companyName}</h3>
                <p className="product-desc">{seller.contactPerson}</p>
                {seller.phone && <a href={`tel:${seller.phone}`} className="btn-outline text-decoration-none d-inline-block me-2"><i className="bi bi-telephone"></i> {seller.phone}</a>}
                {seller.whatsAppNumber && (
                  <a href={`https://wa.me/${seller.whatsAppNumber.replace(/\D/g, '')}`} target="_blank" rel="noreferrer" className="btn-outline text-decoration-none d-inline-block">
                    <i className="bi bi-whatsapp"></i> WhatsApp
                  </a>
                )}
              </div>
            )}
            
            {mapSrc && (
              <div className="product-card mt-3">
                <h3 className="product-title mb-3"><i className="bi bi-geo-alt text-accent"></i> Xəritədə yeri</h3>
                <iframe
                  width="100%"
                  height="300"
                  frameBorder="0"
                  style={{ border: 0, borderRadius: '8px' }}
                  src={mapSrc}
                  allowFullScreen
                ></iframe>
              </div>
            )}
          </div>
        </div>
      </section>
    </div>
  );
}

export default Details;

