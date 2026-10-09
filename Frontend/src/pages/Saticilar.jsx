import React, { useEffect, useState, useMemo } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';

function Saticilar() {
  const [sellers, setSellers] = useState(null);
  const [search, setSearch] = useState('');
  const [searchType, setSearchType] = useState('all'); // 'all', 'servis', 'usta', 'anbardar'
  const [activeRole, setActiveRole] = useState('all'); // 'all', 'servis', 'usta', 'anbardar'
  const [error, setError] = useState('');

  const load = (term = search, type = searchType, role = activeRole) => {
    setSellers(null);
    setError('');
    Api.sellers({
      searchString: term,
      searchType: type !== 'all' ? type : undefined,
      role: role !== 'all' ? role : undefined
    })
      .then(data => setSellers(data || []))
      .catch(e => setError(e.message));
  };

  useEffect(() => {
    load(search, searchType, activeRole);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchType, activeRole]);

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    load(search, searchType, activeRole);
  };

  const handleReset = () => {
    setSearch('');
    setSearchType('all');
    setActiveRole('all');
    load('', 'all', 'all');
  };

  const placeholderText = useMemo(() => {
    switch (searchType) {
      case 'servis':
        return 'Servis və ya mağaza adı ilə axtar (məs: TechMaster)...';
      case 'usta':
        return 'Ustanın adı və ya soyadı ilə axtar (məs: Elçin, Nigar)...';
      case 'anbardar':
        return 'Anbardarın adı və ya anbar adı ilə axtar (məs: Rəşad, Vüqar)...';
      default:
        return 'Servis adı, usta adı və ya anbardar adı ilə axtar...';
    }
  }, [searchType]);

  const stats = useMemo(() => {
    if (!sellers) return { totalSellers: 0, totalActiveAds: 0, totalEverAds: 0 };
    return {
      totalSellers: sellers.length,
      totalActiveAds: sellers.reduce((acc, s) => acc + (s.activeAds || 0), 0),
      totalEverAds: sellers.reduce((acc, s) => acc + (s.totalAds || 0), 0),
    };
  }, [sellers]);

  return (
    <div className="container py-4">
      <section>
        {/* Başlıq və Məlumat */}
        <div className="text-center mb-4">
          <div className="section-label">Təsdiqlənmiş Tərəfdaşlar</div>
          <h1 className="hero-title mb-2" style={{ fontSize: '2.5rem' }}>
            Satıcılar və <span className="text-accent">Anbarlar</span>
          </h1>
          <p className="text-muted" style={{ maxWidth: '650px', margin: '0 auto' }}>
            Platformamızdakı rəsmi servis mərkəzləri, peşəkar ustalar və etibarlı ehtiyat hissə anbarları
          </p>

          {/* Ümumi statistika göstəriciləri */}
          {sellers && (
            <div className="d-flex flex-wrap justify-content-center gap-3 mt-3">
              <span className="badge-accent" style={{ padding: '0.4rem 0.8rem', borderRadius: '20px' }}>
                <i className="bi bi-people-fill me-1"></i> {stats.totalSellers} Satıcı / Anbar
              </span>
              <span className="badge" style={{ background: 'rgba(255,255,255,0.06)', border: '1px solid var(--border-color)', color: 'var(--text-main)', padding: '0.4rem 0.8rem', borderRadius: '20px' }}>
                <i className="bi bi-box-seam me-1 text-accent"></i> {stats.totalActiveAds} Aktiv elan
              </span>
              <span className="badge" style={{ background: 'rgba(255,255,255,0.06)', border: '1px solid var(--border-color)', color: 'var(--text-muted)', padding: '0.4rem 0.8rem', borderRadius: '20px' }}>
                <i className="bi bi-archive me-1"></i> {stats.totalEverAds} İndiyədək yerləşdirilmiş elan
              </span>
            </div>
          )}
        </div>

        {/* Axtarış və Filtr Paneli */}
        <div className="p-3 mb-4 rounded-4" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)' }}>
          {/* Kateqoriya Tabs */}
          <div className="d-flex flex-wrap gap-2 mb-3 pb-3 border-bottom" style={{ borderColor: 'var(--border-color)' }}>
            <button
              type="button"
              onClick={() => setActiveRole('all')}
              className={`btn btn-sm ${activeRole === 'all' ? 'bg-accent text-dark fw-bold' : 'btn-outline text-muted'}`}
              style={{ borderRadius: '20px', padding: '0.4rem 1.2rem' }}
            >
              <i className="bi bi-grid-fill me-1"></i> Hamısı
            </button>
            <button
              type="button"
              onClick={() => setActiveRole('servis')}
              className={`btn btn-sm ${activeRole === 'servis' ? 'bg-accent text-dark fw-bold' : 'btn-outline text-muted'}`}
              style={{ borderRadius: '20px', padding: '0.4rem 1.2rem' }}
            >
              <i className="bi bi-shop me-1"></i> Servis və Mağazalar
            </button>
            <button
              type="button"
              onClick={() => setActiveRole('usta')}
              className={`btn btn-sm ${activeRole === 'usta' ? 'bg-accent text-dark fw-bold' : 'btn-outline text-muted'}`}
              style={{ borderRadius: '20px', padding: '0.4rem 1.2rem' }}
            >
              <i className="bi bi-tools me-1"></i> Ustalar
            </button>
            <button
              type="button"
              onClick={() => setActiveRole('anbardar')}
              className={`btn btn-sm ${activeRole === 'anbardar' ? 'bg-accent text-dark fw-bold' : 'btn-outline text-muted'}`}
              style={{ borderRadius: '20px', padding: '0.4rem 1.2rem' }}
            >
              <i className="bi bi-box-seam me-1"></i> Anbardarlar və Anbarlar
            </button>
          </div>

          {/* Axtarış Formu */}
          <form onSubmit={handleSearchSubmit} className="d-flex flex-column flex-md-row gap-2">
            {/* Axtarış meyarı (dropdown) */}
            <div style={{ minWidth: '200px' }}>
              <select
                className="form-select"
                value={searchType}
                onChange={e => setSearchType(e.target.value)}
                style={{
                  background: 'var(--input-bg)',
                  borderColor: 'var(--input-border)',
                  color: 'var(--text-main)',
                  borderRadius: '12px',
                  padding: '0.6rem 1rem'
                }}
              >
                <option value="all">Hamısı üzrə axtar</option>
                <option value="servis">Servisin adına görə</option>
                <option value="usta">Ustanın adına görə</option>
                <option value="anbardar">Anbardarın adına görə</option>
              </select>
            </div>

            {/* Axtarış sahəsi */}
            <div className="position-relative flex-grow-1">
              <i className="bi bi-search position-absolute text-muted" style={{ left: '14px', top: '50%', transform: 'translateY(-50%)' }}></i>
              <input
                type="text"
                value={search}
                onChange={e => setSearch(e.target.value)}
                placeholder={placeholderText}
                className="form-control ps-5"
                style={{
                  background: 'var(--input-bg)',
                  borderColor: 'var(--input-border)',
                  color: 'var(--text-main)',
                  borderRadius: '12px',
                  padding: '0.6rem 1rem 0.6rem 2.5rem'
                }}
              />
              {search && (
                <button
                  type="button"
                  onClick={() => { setSearch(''); load('', searchType, activeRole); }}
                  className="btn btn-link position-absolute text-muted p-0"
                  style={{ right: '12px', top: '50%', transform: 'translateY(-50%)', textDecoration: 'none' }}
                >
                  <i className="bi bi-x-circle-fill"></i>
                </button>
              )}
            </div>

            {/* Axtar düyməsi */}
            <button
              type="submit"
              className="btn btn-primary bg-accent text-dark fw-bold px-4"
              style={{ borderRadius: '12px', whiteSpace: 'nowrap' }}
            >
              <i className="bi bi-search me-1"></i> Axtar
            </button>

            {/* Təmizlə düyməsi (əgər filtr aktivdirsə) */}
            {(search || searchType !== 'all' || activeRole !== 'all') && (
              <button
                type="button"
                onClick={handleReset}
                className="btn btn-outline text-muted"
                style={{ borderRadius: '12px', whiteSpace: 'nowrap' }}
                title="Filtrləri sıfırla"
              >
                <i className="bi bi-arrow-counterclockwise me-1"></i> Sıfırla
              </button>
            )}
          </form>
        </div>

        {/* Nəticələr Bölməsi */}
        {error ? (
          <ErrorBox message={error} />
        ) : !sellers ? (
          <Loading />
        ) : sellers.length === 0 ? (
          <div className="text-center py-5 p-4 rounded-4" style={{ background: 'var(--bg-surface)', border: '1px solid var(--border-color)' }}>
            <i className="bi bi-search text-muted" style={{ fontSize: '3rem' }}></i>
            <h4 className="mt-3">Uyğun satıcı və ya anbar tapılmadı</h4>
            <p className="text-muted">Axtarış sözünü dəyişin və ya filtrləri sıfırlayın.</p>
            <button onClick={handleReset} className="btn btn-outline mt-2" style={{ borderRadius: '20px' }}>
              Bütün satıcıları göstər
            </button>
          </div>
        ) : (
          <div className="products-grid">
            {sellers.map(s => {
              const isUsta = s.roleName === 'Usta';
              const isAnbardar = s.roleName === 'Anbardar';

              const roleBadgeClass = isUsta
                ? 'badge bg-warning bg-opacity-10 text-warning border border-warning'
                : isAnbardar
                ? 'badge bg-info bg-opacity-10 text-info border border-info'
                : 'badge bg-primary bg-opacity-10 text-primary border border-primary';

              const roleIcon = isUsta ? 'bi-tools' : isAnbardar ? 'bi-box-seam' : 'bi-shop';

              return (
                <div key={s.id} className="product-card d-flex flex-column h-100 shadow-sm" style={{ minHeight: '380px' }}>
                  {/* Kartın yuxarı hissəsi: Nişanlar və Reytinq */}
                  <div className="d-flex justify-content-between align-items-center mb-3">
                    <span className={roleBadgeClass} style={{ padding: '0.35rem 0.65rem', borderRadius: '12px' }}>
                      <i className={`bi ${roleIcon} me-1`}></i> {s.roleName}
                    </span>
                    <div className="d-flex align-items-center gap-1">
                      {s.isPhoneVerified && (
                        <span className="badge bg-success bg-opacity-10 text-success border border-success" title="Təsdiqlənmiş satıcı" style={{ borderRadius: '12px' }}>
                          <i className="bi bi-patch-check-fill me-1"></i> Təsdiqli
                        </span>
                      )}
                      <span className="badge bg-warning bg-opacity-10 text-warning border border-warning" style={{ borderRadius: '12px' }}>
                        ★ {Number(s.rating || 5).toFixed(1)}
                      </span>
                    </div>
                  </div>

                  {/* Servis / Mağaza / Şirkət adı */}
                  <h3 className="product-title mb-1" style={{ fontSize: '1.25rem' }}>
                    <i className="bi bi-shop text-accent me-2"></i>
                    {s.companyName}
                  </h3>

                  {/* Usta / Anbardar / Məsul şəxsin adı */}
                  <div className="mb-3 text-muted small">
                    {isUsta ? (
                      <div><i className="bi bi-person-fill text-accent me-1"></i> Usta: <strong className="text-main">{s.contactPerson || `${s.firstName || ''} ${s.lastName || ''}`.trim()}</strong></div>
                    ) : isAnbardar ? (
                      <div><i className="bi bi-person-fill text-accent me-1"></i> Anbardar: <strong className="text-main">{s.contactPerson || `${s.firstName || ''} ${s.lastName || ''}`.trim()}</strong></div>
                    ) : (
                      <div><i className="bi bi-person-fill text-accent me-1"></i> Məsul şəxs: <strong className="text-main">{s.contactPerson}</strong></div>
                    )}
                  </div>

                  {/* Elan Statistikası Qutusu (Aktiv elan sayı və İndiyədək yerləşdiyi elan sayı) */}
                  <div
                    className="p-3 rounded-3 mb-3 d-flex justify-content-between align-items-center"
                    style={{ background: 'var(--input-bg)', border: '1px solid var(--border-color)' }}
                  >
                    <div className="text-center flex-fill border-end" style={{ borderColor: 'var(--border-color)' }}>
                      <div className="text-muted small" style={{ fontSize: '0.78rem' }}>
                        <i className="bi bi-check-circle-fill text-success me-1"></i>Aktiv Elan Sayı
                      </div>
                      <div className="fw-bold text-accent mt-1" style={{ fontSize: '1.35rem' }}>
                        {s.activeAds} <span style={{ fontSize: '0.8rem', fontWeight: 'normal', color: 'var(--text-muted)' }}>ədəd</span>
                      </div>
                    </div>
                    <div className="text-center flex-fill">
                      <div className="text-muted small" style={{ fontSize: '0.78rem' }}>
                        <i className="bi bi-clock-history me-1"></i>İndiyədək Yerləşdirdiyi
                      </div>
                      <div className="fw-bold mt-1" style={{ fontSize: '1.35rem', color: 'var(--text-main)' }}>
                        {s.totalAds} <span style={{ fontSize: '0.8rem', fontWeight: 'normal', color: 'var(--text-muted)' }}>elan</span>
                      </div>
                    </div>
                  </div>

                  {/* Əlaqə məlumatları */}
                  <div className="d-flex flex-column gap-1 mb-3 small text-muted">
                    {s.address && (
                      <div>
                        <i className="bi bi-geo-alt me-1 text-accent"></i>
                        <span>{s.address}</span>
                      </div>
                    )}
                    {s.phone && (
                      <div>
                        <i className="bi bi-telephone me-1 text-accent"></i>
                        <a href={`tel:${s.phone}`} className="text-decoration-none text-main hover-accent">
                          {s.phone}
                        </a>
                      </div>
                    )}
                    {s.whatsAppNumber && (
                      <div>
                        <i className="bi bi-whatsapp me-1 text-success"></i>
                        <a
                          href={`https://wa.me/${s.whatsAppNumber.replace(/\D/g, '')}`}
                          target="_blank"
                          rel="noreferrer"
                          className="text-decoration-none text-success"
                        >
                          {s.whatsAppNumber}
                        </a>
                      </div>
                    )}
                  </div>

                  {/* Fəaliyyət düyməsi: Elanlara keçid */}
                  <div className="mt-auto pt-2">
                    <Link
                      to={`/?sellerId=${s.id}`}
                      className="btn btn-outline w-100 d-flex justify-content-between align-items-center"
                      style={{ borderRadius: '12px', padding: '0.6rem 1rem' }}
                    >
                      <span>
                        <i className="bi bi-box-seam me-2 text-accent"></i>
                        Elanlarına bax
                      </span>
                      <span className="badge-accent" style={{ borderRadius: '12px', padding: '0.2rem 0.6rem' }}>
                        {s.activeAds} aktiv <i className="bi bi-arrow-right ms-1"></i>
                      </span>
                    </Link>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </section>
    </div>
  );
}

export default Saticilar;
