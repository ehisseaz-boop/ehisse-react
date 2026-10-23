import React, { useState, useEffect } from 'react';
import { api as apiFetch } from '../../api';
import { alertModal } from '../../utils/confirmModal';

export default function AdminSettings() {
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [settings, setSettings] = useState({
    AboutText: '',
    AboutFull: '',
    ContactPhone: '',
    ContactWhatsApp: '',
    ContactEmail: '',
    ContactAddress: '',
    AnnouncementBanner: ''
  });

  useEffect(() => {
    loadSettings();
  }, []);

  async function loadSettings() {
    try {
      const data = await apiFetch('/api/Admin/Settings');
      if (data) {
        setSettings({
          AboutText: data.AboutText?.value || '',
          AboutFull: data.AboutFull?.value || '',
          ContactPhone: data.ContactPhone?.value || '',
          ContactWhatsApp: data.ContactWhatsApp?.value || '',
          ContactEmail: data.ContactEmail?.value || '',
          ContactAddress: data.ContactAddress?.value || '',
          AnnouncementBanner: data.AnnouncementBanner?.value || ''
        });
      }
    } catch (err) {
      console.error(err);
      alertModal('Tənzimləmələri yükləmək mümkün olmadı: ' + err.message, 'Xəta', 'error');
    } finally {
      setLoading(false);
    }
  }

  async function handleSave(e) {
    e.preventDefault();
    setSaving(true);
    try {
      await apiFetch('/api/Admin/UpdateSettings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(settings)
      });
      alertModal('Bütün məlumatlar və "Haqqında" mətni uğurla yeniləndi!', 'Uğurlu', 'success');
    } catch (err) {
      alertModal('Yadda saxlamaq mümkün olmadı: ' + err.message, 'Xəta', 'error');
    } finally {
      setSaving(false);
    }
  }

  function handleChange(field, value) {
    setSettings(prev => ({ ...prev, [field]: value }));
  }

  if (loading) {
    return (
      <div className="p-4 text-center">
        <div className="spinner-border text-primary" role="status"></div>
        <div className="mt-2 text-muted">Məlumatlar yüklənir...</div>
      </div>
    );
  }

  return (
    <div className="p-3 p-md-4">
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">
            <i className="bi bi-gear-wide-connected text-primary me-2"></i>
            Sayt Tənzimləmələri və CMS
          </h2>
          <p className="text-muted small mb-0">
            "Haqqında", əlaqə məlumatları və saytın statik mətnlərini birbaşa buradan dəyişə bilərsiniz.
          </p>
        </div>
      </div>

      <form onSubmit={handleSave} className="card shadow-sm border-0">
        <div className="card-body p-4">
          
          {/* Haqqımızda Bölməsi */}
          <div className="mb-4">
            <h5 className="fw-bold text-primary mb-3">
              <i className="bi bi-file-earmark-text me-2"></i>
              "Haqqında" və Tanıtım Mətnləri
            </h5>
            
            <div className="mb-3">
              <label className="form-label fw-semibold">
                Saytın Qısa Haqqında Mətni (Footer və alt hissələr üçün)
              </label>
              <textarea 
                className="form-control" 
                rows="2" 
                value={settings.AboutText}
                onChange={(e) => handleChange('AboutText', e.target.value)}
                placeholder="Qısa məlumat daxil edin..."
              />
              <div className="form-text">Bu mətn saytın altlığında (footer) brendin altında göstərilir.</div>
            </div>

            <div className="mb-3">
              <label className="form-label fw-semibold">
                Geniş Haqqımızda Mətni
              </label>
              <textarea 
                className="form-control" 
                rows="5" 
                value={settings.AboutFull}
                onChange={(e) => handleChange('AboutFull', e.target.value)}
                placeholder="Sayt haqqında ətraflı məlumat..."
              />
            </div>
          </div>

          <hr className="my-4" />

          {/* Əlaqə Məlumatları */}
          <div className="mb-4">
            <h5 className="fw-bold text-primary mb-3">
              <i className="bi bi-telephone me-2"></i>
              Əlaqə Məlumatları
            </h5>

            <div className="row g-3">
              <div className="col-md-6">
                <label className="form-label fw-semibold">Əlaqə Telefonu</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={settings.ContactPhone}
                  onChange={(e) => handleChange('ContactPhone', e.target.value)}
                  placeholder="+994 10 252 32 26"
                />
              </div>

              <div className="col-md-6">
                <label className="form-label fw-semibold">WhatsApp Nömrəsi</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={settings.ContactWhatsApp}
                  onChange={(e) => handleChange('ContactWhatsApp', e.target.value)}
                  placeholder="+994 10 396 15 01"
                />
              </div>

              <div className="col-md-6">
                <label className="form-label fw-semibold">Rəsmi E-poçt (Email)</label>
                <input 
                  type="email" 
                  className="form-control" 
                  value={settings.ContactEmail}
                  onChange={(e) => handleChange('ContactEmail', e.target.value)}
                  placeholder="info@e-hisse.az"
                />
              </div>

              <div className="col-md-6">
                <label className="form-label fw-semibold">Ünvan</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={settings.ContactAddress}
                  onChange={(e) => handleChange('ContactAddress', e.target.value)}
                  placeholder="Bakı şəh., 28 May / Nizami küç."
                />
              </div>
            </div>
          </div>

          <hr className="my-4" />

          {/* Sayt Yuxarı Banneri */}
          <div className="mb-4">
            <h5 className="fw-bold text-primary mb-3">
              <i className="bi bi-megaphone me-2"></i>
              Yuxarı Elan / Bildiriş Xətti (Top Banner)
            </h5>
            <div>
              <label className="form-label fw-semibold">Bildiriş mətni (Boş qoyularsa gizlənir)</label>
              <input 
                type="text" 
                className="form-control" 
                value={settings.AnnouncementBanner}
                onChange={(e) => handleChange('AnnouncementBanner', e.target.value)}
                placeholder="Məs: Saytımızda texniki profilaktika gedir və ya Yeni xidmət istifadəyə verildi!"
              />
            </div>
          </div>

          {/* Yadda saxla düyməsi */}
          <div className="d-flex justify-content-end">
            <button 
              type="submit" 
              className="btn btn-primary px-4 py-2 fw-semibold"
              disabled={saving}
            >
              {saving ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                  Yadda saxlanılır...
                </>
              ) : (
                <>
                  <i className="bi bi-check2-circle me-1"></i> Yadda saxla
                </>
              )}
            </button>
          </div>

        </div>
      </form>
    </div>
  );
}
