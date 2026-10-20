import React, { useEffect, useState, useMemo } from 'react';
import { Link } from 'react-router-dom';
import { Api } from '../api';
import { Loading, ErrorBox } from './Categories';
import { Modal } from '../components/Panel';
import { usePageSeo } from '../utils/seo';
import { alertModal } from '../utils/confirmModal';

function Saticilar({ user }) {
  usePageSeo({
    title: 'Satıcılar və Anbarlar',
    description: 'Etibarlı təchizatçılar, mobil telefon və kompüter təmiri ustaları və anbarlar.'
  });

  const [sellers, setSellers] = useState(null);
  const [search, setSearch] = useState('');
  const [searchType, setSearchType] = useState('all'); // 'all', 'servis', 'usta', 'anbardar'
  const [activeRole, setActiveRole] = useState('all'); // 'all', 'servis', 'usta', 'anbardar'
  const [error, setError] = useState('');

  // Rəy və reytinqlər üçün modal state-ləri
  const [activeReviewSeller, setActiveReviewSeller] = useState(null);
  const [sellerReviewsData, setSellerReviewsData] = useState(null);
  const [reviewsLoading, setReviewsLoading] = useState(false);
  const [showAddReviewForm, setShowAddReviewForm] = useState(false);
  const [ratingInput, setRatingInput] = useState(5);
  const [commentInput, setCommentInput] = useState('');
  const [reviewSubmitting, setReviewSubmitting] = useState(false);
  const [reviewMsg, setReviewMsg] = useState({ text: '', type: '' });

  const openReviewsModal = async (seller) => {
    setActiveReviewSeller(seller);
    setShowAddReviewForm(false);
    setReviewMsg({ text: '', type: '' });
    setReviewsLoading(true);
    try {
      const res = await Api.getSellerReviews(seller.id);
      setSellerReviewsData(res);
    } catch (e) {
      console.error(e);
    } finally {
      setReviewsLoading(false);
    }
  };

  const handleReviewSubmit = async (e) => {
    e.preventDefault();
    if (!user) {
      alertModal('Rəy bildirmək üçün zəhmət olmasa daxil olun.', 'Diqqət', 'info');
      return;
    }
    if (!commentInput.trim()) {
      setReviewMsg({ text: 'Zəhmət olmasa rəyinizi qeyd edin.', type: 'danger' });
      return;
    }
    setReviewSubmitting(true);
    setReviewMsg({ text: '', type: '' });
    try {
      const res = await Api.addSellerReview({
        sellerId: activeReviewSeller.id,
        rating: Number(ratingInput),
        comment: commentInput.trim()
      });
      setReviewMsg({ text: res.message || 'Rəyiniz uğurla əlavə edildi!', type: 'success' });
      setCommentInput('');
      setRatingInput(5);
      // Satıcılar siyahısında həmin satıcının reytinqini də yeniləyək
      if (res.newRating !== undefined) {
        setSellers(prev => prev ? prev.map(s => s.id === activeReviewSeller.id ? { ...s, rating: res.newRating } : s) : prev);
      }
      const updatedReviews = await Api.getSellerReviews(activeReviewSeller.id);
      setSellerReviewsData(updatedReviews);
      setTimeout(() => {
        setShowAddReviewForm(false);
        setReviewMsg({ text: '', type: '' });
      }, 1500);
    } catch (err) {
      setReviewMsg({ text: err.message || 'Xəta baş verdi.', type: 'danger' });
    } finally {
      setReviewSubmitting(false);
    }
  };


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
        return 'Satıcının adı və ya mağaza adı ilə axtar (məs: Rəşad, Vüqar)...';
      default:
        return 'Servis adı, usta adı və ya satıcı adı ilə axtar...';
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
              <i className="bi bi-box-seam me-1"></i> Satıcılar və Mağazalar
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
                <option value="anbardar">Satıcının adına görə</option>
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
                      <button
                        type="button"
                        onClick={() => openReviewsModal(s)}
                        className="badge bg-warning bg-opacity-10 text-warning border border-warning d-flex align-items-center gap-1"
                        style={{ borderRadius: '12px', cursor: 'pointer', padding: '0.35rem 0.6rem' }}
                        title="Mağaza rəylərinə bax və qiymətləndir"
                      >
                        <i className="bi bi-star-fill"></i> {Number(s.rating || 5).toFixed(1)}
                      </button>
                    </div>
                  </div>

                  {/* Servis / Mağaza / Şirkət adı və Loqosu */}
                  <div className="d-flex align-items-center gap-3 mb-2">
                    <div
                      style={{
                        width: '48px',
                        height: '48px',
                        borderRadius: '10px',
                        border: '1.5px solid var(--accent)',
                        overflow: 'hidden',
                        backgroundColor: 'var(--bg-surface-hover)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        flexShrink: 0
                      }}
                    >
                      {s.shopPhotoUrl ? (
                        <img
                          src={s.shopPhotoUrl}
                          alt={s.companyName}
                          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                        />
                      ) : (
                        <i className="bi bi-shop text-muted fs-4"></i>
                      )}
                    </div>
                    <div>
                      <h3 className="product-title m-0" style={{ fontSize: '1.15rem' }}>
                        {s.companyName}
                      </h3>
                      {/* Usta / Anbardar / Məsul şəxsin adı */}
                      <div className="text-muted small mt-1">
                        {isUsta ? (
                          <span><i className="bi bi-person-fill text-accent me-1"></i> Usta: <strong className="text-main">{s.contactPerson || `${s.firstName || ''} ${s.lastName || ''}`.trim()}</strong></span>
                        ) : isAnbardar ? (
                          <span><i className="bi bi-person-fill text-accent me-1"></i> Satıcı: <strong className="text-main">{s.contactPerson || `${s.firstName || ''} ${s.lastName || ''}`.trim()}</strong></span>
                        ) : (
                          <span><i className="bi bi-person-fill text-accent me-1"></i> Məsul şəxs: <strong className="text-main">{s.contactPerson}</strong></span>
                        )}
                      </div>
                    </div>
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
                      <div className="d-flex justify-content-between align-items-center">
                        <div className="text-truncate">
                          <i className="bi bi-geo-alt me-1 text-accent"></i>
                          <span>{s.address}</span>
                        </div>
                        <a
                          href={
                            s.latitude && s.longitude
                              ? `https://www.google.com/maps?q=${s.latitude},${s.longitude}`
                              : `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(s.address)}`
                          }
                          target="_blank"
                          rel="noreferrer"
                          className="ms-2 text-accent text-decoration-none fw-semibold flex-shrink-0"
                          style={{ fontSize: '11px' }}
                          title="Xəritədə bax"
                        >
                          Xəritə <i className="bi bi-box-arrow-up-right"></i>
                        </a>
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

                  {/* Fəaliyyət düymələri */}
                  <div className="mt-auto pt-2 d-flex flex-column gap-2">
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

                    <button
                      type="button"
                      onClick={() => openReviewsModal(s)}
                      className="btn btn-sm btn-outline text-muted w-100 d-flex justify-content-center align-items-center gap-1"
                      style={{ borderRadius: '10px' }}
                    >
                      <i className="bi bi-chat-square-text text-warning"></i>
                      <span>Rəylər və Qiymətləndirmə</span>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </section>

      {/* MAĞAZA RƏYLƏRİ VƏ DƏYƏRLƏNDİRMƏ MODALI */}
      {activeReviewSeller && (
        <Modal
          open={true}
          title={`${activeReviewSeller.companyName || 'Mağaza'} — Rəylər və Reytinq`}
          onClose={() => setActiveReviewSeller(null)}
        >
          <div className="p-3 rounded mb-3 d-flex align-items-center justify-content-between flex-wrap gap-2" style={{ background: 'var(--bg-main, #141414)', border: '1px solid var(--border-color)' }}>
            <div>
              <div className="d-flex align-items-center gap-2 mb-1">
                <span className="text-warning fw-bold fs-3">
                  ★ {Number(activeReviewSeller.rating || 5).toFixed(1)}
                </span>
                <span className="text-muted small">/ 5.0 maks</span>
              </div>
              <span className="text-muted small">Cəmi {sellerReviewsData?.totalReviews || 0} alıcı/usta rəyi</span>
            </div>

            {user ? (
              <button
                type="button"
                className="btn btn-sm btn-outline"
                style={{ borderColor: 'var(--accent)', color: 'var(--accent)' }}
                onClick={() => setShowAddReviewForm(!showAddReviewForm)}
              >
                <i className={`bi ${showAddReviewForm ? 'bi-x-lg' : 'bi-pencil-square'} me-1`}></i>
                {showAddReviewForm ? 'Formu bağla' : 'Rəy / Qiymət yaz'}
              </button>
            ) : (
              <Link to="/login" className="btn btn-sm btn-outline text-muted">
                Rəy yazmaq üçün daxil olun
              </Link>
            )}
          </div>

          {showAddReviewForm && (
            <form onSubmit={handleReviewSubmit} className="p-3 rounded mb-3" style={{ background: 'var(--bg-surface-hover)', border: '1px solid var(--border-color)' }}>
              <h6 className="fw-semibold mb-2">Mağazanı Dəyərləndirin</h6>

              {reviewMsg.text && (
                <div className={`alert alert-${reviewMsg.type} mb-2 py-2 small`}>
                  {reviewMsg.text}
                </div>
              )}

              <div className="mb-2">
                <label className="form-label text-muted small d-block mb-1">Reytinqiniz (1 - 5 ulduz)</label>
                <div className="d-flex gap-2">
                  {[1, 2, 3, 4, 5].map(star => (
                    <button
                      key={star}
                      type="button"
                      className="btn p-0 border-0 bg-transparent"
                      onClick={() => setRatingInput(star)}
                      style={{ fontSize: '1.8rem', color: star <= ratingInput ? '#ffc107' : '#555', cursor: 'pointer' }}
                    >
                      <i className={`bi ${star <= ratingInput ? 'bi-star-fill' : 'bi-star'}`}></i>
                    </button>
                  ))}
                  <span className="badge bg-warning bg-opacity-10 text-warning border border-warning align-self-center ms-2">
                    {ratingInput} / 5
                  </span>
                </div>
              </div>

              <div className="mb-3">
                <label className="form-label text-muted small">Yorumunuz / Fikriniz</label>
                <textarea
                  className="form-control"
                  rows="3"
                  placeholder="Mağazanın xidməti, ehtiyat hissələrinin keyfiyyəti və çatdırılma barədə yazın..."
                  value={commentInput}
                  onChange={e => setCommentInput(e.target.value)}
                  required
                  minLength={3}
                ></textarea>
              </div>

              <div className="d-flex justify-content-end gap-2">
                <button type="button" className="btn btn-sm btn-secondary" onClick={() => setShowAddReviewForm(false)}>
                  Ləğv et
                </button>
                <button type="submit" className="btn btn-sm" style={{ backgroundColor: 'var(--accent)', color: 'var(--accent-text)' }} disabled={reviewSubmitting}>
                  {reviewSubmitting ? 'Göndərilir...' : 'Rəyi Göndər'}
                </button>
              </div>
            </form>
          )}

          {reviewsLoading ? (
            <div className="text-center py-4 text-muted">Rəylər yüklənir...</div>
          ) : sellerReviewsData && sellerReviewsData.reviews?.length > 0 ? (
            <div className="d-flex flex-column gap-2" style={{ maxHeight: '350px', overflowY: 'auto' }}>
              {sellerReviewsData.reviews.map(rev => (
                <div key={rev.id} className="p-2 border rounded" style={{ borderColor: 'var(--border-color)', background: 'var(--bg-main)' }}>
                  <div className="d-flex justify-content-between align-items-center mb-1">
                    <div className="d-flex align-items-center gap-2">
                      <strong className="text-main small">{rev.userName}</strong>
                      <span className="badge bg-secondary" style={{ fontSize: '0.65rem' }}>{rev.userRole}</span>
                    </div>
                    <div className="text-warning small">
                      {[1, 2, 3, 4, 5].map(s => (
                        <i key={s} className={`bi ${s <= rev.rating ? 'bi-star-fill' : 'bi-star'}`}></i>
                      ))}
                    </div>
                  </div>
                  <p className="m-0 small text-main" style={{ whiteSpace: 'pre-line' }}>{rev.comment}</p>
                  <div className="text-muted" style={{ fontSize: '0.7rem' }}>
                    {new Date(rev.createdDate).toLocaleDateString('az-AZ')}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-4 text-muted border rounded" style={{ borderColor: 'var(--border-color)', borderStyle: 'dashed' }}>
              <i className="bi bi-chat-square-text fs-4 mb-1 d-block"></i>
              <span>Bu mağaza üçün hələ rəy yazılmayıb. İlk dəyərləndirməni siz edin!</span>
            </div>
          )}
        </Modal>
      )}
    </div>
  );
}

export default Saticilar;
