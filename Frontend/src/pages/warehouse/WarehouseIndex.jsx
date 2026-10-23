import React, { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Api, imageSrc } from '../../api';
import { Loading, ErrorBox } from '../Categories';
import { Table, statusBadge } from '../../components/Panel';
import { cleanPartTypeName } from '../Home';
import { confirmModal, promptModal } from '../../utils/confirmModal';
import { exportWarehouseToExcel } from '../../utils/excelHelper';
import ExcelImportModal from '../../components/ExcelImportModal';

function WarehouseIndex() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');
  const [msg, setMsg] = useState('');
  const [importModalOpen, setImportModalOpen] = useState(false);

  const load = useCallback(() => {
    Api.warehouse.list().then(setData).catch(e => setError(e.message));
  }, []);

  useEffect(() => { load(); }, [load]);

  const run = async (fn, okMsg) => {
    setMsg('');
    try { await fn(); if (okMsg) setMsg(okMsg); load(); }
    catch (e) { setMsg(e.message); }
  };

  const changePrice = async (p) => {
    const v = await promptModal({
      title: 'Yeni qiymət daxil edin',
      inputLabel: 'AZN:',
      inputValue: p.price,
      inputType: 'number',
      confirmText: 'Yenilə'
    });
    if (v === null || v === undefined) return;
    const n = Number(String(v).replace(',', '.'));
    if (!(n > 0)) { setMsg('Qiymət sıfırdan böyük olmalıdır.'); return; }
    run(() => Api.warehouse.updatePrice(p.id, n), 'Qiymət yeniləndi.');
  };

  if (error) return <ErrorBox message={error} />;
  if (!data) return <Loading />;

  const parts = data.parts;
  const columns = [
    { key: 'image', label: '', render: p => p.image ? <img src={imageSrc(p.image)} alt="" style={{ width: 48, height: 48, objectFit: 'cover', borderRadius: 6 }} /> : null },
    { key: 'title', label: 'Elan', render: p => {
      let rawTitle = (p.title || '')
        .replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar')
        .replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar')
        .replace(/telefon\s+üçün\s+/gi, '')
        .trim();

      const brand = (p.brandName || '').trim();
      const model = (p.modelName || '').trim();
      const cleanType = cleanPartTypeName(p.partTypeName);

      // Əgər rawTitle artıq brand və ya model ilə başlayırsa, təkrar əlavə etmirik
      let displayTitle = rawTitle;
      if (brand && model) {
        const fullPrefix = `${brand} ${model}`.toLowerCase();
        if (displayTitle.toLowerCase().startsWith(fullPrefix)) {
          // artıq içində var
        } else if (displayTitle.toLowerCase().startsWith(model.toLowerCase())) {
          displayTitle = `${brand} ${displayTitle}`;
        } else {
          displayTitle = `${brand} ${model} ${displayTitle}`;
        }
      } else if (model && !displayTitle.toLowerCase().startsWith(model.toLowerCase())) {
        displayTitle = `${model} ${displayTitle}`;
      }

      return (
        <>
          <Link to={`/warehouse/part/${p.id}`} className="text-decoration-none fw-semibold">
            {displayTitle.trim()}
          </Link>
          <div className="text-muted" style={{ fontSize: '0.8rem' }}>
            {p.sku ? `${p.sku} · ` : ''}{cleanType} · {p.qualityTypeName}
          </div>
        </>
      );
    }},
    { key: 'price', label: 'Qiymət', render: p => <a href="#" onClick={e => { e.preventDefault(); changePrice(p); }} title="Qiyməti dəyiş">{p.price} AZN</a> },
    { key: 'stockQuantity', label: 'Stok', render: p => <span className={`fw-bold ${p.stockQuantity <= 0 ? 'text-danger' : ''}`}>{p.stockQuantity} ədəd</span> },
    { key: 'viewCount', label: 'Baxış' },
    { key: 'status', label: 'Status', render: p => <>{statusBadge(p.moderationStatus)}{p.moderationStatus === 'Rejected' && p.rejectionReason && <div className="text-muted" style={{ fontSize: '0.75rem' }}>{p.rejectionReason}</div>}<div style={{ fontSize: '0.75rem' }}>{p.isActive ? 'Aktiv' : 'Deaktiv'}</div></> },
    {
      key: 'actions', label: '', render: p => (
        <div className="d-flex gap-1 flex-wrap">
          <Link to={`/warehouse/part/${p.id}`} className="btn-outline" title="Elan təfərrüatları və satış"><i className="bi bi-eye"></i></Link>
          <button 
            className="btn-outline" 
            title={p.stockQuantity === 1 ? 'Satıldı (Son məhsul)' : 'Bu elandan bir məhsul daha satıldı'} 
            disabled={p.stockQuantity <= 0} 
            onClick={async () => {
              if (p.stockQuantity <= 0) return;
              const msg = p.stockQuantity === 1 
                ? 'Bu elan üzrə son məhsul satıldı. Satışı təsdiqləyirsinizmi?' 
                : 'Bir ədəd satıldığı kimi qeydə alınacaq və elanın stok sayı bir vahid azalacaq. Təsdiq edirsinizmi?';
              const confirmed = await confirmModal({
                title: p.stockQuantity === 1 ? 'Son məhsul satışı' : 'Satış qeydi',
                text: msg,
                icon: 'question',
                confirmButtonText: 'Təsdiq edirəm'
              });
              if (!confirmed) return;
              run(() => Api.warehouse.recordSale(p.id), p.stockQuantity === 1 ? 'Son məhsul satıldı və elan deaktiv edildi.' : 'Satış qeydə alındı.');
            }}
          >
            <i className="bi bi-cart-check"></i>
          </button>
          <Link to={`/warehouse/edit-part/${p.id}`} className="btn-outline" title="Redaktə"><i className="bi bi-pencil"></i></Link>
          <button className="btn-outline" title={p.isActive ? 'Deaktiv et' : 'Aktiv et'} onClick={() => run(() => Api.warehouse.toggleActive(p.id))}><i className={`bi bi-${p.isActive ? 'eye-slash' : 'eye'}`}></i></button>
          <button 
            className="btn-outline" 
            title="Sil" 
            onClick={async () => {
              if (await confirmModal('Elanı silmək istədiyinizə əminsiniz?')) {
                run(() => Api.warehouse.remove(p.id), 'Elan silindi.');
              }
            }}
          >
            <i className="bi bi-trash"></i>
          </button>
        </div>
      )
    },
  ];

  const seller = data.seller || {};
  const mapAddress = seller.address || '';
  const hasCoords = seller.latitude && seller.longitude;
  const mapEmbedUrl = hasCoords
    ? `https://maps.google.com/maps?q=${seller.latitude},${seller.longitude}&t=&z=15&ie=UTF8&iwloc=&output=embed`
    : mapAddress
    ? `https://maps.google.com/maps?q=${encodeURIComponent(mapAddress)}&t=&z=14&ie=UTF8&iwloc=&output=embed`
    : null;

  return (
    <>
      {/* Mağaza / Usta / Anbardar Profil Başlığı və Xəritə */}
      <div className="product-card mb-4 p-3 p-md-4" style={{ borderRadius: '14px', background: 'var(--bg-surface)' }}>
        <div className="row g-3 align-items-center">
          <div className="col-12 col-md-7 d-flex align-items-center gap-3">
            <div
              style={{
                width: '68px',
                height: '68px',
                borderRadius: '14px',
                border: '2px solid var(--accent)',
                overflow: 'hidden',
                backgroundColor: 'var(--bg-surface-hover)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0
              }}
            >
              {seller.shopPhotoUrl ? (
                <img
                  src={seller.shopPhotoUrl}
                  alt={(seller.companyName || "").replace(/\(?Anbardar\)?/gi, "(Satıcı)")}
                  onError={(e) => {
                    e.currentTarget.style.display = 'none';
                    if (e.currentTarget.nextElementSibling) {
                      e.currentTarget.nextElementSibling.style.display = 'block';
                    }
                  }}
                  style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                />
              ) : null}
              <i
                className="bi bi-shop text-muted fs-2"
                style={{ display: seller.shopPhotoUrl ? 'none' : 'block' }}
              ></i>
            </div>
            <div>
              <div className="d-flex align-items-center gap-2">
                <h4 className="fw-bold m-0 text-main" style={{ fontSize: '1.25rem' }}>{(seller.companyName || "").replace(/\(?Anbardar\)?/gi, "(Satıcı)")}</h4>
                <Link to="/warehouse/settings" className="badge bg-secondary text-decoration-none" title="Tənzimləmələrə keç">
                  <i className="bi bi-pencil-square me-1"></i>Redaktə
                </Link>
              </div>
              <div className="text-muted small mt-1 d-flex align-items-center gap-1">
                <i className="bi bi-geo-alt-fill text-accent"></i>
                <span>{seller.address || 'Ünvan qeyd edilməyib'}</span>
              </div>
              <div className="text-muted small mt-1">
                Cəmi <strong>{parts.length}</strong> elan · Aktiv: <strong className="text-success">{parts.filter(p => p.isActive).length}</strong>
              </div>
            </div>
          </div>

          <div className="col-12 col-md-5">
            {mapEmbedUrl ? (
              <div style={{ borderRadius: '10px', overflow: 'hidden', border: '1px solid var(--border-color)', position: 'relative' }}>
                <iframe
                  title="Kabinet Xəritə"
                  width="100%"
                  height="130"
                  frameBorder="0"
                  style={{ border: 0, display: 'block' }}
                  src={mapEmbedUrl}
                  allowFullScreen
                />
                <a
                  href={
                    hasCoords
                      ? `https://www.google.com/maps?q=${seller.latitude},${seller.longitude}`
                      : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(mapAddress)}`
                  }
                  target="_blank"
                  rel="noreferrer"
                  className="position-absolute bottom-0 end-0 m-1 badge bg-dark bg-opacity-75 text-accent text-decoration-none"
                  style={{ fontSize: '11px', padding: '4px 8px' }}
                >
                  Google Maps <i className="bi bi-box-arrow-up-right ms-1"></i>
                </a>
              </div>
            ) : (
              <div className="p-3 text-center rounded border border-dashed small text-muted">
                <i className="bi bi-pin-map text-accent fs-5 d-block mb-1"></i>
                <span>Xəritədə mağaza yerinizi göstərmək üçün </span>
                <Link to="/warehouse/settings" className="text-accent fw-bold text-decoration-none">
                  profil ünvanınızı qeyd edin
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>

      <div className="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <div>
          <h5 className="m-0 fw-bold">Elanlarım</h5>
        </div>
        <div className="d-flex gap-2 flex-wrap">
          <button 
            type="button" 
            className="btn-outline" 
            title="Bütün elanları Excel faylı kimi endir"
            onClick={() => exportWarehouseToExcel(parts, seller.companyName)}
            disabled={!parts || parts.length === 0}
          >
            <i className="bi bi-file-earmark-arrow-down text-accent"></i> Excel-ə Çıxar
          </button>
          <button 
            type="button" 
            className="btn-outline" 
            title="Excel və ya CSV faylı ilə kütləvi elan əlavə et"
            onClick={() => setImportModalOpen(true)}
          >
            <i className="bi bi-file-earmark-arrow-up text-accent"></i> Excel-dən Yüklə
          </button>
          <Link to="/warehouse/deleted" className="btn-outline text-decoration-none">
            <i className="bi bi-trash3 me-1"></i> Silinən elanlar
          </Link>
          <Link to="/warehouse/add-part" className="btn-outline bg-accent text-decoration-none" style={{ color: '#000' }}>
            <i className="bi bi-plus-lg"></i> Yeni elan
          </Link>
        </div>
      </div>
      {msg && <div className="alert alert-info">{msg}</div>}
      {/* Desktop üçün Cədvəl Görünüşü (Geniş ekranlar) */}
      <div className="d-none d-md-block">
        <Table columns={columns} rows={parts} empty="Hələ elanınız yoxdur." />
      </div>

      {/* Mobil üçün Səliqəli Kart Görünüşü (Heç bir horizontal scroll olmadan) */}
      <div className="d-block d-md-none">
        {(!parts || parts.length === 0) ? (
          <div className="empty-state text-center p-4 rounded" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)' }}>
            <i className="bi bi-inbox fs-2 text-muted"></i>
            <p className="mt-2 text-muted">Hələ elanınız yoxdur.</p>
          </div>
        ) : (
          <div className="d-flex flex-column gap-3 mb-4">
            {parts.map(p => {
              let rawTitle = (p.title || '')
                .replace(/telefon\s+üçün\s+ekranlar/gi, 'Ekranlar')
                .replace(/telefon\s+üçün\s+batareyalar/gi, 'Batareyalar')
                .replace(/telefon\s+üçün\s+/gi, '')
                .trim();
              const brand = (p.brandName || '').trim();
              const model = (p.modelName || '').trim();
              const cleanType = cleanPartTypeName(p.partTypeName);

              let displayTitle = rawTitle;
              if (brand && model) {
                const fullPrefix = `${brand} ${model}`.toLowerCase();
                if (displayTitle.toLowerCase().startsWith(fullPrefix)) {
                  // ok
                } else if (displayTitle.toLowerCase().startsWith(model.toLowerCase())) {
                  displayTitle = `${brand} ${displayTitle}`;
                } else {
                  displayTitle = `${brand} ${model} ${displayTitle}`;
                }
              } else if (model && !displayTitle.toLowerCase().startsWith(model.toLowerCase())) {
                displayTitle = `${model} ${displayTitle}`;
              }

              return (
                <div
                  key={p.id}
                  className="product-card p-3 shadow-sm"
                  style={{
                    borderRadius: '12px',
                    border: '1px solid var(--border-color)',
                    background: 'var(--bg-surface)'
                  }}
                >
                  <div className="d-flex gap-3 align-items-start mb-2">
                    {p.image ? (
                      <img
                        src={imageSrc(p.image)}
                        alt=""
                        style={{
                          width: '64px',
                          height: '64px',
                          objectFit: 'cover',
                          borderRadius: '8px',
                          border: '1px solid var(--border-color)',
                          flexShrink: 0
                        }}
                      />
                    ) : (
                      <div
                        style={{
                          width: '64px',
                          height: '64px',
                          borderRadius: '8px',
                          background: 'var(--bg-surface-hover)',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          flexShrink: 0,
                          color: 'var(--text-muted)'
                        }}
                      >
                        <i className="bi bi-image fs-4"></i>
                      </div>
                    )}

                    <div className="flex-grow-1 min-w-0">
                      <Link
                        to={`/warehouse/part/${p.id}`}
                        className="text-decoration-none fw-bold text-main d-block mb-1 text-truncate"
                        style={{ fontSize: '0.98rem' }}
                      >
                        {displayTitle.trim()}
                      </Link>
                      <div className="text-muted small mb-1" style={{ fontSize: '0.78rem' }}>
                        {p.sku ? <span>{p.sku} · </span> : null}
                        <span>{cleanType}</span>
                        {p.qualityTypeName && <span> · {p.qualityTypeName}</span>}
                      </div>
                      <div className="d-flex align-items-center gap-2 flex-wrap">
                        {statusBadge(p.moderationStatus)}
                        <span className={`badge ${p.isActive ? 'bg-success-subtle text-success border border-success' : 'bg-secondary-subtle text-muted'}`} style={{ fontSize: '0.7rem' }}>
                          {p.isActive ? 'Aktiv' : 'Deaktiv'}
                        </span>
                        <span className="text-muted small" style={{ fontSize: '0.75rem' }}>
                          <i className="bi bi-eye me-1"></i>{p.viewCount} baxış
                        </span>
                      </div>
                    </div>
                  </div>

                  {/* Qiymət və Stok xülasəsi */}
                  <div
                    className="d-flex justify-content-between align-items-center p-2 rounded mb-3"
                    style={{ background: 'var(--input-bg)', border: '1px solid var(--border-color)' }}
                  >
                    <div>
                      <span className="text-muted small d-block" style={{ fontSize: '0.75rem' }}>Qiymət:</span>
                      <a
                        href="#"
                        onClick={e => { e.preventDefault(); changePrice(p); }}
                        className="fw-bold text-accent text-decoration-none"
                        style={{ fontSize: '1.1rem' }}
                        title="Qiyməti dəyiş"
                      >
                        {p.price} AZN <i className="bi bi-pencil-fill ms-1" style={{ fontSize: '0.75rem' }}></i>
                      </a>
                    </div>
                    <div className="text-end">
                      <span className="text-muted small d-block" style={{ fontSize: '0.75rem' }}>Stok sayı:</span>
                      <strong className={p.stockQuantity <= 0 ? 'text-danger' : 'text-main'} style={{ fontSize: '1rem' }}>
                        {p.stockQuantity} ədəd
                      </strong>
                    </div>
                  </div>

                  {/* Əməliyyat Düymələri (Səliqəli tam enli düymələr sırası) */}
                  <div className="d-flex gap-2">
                    <Link
                      to={`/warehouse/part/${p.id}`}
                      className="btn btn-sm btn-outline flex-fill text-center d-flex align-items-center justify-content-center gap-1"
                      title="Bax"
                    >
                      <i className="bi bi-eye"></i> Bax
                    </Link>
                    <button
                      className="btn btn-sm btn-outline-success flex-fill d-flex align-items-center justify-content-center gap-1 fw-semibold"
                      title="Satış qeyd et"
                      disabled={p.stockQuantity <= 0}
                      onClick={async () => {
                        if (p.stockQuantity <= 0) return;
                        const msg = p.stockQuantity === 1 
                          ? 'Bu elan üzrə son məhsul satıldı. Satışı təsdiqləyirsinizmi?' 
                          : 'Bir ədəd satıldığı kimi qeydə alınacaq və elanın stok sayı bir vahid azalacaq. Təsdiq edirsinizmi?';
                        const confirmed = await confirmModal({
                          title: p.stockQuantity === 1 ? 'Son məhsul satışı' : 'Satış qeydi',
                          text: msg,
                          icon: 'question',
                          confirmButtonText: 'Təsdiq edirəm'
                        });
                        if (!confirmed) return;
                        run(() => Api.warehouse.recordSale(p.id), p.stockQuantity === 1 ? 'Son məhsul satıldı və elan deaktiv edildi.' : 'Satış qeydə alındı.');
                      }}
                    >
                      <i className="bi bi-cart-check"></i> Satıldı
                    </button>
                    <Link
                      to={`/warehouse/edit-part/${p.id}`}
                      className="btn btn-sm btn-outline flex-fill text-center d-flex align-items-center justify-content-center gap-1"
                      title="Redaktə et"
                    >
                      <i className="bi bi-pencil"></i>
                    </Link>
                    <button
                      className="btn btn-sm btn-outline"
                      title={p.isActive ? 'Deaktiv et' : 'Aktiv et'}
                      onClick={() => run(() => Api.warehouse.toggleActive(p.id))}
                    >
                      <i className={`bi bi-${p.isActive ? 'eye-slash' : 'eye'}`}></i>
                    </button>
                    <button
                      className="btn btn-sm btn-outline-danger"
                      title="Sil"
                      onClick={async () => {
                        if (await confirmModal('Elanı silmək istədiyinizə əminsiniz?')) {
                          run(() => Api.warehouse.remove(p.id), 'Elan silindi.');
                        }
                      }}
                    >
                      <i className="bi bi-trash"></i>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>

      <ExcelImportModal
        isOpen={importModalOpen}
        onClose={() => setImportModalOpen(false)}
        onSuccess={load}
      />
    </>
  );
}

export default WarehouseIndex;
