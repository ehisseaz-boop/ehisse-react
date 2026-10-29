import React, { useState, useEffect, useRef } from 'react';
import { Api } from '../api';
import { parseExcelFile, downloadExcelTemplate } from '../utils/excelHelper';
import { alertModal } from '../utils/confirmModal';
import { addNotification } from '../utils/notifications';

export default function ExcelImportModal({ isOpen, onClose, onSuccess }) {
  const [file, setFile] = useState(null);
  const [parsing, setParsing] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [previewRows, setPreviewRows] = useState([]);
  const [lookups, setLookups] = useState(null);
  const [errors, setErrors] = useState([]);
  const [matchedItems, setMatchedItems] = useState([]);
  const fileInputRef = useRef(null);

  // Kataloq lookups (bütün kateqoriyalar və keyfiyyətlər)
  useEffect(() => {
    if (!isOpen) return;
    (async () => {
      try {
        const [categories, qualityTypes] = await Promise.all([
          Api.lookupCategories(),
          Api.qualityTypes()
        ]);
        setLookups({ categories, qualityTypes });
      } catch (err) {
        console.error('Lookups yüklənərkən xəta:', err);
      }
    })();
  }, [isOpen]);

  if (!isOpen) return null;

  const handleFileChange = async (e) => {
    const selected = e.target.files?.[0];
    if (!selected) return;
    setFile(selected);
    setParsing(true);
    setErrors([]);
    setMatchedItems([]);
    setPreviewRows([]);

    try {
      const rows = await parseExcelFile(selected);
      if (!rows || rows.length === 0) {
        setErrors(['Excel faylı boşdur və ya düzgün məlumat tapılmadı.']);
        setParsing(false);
        return;
      }
      setPreviewRows(rows.slice(0, 15)); // İlk 15 sətir önbaxış üçün

      // Sətirləri validasiya və ID-lərlə uyğunlaşdırma
      await matchAndValidateRows(rows);
    } catch (err) {
      setErrors([err.message]);
    } finally {
      setParsing(false);
    }
  };

  const matchAndValidateRows = async (rows) => {
    if (!lookups) return;
    const validationErrors = [];
    const validItems = [];

    // Kateqoriyalar üzrə brend, model və hissə növlərini keşləyirik ki, hər sətirdə təkrar API sorğusu getməsin
    const categoryCache = {};

    for (let i = 0; i < rows.length; i++) {
      const row = rows[i];
      const rowNum = i + 2;

      // Sütun adları (həm azərbaycan, həm ingilis başlıqları dəstəkləyirik)
      const catName = (row['Kateqoriya'] || row['Category'] || '').toString().trim();
      const brandName = (row['Brend'] || row['Brand'] || '').toString().trim();
      const modelName = (row['Model'] || row['DeviceModel'] || '').toString().trim();
      const partTypeName = (row['Hissə Növü'] || row['PartType'] || row['Hisse Novu'] || '').toString().trim();
      const qualityName = (row['Keyfiyyət'] || row['Quality'] || row['Keyfiyyet'] || 'Orijinal').toString().trim();
      const priceVal = parseFloat(String(row['Qiymət (AZN)'] || row['Qiymət'] || row['Price'] || '0').replace(',', '.'));
      const stockVal = parseInt(String(row['Stok Sayı'] || row['Stok'] || row['Stock'] || '1'), 10);
      const skuVal = (row['Artikul / SKU (İstəyə bağlı)'] || row['Artikul'] || row['SKU'] || '').toString().trim();
      const warrantyVal = parseInt(String(row['Zəmanət (Ay)'] || row['Zəmanət'] || row['Warranty'] || '0'), 10);
      const descVal = (row['Qeyd / Təsvir'] || row['Təsvir'] || row['Description'] || '').toString().trim();

      if (!brandName || !modelName || !partTypeName) {
        validationErrors.push(`Sətir ${rowNum}: Brend, Model və Hissə növü mütləq qeyd edilməlidir.`);
        continue;
      }

      if (!(priceVal > 0)) {
        validationErrors.push(`Sətir ${rowNum}: Qiymət 0-dan böyük olmalıdır.`);
        continue;
      }

      // Kateqoriya tapmaq (əgər boşdursa varsayılan Telefon)
      const matchedCat = lookups.categories?.find(c => 
        c.name.toLowerCase() === (catName || 'telefon').toLowerCase()
      ) || lookups.categories?.[0];

      if (!matchedCat) {
        validationErrors.push(`Sətir ${rowNum}: "${catName}" kateqoriyası sistemdə tapılmadı.`);
        continue;
      }

      // Həmin kateqoriyanın məlumatlarını keşə yükləyirik
      if (!categoryCache[matchedCat.id]) {
        try {
          const [catBrands, catPartTypes] = await Promise.all([
            Api.brandsByCategory(matchedCat.id),
            Api.partTypesByCategory(matchedCat.id)
          ]);
          categoryCache[matchedCat.id] = { brands: catBrands || [], partTypes: catPartTypes || [], models: {} };
        } catch {
          categoryCache[matchedCat.id] = { brands: [], partTypes: [], models: {} };
        }
      }

      const cache = categoryCache[matchedCat.id];

      // Brend tapmaq
      const matchedBrand = cache.brands.find(b => b.name.toLowerCase() === brandName.toLowerCase());
      if (!matchedBrand) {
        validationErrors.push(`Sətir ${rowNum}: "${brandName}" brendi ${matchedCat.name} kateqoriyasında tapılmadı.`);
        continue;
      }

      // Modelləri keşə yükləyirik
      if (!cache.models[matchedBrand.id]) {
        try {
          const bModels = await Api.modelsByBrand(matchedCat.id, matchedBrand.id);
          cache.models[matchedBrand.id] = bModels || [];
        } catch {
          cache.models[matchedBrand.id] = [];
        }
      }

      // Model tapmaq
      const matchedModel = cache.models[matchedBrand.id].find(m => 
        m.name.toLowerCase() === modelName.toLowerCase() ||
        `${matchedBrand.name} ${m.name}`.toLowerCase() === modelName.toLowerCase() || (m.modelCode && m.modelCode.toLowerCase().includes(modelName.toLowerCase()))
      );
      if (!matchedModel) {
        validationErrors.push(`Sətir ${rowNum}: "${modelName}" modeli (${brandName}) tapılmadı.`);
        continue;
      }

      // Hissə növü tapmaq
      const cleanPtSearch = partTypeName.replace(/telefon\s+üçün\s+/gi, '').trim().toLowerCase();
      const matchedPt = cache.partTypes.find(pt => {
        const cleanPtName = pt.name.replace(/telefon\s+üçün\s+/gi, '').trim().toLowerCase();
        return cleanPtName.includes(cleanPtSearch) || cleanPtSearch.includes(cleanPtName);
      });
      if (!matchedPt) {
        validationErrors.push(`Sətir ${rowNum}: "${partTypeName}" hissə növü tapılmadı.`);
        continue;
      }

      // Keyfiyyət tapmaq
      const qLower = qualityName.toLowerCase();
      const matchedQuality = lookups.qualityTypes?.find(q => {
        const qName = q.name.toLowerCase();
        const qCode = (q.code || '').toLowerCase();
        if (qName.includes(qLower) || qLower.includes(qName)) return true;
        if ((qLower.includes('işlənmiş') || qLower.includes('islenmis') || qLower.includes('used') || qLower.includes('üstdən')) && (qCode.includes('used') || qName.includes('işlənmiş'))) return true;
        if ((qLower.includes('orijinal') || qLower.includes('original') || qLower.includes('zavod') || qLower.includes('yeni')) && !qLower.includes('işlənmiş') && (qCode.includes('new') || qName.includes('yeni'))) return true;
        if ((qLower.includes('a+') || qLower.includes('a-class') || qLower.includes('a class') || qLower.includes('servis') || qLower.includes('premium')) && (qCode.includes('a_class') || qName.includes('a-class'))) return true;
        if ((qLower.includes('b-class') || qLower.includes('b class') || qLower.includes('standart') || qLower.includes('dublikat')) && (qCode.includes('b_class') || qName.includes('b-class'))) return true;
        if ((qLower.includes('c-class') || qLower.includes('c class') || qLower.includes('ekonom')) && (qCode.includes('c_class') || qName.includes('c-class'))) return true;
        return false;
      }) || lookups.qualityTypes?.[0];

      validItems.push({
        deviceModelId: matchedModel.id,
        partTypeId: matchedPt.id,
        qualityTypeId: matchedQuality ? matchedQuality.id : 1,
        title: `${matchedBrand.name} ${matchedModel.name} ${matchedPt.name}`,
        description: descVal,
        sku: skuVal,
        price: priceVal,
        stockQuantity: isNaN(stockVal) || stockVal < 0 ? 1 : stockVal,
        warrantyMonths: isNaN(warrantyVal) || warrantyVal < 0 ? 0 : warrantyVal,
        minOrderQuantity: 1,
        currency: 'AZN'
      });
    }

    setErrors(validationErrors);
    setMatchedItems(validItems);
  };

  const handleImportSubmit = async () => {
    if (!matchedItems || matchedItems.length === 0) {
      alertModal('Yükləmək üçün düzgün məhsul sətirləri yoxdur. Zəhmət olmasa xətaları nəzərdən keçirin.', 'Xəbərdarlıq', 'warning');
      return;
    }

    setUploading(true);
    try {
      const res = await Api.warehouse.bulkImport(matchedItems);
      
      // Bildirişlər mərkəzinə uğurlu əlavə olunma bildirişi atırıq
      try {
        const u = JSON.parse(localStorage.getItem('ehisse_user') || '{}');
        if (u?.id) {
          addNotification(u.id, {
            title: 'Kütləvi yükləmə tamamlandı',
            message: `${res.addedCount || matchedItems.length} yeni elan əlavə edildi / yeniləndi.`,
            icon: 'bi-file-earmark-check',
            type: 'success',
            link: '/warehouse'
          });
        }
      } catch (e) {
        console.error(e);
      }

      await alertModal(res.message || 'Məhsullar uğurla yükləndi!', 'Uğurlu əməliyyat', 'success');
      onSuccess?.();
      onClose();
    } catch (err) {
      alertModal(err.message || 'Kütləvi yükləmə zamanı xəta baş verdi.', 'Xəta', 'error');
    } finally {
      setUploading(false);
    }
  };

  const resetAll = () => {
    setFile(null);
    setPreviewRows([]);
    setMatchedItems([]);
    setErrors([]);
    if (fileInputRef.current) fileInputRef.current.value = '';
  };

  return (
    <div className="modal-backdrop-custom d-flex align-items-center justify-content-center p-3" style={{ position: 'fixed', inset: 0, zIndex: 1050, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(5px)' }}>
      <div className="card shadow-lg w-100" style={{ maxWidth: '820px', maxHeight: '90vh', display: 'flex', flexDirection: 'column', background: 'var(--bg-surface)', border: '1px solid var(--border-color)', borderRadius: '16px' }}>
        
        {/* Başlıq */}
        <div className="p-3 border-bottom d-flex align-items-center justify-content-between" style={{ borderColor: 'var(--border-color)' }}>
          <div className="d-flex align-items-center gap-2">
            <div style={{ width: 36, height: 36, borderRadius: '8px', background: 'var(--accent)', color: '#000', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold' }}>
              <i className="bi bi-file-earmark-excel fs-5"></i>
            </div>
            <div>
              <h5 className="m-0 fw-bold text-main">Excel ilə Kütləvi Elan Yükləmə</h5>
              <small className="text-muted">1 kliklə yüzlərlə ehtiyat hissəsini sayta əlavə edin və ya qiymətləri yeniləyin</small>
            </div>
          </div>
          <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
        </div>

        {/* Gövdə */}
        <div className="p-3 overflow-auto flex-grow-1">
          {/* Nümunə Şablon İzahı */}
          <div className="p-3 mb-3 rounded d-flex align-items-center justify-content-between flex-wrap gap-2" style={{ background: 'var(--bg-surface-hover)', border: '1px dashed var(--border-color)' }}>
            <div>
              <strong className="text-main d-block">Hazır Nümunə Faylı (Şablon)</strong>
              <small className="text-muted">Sütunların dəqiq sırasını və nümunələri görmək üçün şablonu endirin</small>
            </div>
            <button type="button" onClick={downloadExcelTemplate} className="btn-outline d-flex align-items-center gap-2" style={{ background: 'var(--accent)', color: '#000', borderColor: 'var(--accent)', fontWeight: 600 }}>
              <i className="bi bi-download"></i> Nümunə Şablonu Endir (.xlsx)
            </button>
          </div>

          {/* Fayl Seçmə Sahəsi */}
          {!file ? (
            <div 
              className="p-5 text-center rounded border-2 border-dashed my-3"
              style={{ borderColor: 'var(--border-color)', cursor: 'pointer', background: 'var(--input-bg)' }}
              onClick={() => fileInputRef.current?.click()}
            >
              <i className="bi bi-cloud-arrow-up text-accent" style={{ fontSize: '3rem' }}></i>
              <h6 className="mt-2 text-main fw-bold">Excel və ya CSV faylınızı bura atın və ya klikləyin</h6>
              <p className="text-muted small mb-0">Dəstəklənən formatlar: .xlsx, .xls, .csv</p>
              <input 
                ref={fileInputRef} 
                type="file" 
                accept=".xlsx, .xls, .csv" 
                className="d-none" 
                onChange={handleFileChange} 
              />
            </div>
          ) : (
            <div className="mb-3">
              <div className="p-3 rounded d-flex align-items-center justify-content-between mb-3" style={{ background: 'var(--input-bg)', border: '1px solid var(--border-color)' }}>
                <div className="d-flex align-items-center gap-2">
                  <i className="bi bi-file-earmark-check text-success fs-3"></i>
                  <div>
                    <strong className="text-main d-block">{file.name}</strong>
                    <small className="text-muted">{(file.size / 1024).toFixed(1)} KB · {parsing ? 'Oxunur...' : `${matchedItems.length} təsdiqlənmiş məhsul`}</small>
                  </div>
                </div>
                <button type="button" className="btn btn-sm btn-outline-danger" onClick={resetAll} disabled={uploading}>
                  <i className="bi bi-x-circle me-1"></i> Başqa fayl seç
                </button>
              </div>

              {/* Status və Say */}
              <div className="row g-2 mb-3 text-center">
                <div className="col-4">
                  <div className="p-2 rounded" style={{ background: 'var(--bg-surface-hover)', border: '1px solid var(--border-color)' }}>
                    <small className="text-muted d-block">Cəmi Sətir</small>
                    <strong className="fs-5 text-main">{previewRows.length ? `${previewRows.length}+` : 0}</strong>
                  </div>
                </div>
                <div className="col-4">
                  <div className="p-2 rounded" style={{ background: 'rgba(40, 167, 69, 0.15)', border: '1px solid rgba(40, 167, 69, 0.3)' }}>
                    <small className="text-success d-block">Yüklənməyə Hazır</small>
                    <strong className="fs-5 text-success">{matchedItems.length}</strong>
                  </div>
                </div>
                <div className="col-4">
                  <div className="p-2 rounded" style={{ background: errors.length ? 'rgba(220, 53, 69, 0.15)' : 'var(--bg-surface-hover)', border: errors.length ? '1px solid rgba(220, 53, 69, 0.3)' : '1px solid var(--border-color)' }}>
                    <small className={errors.length ? 'text-danger d-block' : 'text-muted d-block'}>Xətalı Sətir</small>
                    <strong className={errors.length ? 'fs-5 text-danger' : 'fs-5 text-muted'}>{errors.length}</strong>
                  </div>
                </div>
              </div>

              {/* Xətalar Paneli */}
              {errors.length > 0 && (
                <div className="p-3 mb-3 rounded" style={{ background: 'rgba(220, 53, 69, 0.1)', border: '1px solid rgba(220, 53, 69, 0.25)', maxHeight: '140px', overflowY: 'auto' }}>
                  <strong className="text-danger small d-block mb-1">
                    <i className="bi bi-exclamation-triangle-fill me-1"></i> Aşağıdakı sətirlərdə uyğunsuzluq tapıldı (həmin sətirlər buraxılacaq):
                  </strong>
                  <ul className="m-0 ps-3 small text-danger">
                    {errors.slice(0, 10).map((err, idx) => (
                      <li key={idx}>{err}</li>
                    ))}
                    {errors.length > 10 && <li>... və daha {errors.length - 10} xəta</li>}
                  </ul>
                </div>
              )}

              {/* İlk bir neçə sətir üçün önbaxış cədvəli */}
              {matchedItems.length > 0 && (
                <div className="mb-2">
                  <strong className="text-muted small d-block mb-2">Yüklənəcək məhsulların önbaxışı (ilk nümunələr):</strong>
                  <div className="table-responsive rounded border" style={{ borderColor: 'var(--border-color)', maxHeight: '200px' }}>
                    <table className="table table-sm table-dark m-0" style={{ fontSize: '0.82rem' }}>
                      <thead>
                        <tr style={{ background: 'var(--bg-surface-hover)' }}>
                          <th>#</th>
                          <th>Elan Adı</th>
                          <th>Qiymət</th>
                          <th>Stok</th>
                          <th>SKU</th>
                        </tr>
                      </thead>
                      <tbody>
                        {matchedItems.slice(0, 5).map((item, idx) => (
                          <tr key={idx}>
                            <td>{idx + 1}</td>
                            <td>{item.title}</td>
                            <td className="text-accent fw-bold">{item.price} AZN</td>
                            <td>{item.stockQuantity} ədəd</td>
                            <td><span className="text-muted">{item.sku || '—'}</span></td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Düymələr */}
        <div className="p-3 border-top d-flex align-items-center justify-content-between" style={{ borderColor: 'var(--border-color)' }}>
          <button type="button" className="btn-outline" onClick={onClose} disabled={uploading}>
            Bağla
          </button>
          {file && (
            <button 
              type="button" 
              className="btn-outline bg-accent text-dark fw-bold d-flex align-items-center gap-2" 
              onClick={handleImportSubmit}
              disabled={uploading || parsing || matchedItems.length === 0}
            >
              {uploading ? (
                <>
                  <span className="spinner-border spinner-border-sm" role="status"></span>
                  Yüklənir...
                </>
              ) : (
                <>
                  <i className="bi bi-cloud-arrow-up-fill"></i>
                  {matchedItems.length} Məhsulu Sayta Yüklə
                </>
              )}
            </button>
          )}
        </div>

      </div>
    </div>
  );
}
