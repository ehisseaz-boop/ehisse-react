import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { statusBadge } from '../../components/Panel';
import { confirmModal, alertModal } from '../../utils/confirmModal';

export default function WarehousePartDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [part, setPart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [actionBusy, setActionBusy] = useState(false);

  const loadPart = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await Api.warehouse.get(id);
      setPart(data);
    } catch (err) {
      setError(err.message || 'Elan məlumatları tapılmadı.');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    loadPart();
  }, [loadPart]);

  const handleRecordSale = async () => {
    if (!part || part.stockQuantity <= 0) return;

    let confirmMsg = '';
    let confirmTitle = '';

    if (part.stockQuantity === 1) {
      confirmTitle = 'Son məhsul satışı';
      confirmMsg = 'Bu elan üzrə son məhsul satıldı. Satışı təsdiqləyirsinizmi? (Elan avtomatik deaktiv ediləcək)';
    } else {
      confirmTitle = 'Satış qeydi';
      confirmMsg = 'Bir ədəd satıldığı kimi qeydə alınacaq və elanın stok sayı bir vahid azalacaq. Təsdiq edirsinizmi?';
    }

    const confirmed = await confirmModal({
      title: confirmTitle,
      text: confirmMsg,
      icon: 'question',
      confirmButtonText: 'Təsdiq edirəm'
    });
    if (!confirmed) return;

    setActionBusy(true);
    try {
      await Api.warehouse.recordSale(part.id);
      if (window.Swal) {
        await window.Swal.fire({
          icon: 'success',
          title: 'Satış qeydə alındı!',
          text: part.stockQuantity === 1 
            ? 'Son məhsul satıldı və elan deaktiv edildi.' 
            : 'Stok sayı 1 vahid azaldıldı.',
          timer: 2000,
          showConfirmButton: false
        });
      }
      loadPart();
    } catch (err) {
      await alertModal(err.message || 'Satış qeyd edilə bilmədi.', 'Xəta baş verdi', 'error');
    } finally {
      setActionBusy(false);
    }
  };

  if (loading) return <Loading />;
  if (error) return <ErrorBox message={error} />;
  if (!part) return <ErrorBox message="Elan tapılmadı." />;

  const isOutOfStock = part.stockQuantity <= 0;
  const isLastItem = part.stockQuantity === 1;

  return (
    <div className="container py-3">
      <div className="mb-3 d-flex justify-content-between align-items-center flex-wrap gap-2">
        <button 
          onClick={() => navigate('/warehouse')} 
          className="btn btn-outline d-inline-flex align-items-center gap-2"
        >
          <i className="bi bi-arrow-left"></i> Elanlarıma qayıt
        </button>
        <div className="d-flex gap-2">
          <Link to={`/details/${part.id}`} className="btn btn-outline" target="_blank">
            <i className="bi bi-box-arrow-up-right me-1"></i> İctimai səhifə
          </Link>
          <Link to={`/warehouse/edit-part/${part.id}`} className="btn btn-warning text-dark fw-bold">
            <i className="bi bi-pencil me-1"></i> Redaktə et
          </Link>
        </div>
      </div>

      <div className="row g-4">
        {/* Sol tərəf: Şəkillər və detallar */}
        <div className="col-lg-8">
          <div className="p-4 rounded" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)' }}>
            <div className="d-flex justify-content-between align-items-start mb-3 flex-wrap gap-2">
              <div>
                <h3 className="text-main fw-bold m-0">{part.title}</h3>
                <div className="text-muted small mt-1">
                  SKU: <strong>{part.sku || 'Təyin edilməyib'}</strong> · № {part.id}
                </div>
              </div>
              <div className="d-flex align-items-center gap-2">
                {statusBadge(part.moderationStatus)}
                <span className={`badge ${part.isActive ? 'bg-success' : 'bg-secondary'}`}>
                  {part.isActive ? 'Aktiv' : 'Deaktiv'}
                </span>
              </div>
            </div>

            {/* Qalereya */}
            {part.images && part.images.length > 0 ? (
              <div className="d-flex gap-2 flex-wrap my-3">
                {part.images.map(img => (
                  <img
                    key={img.id}
                    src={imageSrc(img.url)}
                    alt=""
                    style={{
                      width: '120px',
                      height: '120px',
                      objectFit: 'cover',
                      borderRadius: '8px',
                      border: img.isMain ? '2px solid var(--accent)' : '1px solid var(--border-color)'
                    }}
                  />
                ))}
              </div>
            ) : (
              <div className="p-3 text-muted text-center border rounded my-3">
                <i className="bi bi-image fs-1 d-block mb-1"></i> Şəkil əlavə edilməyib
              </div>
            )}

            <div className="mt-4">
              <h5 className="fw-semibold text-main">Təsvir</h5>
              <p className="text-muted" style={{ whiteSpace: 'pre-line' }}>
                {part.description || 'Əlavə təsvir qeyd edilməyib.'}
              </p>
            </div>

            {part.locationAddress && (
              <div className="mt-3 text-muted small">
                <i className="bi bi-geo-alt text-accent me-1"></i>
                Ünvan: {part.locationAddress}
              </div>
            )}
          </div>
        </div>

        {/* Sağ tərəf: Stok və Satış idarəetməsi */}
        <div className="col-lg-4">
          <div className="p-4 rounded" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)' }}>
            <h5 className="fw-bold text-main mb-3">Qiymət və Stok</h5>
            
            <div className="mb-3 pb-3 border-bottom" style={{ borderColor: 'var(--border-color)' }}>
              <div className="text-muted small">Qiymət</div>
              <h2 className="text-accent fw-bold m-0">{part.price} {part.currency || 'AZN'}</h2>
            </div>

            <div className="mb-4">
              <div className="text-muted small mb-1">Cari stok miqdarı</div>
              <div className="d-flex align-items-center gap-2">
                <span className={`fs-4 fw-bold ${isOutOfStock ? 'text-danger' : 'text-main'}`}>
                  {part.stockQuantity} ədəd
                </span>
                {isOutOfStock && (
                  <span className="badge bg-danger">Stok bitib</span>
                )}
              </div>
            </div>

            {/* Biznes məntiqinə uyğun Satış qeyd etmə düyməsi */}
            <div className="d-grid gap-2">
              {!isOutOfStock ? (
                <button
                  type="button"
                  onClick={handleRecordSale}
                  disabled={actionBusy}
                  className={`btn py-3 fw-bold d-flex align-items-center justify-content-center gap-2 ${
                    isLastItem ? 'btn-danger' : 'btn-success'
                  }`}
                  style={{ borderRadius: '8px', fontSize: '1rem' }}
                >
                  <i className="bi bi-cart-check fs-5"></i>
                  <span>
                    {isLastItem ? 'Satıldı' : 'Bu elandan bir məhsul daha satıldı'}
                  </span>
                </button>
              ) : (
                <div className="alert alert-secondary text-center small m-0">
                  <i className="bi bi-info-circle me-1"></i> Stok sıfırdır. Əlavə satış qeydə alına bilməz.
                </div>
              )}
            </div>

            <div className="text-muted small mt-3" style={{ fontSize: '0.8rem', lineHeight: '1.4' }}>
              Qeyd: Satış qeyd edildikdə stok avtomatik azalır və satış tarixçəsinə yazılır. Son məhsul satıldıqda elan avtomatik deaktiv olunur.
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
