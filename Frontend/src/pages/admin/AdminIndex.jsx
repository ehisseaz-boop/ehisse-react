import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { StatCard } from '../../components/Panel';
import { Link } from 'react-router-dom';

export default function AdminIndex() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  async function loadStats() {
    try {
      const data = await apiFetch('/api/Admin/Dashboard');
      setStats(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <div className="text-center py-5">Məlumatlar yüklənir...</div>;
  if (!stats) return <div className="alert alert-danger">Statistik məlumatları yükləmək mümkün olmadı.</div>;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h3 className="section-title mb-1">İdarəetmə Paneli</h3>
          <p className="text-muted m-0">Sistem statistikası və moderasiya mərkəzi</p>
        </div>
        <Link to="/admin/requests" className="btn btn-outline" style={{ borderColor: 'var(--accent)', color: 'var(--accent)' }}>
          <i className="bi bi-inbox me-1"></i> Bütün Müraciətlərə Bax ({stats.activeRequests ?? 0})
        </Link>
      </div>

      {/* Əsas Statistik Göstəricilər */}
      <div className="row g-3 mb-4">
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-people"
            label="Ümumi İstifadəçilər"
            value={stats.countUsers}
            tone="accent"
            to="/admin/users"
          />
        </div>
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-person-check"
            label="Gözləyən Qeydiyyatlar"
            value={stats.pendingUsers}
            tone={stats.pendingUsers > 0 ? "warning" : "success"}
            to="/admin/requests"
          />
        </div>
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-box-seam"
            label="Ümumi Elanlar"
            value={stats.countParts}
            tone="accent"
            to="/admin/ads"
          />
        </div>
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-file-earmark-plus"
            label="Yeni Elan Sorğuları"
            value={stats.pendingNewAds}
            tone={stats.pendingNewAds > 0 ? "warning" : "success"}
            to="/admin/requests"
          />
        </div>
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-pencil-square"
            label="Redaktə Yoxlamaları"
            value={stats.pendingUpdateAds}
            tone={stats.pendingUpdateAds > 0 ? "warning" : "accent"}
            to="/admin/requests"
          />
        </div>
        <div className="col-12 col-sm-6 col-xl-4">
          <StatCard
            icon="bi-arrow-repeat"
            label="Stok Bərpa Sorğuları"
            value={stats.pendingRestockAds}
            tone={stats.pendingRestockAds > 0 ? "warning" : "accent"}
            to="/admin/requests"
          />
        </div>
      </div>

      {/* Sürətli Keçidlər və Qaydalar Bloku */}
      <div className="row g-3">
        <div className="col-12 col-lg-6">
          <div className="panel-card" style={{ background: 'var(--surface-color)', padding: '1.5rem', borderRadius: '12px', border: '1px solid var(--border-color)' }}>
            <h5 className="mb-3 d-flex align-items-center gap-2">
              <i className="bi bi-shield-check text-accent"></i> Moderasiya Qaydaları Xatırlatması
            </h5>
            <ul className="text-muted mb-0 ps-3" style={{ lineHeight: '1.8' }}>
              <li><strong>İstifadəçilər:</strong> Usta və Satıcı admin təsdiq edənə qədər kabinetə daxil ola bilməz.</li>
              <li><strong>Elanlar:</strong> Bütün yeni və redaktə edilmiş elanlar yalnız admin təsdiqindən sonra dərc olunur.</li>
              <li><strong>Stok və Bərpa:</strong> Stok 0 olduqda elan avtomatik deaktiv olur. Satıcı stok artırsa belə, elan yalnız admin bərpa etdikdə yenidən aktivləşir.</li>
              <li><strong>Əlaqə:</strong> Sifarişlər yalnız WhatsApp və zəng vasitəsilə həyata keçirilir.</li>
            </ul>
          </div>
        </div>

        <div className="col-12 col-lg-6">
          <div className="panel-card" style={{ background: 'var(--surface-color)', padding: '1.5rem', borderRadius: '12px', border: '1px solid var(--border-color)' }}>
            <h5 className="mb-3 d-flex align-items-center gap-2">
              <i className="bi bi-lightning-charge text-accent"></i> Sürətli İdarəetmə
            </h5>
            <div className="d-grid gap-2">
              <Link to="/admin/catalog" className="btn btn-outline text-start d-flex justify-content-between align-items-center">
                <span><i className="bi bi-diagram-3 me-2"></i> Kataloq və Hiyerarxiya Matrisi</span>
                <i className="bi bi-chevron-right"></i>
              </Link>
              <Link to="/admin/ads" className="btn btn-outline text-start d-flex justify-content-between align-items-center">
                <span><i className="bi bi-megaphone me-2"></i> Bütün Elanların Reyestri</span>
                <i className="bi bi-chevron-right"></i>
              </Link>
              <Link to="/admin/logs" className="btn btn-outline text-start d-flex justify-content-between align-items-center">
                <span><i className="bi bi-journal-text me-2"></i> Sistem və Satış Audit Jurnalları</span>
                <i className="bi bi-chevron-right"></i>
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
