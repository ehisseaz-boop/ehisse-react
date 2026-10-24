import React, { useState, useEffect } from 'react';
import { Api } from '../api';
import { cleanPartTypeName } from '../pages/Home';

// İkon təyini köməkçisi
export function getCategoryIcon(name = '') {
  const n = name.toLowerCase();
  if (n.includes('telefon') || n.includes('smartfon')) return 'bi-phone';
  if (n.includes('planşet') || n.includes('planset')) return 'bi-tablet';
  if (n.includes('noutbuk') || n.includes('laptop')) return 'bi-laptop';
  if (n.includes('saat')) return 'bi-smartwatch';
  if (n.includes('qulaq')) return 'bi-headphones';
  if (n.includes('kompüter') || n.includes('pc')) return 'bi-pc-display';
  if (n.includes('televizor') || n.includes('tv')) return 'bi-tv';
  if (n.includes('konsol') || n.includes('oyun')) return 'bi-controller';
  if (n.includes('kamera') || n.includes('foto')) return 'bi-camera';
  if (n.includes('alət') || n.includes('təmir')) return 'bi-tools';
  if (n.includes('dron')) return 'bi-send';
  if (n.includes('samokat') || n.includes('skuter')) return 'bi-bicycle';
  if (n.includes('şəbəkə') || n.includes('sebeke')) return 'bi-router';
  if (n.includes('adapter') || n.includes('kabel')) return 'bi-plug';
  return 'bi-cpu';
}

export function getPartTypeIcon(name = '') {
  const n = name.toLowerCase();
  if (n.includes('ekran')) return 'bi-display';
  if (n.includes('batarey')) return 'bi-battery-charging';
  if (n.includes('kamera')) return 'bi-camera';
  if (n.includes('motherboard') || n.includes('ana plata')) return 'bi-motherboard';
  if (n.includes('qapaq') || n.includes('korpus')) return 'bi-shield-shaded';
  if (n.includes('dinamik') || n.includes('zəng') || n.includes('səs')) return 'bi-speaker';
  if (n.includes('charging') || n.includes('şarj') || n.includes('port')) return 'bi-lightning-charge';
  if (n.includes('sim')) return 'bi-sim';
  if (n.includes('düymə') || n.includes('button')) return 'bi-toggle-on';
  if (n.includes('klaviatura') || n.includes('keyboard')) return 'bi-keyboard';
  if (n.includes('touchpad')) return 'bi-mouse';
  if (n.includes('ram') || n.includes('yaddaş')) return 'bi-memory';
  if (n.includes('adapter')) return 'bi-plug';
  if (n.includes('sensor') || n.includes('şüşə')) return 'bi-aspect-ratio';
  if (n.includes('mikrofon')) return 'bi-mic';
  return 'bi-gear-wide-connected';
}

export default function VisualSearchModal({
  open,
  onClose,
  categories = [],
  onSelectResult
}) {
  // Step 1: Kateqoriya seçimi
  // Step 2: Brend seçimi (Kateqoriya daxilində)
  // Step 3: Model seçimi (Brend daxilində)
  // Step 4: Hissə növü seçimi
  const [step, setStep] = useState(1);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [selectedBrand, setSelectedBrand] = useState(null);
  const [selectedModel, setSelectedModel] = useState(null);
  const [selectedPartType, setSelectedPartType] = useState(null);

  const [brands, setBrands] = useState([]);
  const [models, setModels] = useState([]);
  const [partTypes, setPartTypes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchFilter, setSearchFilter] = useState('');

  // Kateqoriya seçildikdə brendləri və hissə növlərini gətir
  const handleCategoryClick = async (cat) => {
    setSelectedCategory(cat);
    setSelectedBrand(null);
    setSelectedModel(null);
    setSelectedPartType(null);
    setSearchFilter('');
    setLoading(true);
    setStep(2);

    try {
      const [bData, ptData] = await Promise.all([
        Api.brandsByCategory(cat.id),
        Api.partTypesByCategory(cat.id)
      ]);
      setBrands(bData || []);
      setPartTypes(ptData || []);
    } catch (err) {
      console.error(err);
      setBrands([]);
      setPartTypes([]);
    } finally {
      setLoading(false);
    }
  };

  // Brend seçildikdə modelləri gətir
  const handleBrandClick = async (b) => {
    setSelectedBrand(b);
    setSelectedModel(null);
    setSearchFilter('');
    setLoading(true);
    setStep(3);

    try {
      const mData = await Api.modelsByBrand(selectedCategory.id, b.id);
      setModels(mData || []);
    } catch (err) {
      console.error(err);
      setModels([]);
    } finally {
      setLoading(false);
    }
  };

  // Model seçildikdə Hissə növü mərhələsinə keç
  const handleModelClick = (m) => {
    setSelectedModel(m);
    setSearchFilter('');
    setStep(4);
  };

  // Nəticəni tətbiq et (İstənilən mərhələdən "Bütün elanlar" seçmək mümkündür)
  const finishSelection = (extra = {}) => {
    onSelectResult?.({
      categoryId: selectedCategory?.id || null,
      brandId: extra.brandId !== undefined ? extra.brandId : (selectedBrand?.id || null),
      modelId: extra.modelId !== undefined ? extra.modelId : (selectedModel?.id || null),
      partTypeId: extra.partTypeId !== undefined ? extra.partTypeId : (selectedPartType?.id || null),
    });
    onClose();
  };

  const handleBack = () => {
    setSearchFilter('');
    if (step === 4) setStep(3);
    else if (step === 3) setStep(2);
    else if (step === 2) setStep(1);
    else onClose();
  };

  if (!open) return null;

  return (
    <div className="visual-catalog-backdrop" onClick={onClose}>
      <div className="visual-catalog-modal" onClick={e => e.stopPropagation()}>
        {/* Başlıq və Naviqasiya Barı */}
        <div className="visual-catalog-header">
          <div className="d-flex align-items-center gap-2">
            {step > 1 ? (
              <button type="button" className="btn btn-icon" onClick={handleBack} title="Geri">
                <i className="bi bi-arrow-left"></i>
              </button>
            ) : (
              <div className="catalog-header-badge">
                <i className="bi bi-grid-fill"></i>
              </div>
            )}
            <div>
              <h5 className="mb-0 fw-bold">
                {step === 1 && 'Kataloq'}
                {step === 2 && selectedCategory?.name}
                {step === 3 && `${selectedCategory?.name} › ${selectedBrand?.name}`}
                {step === 4 && `${selectedBrand?.name} ${selectedModel?.name} › Hissə növü`}
              </h5>
              <div className="text-muted small" style={{ fontSize: '12px' }}>
                {step === 1 && 'Axtarmaq istədiyiniz cihaz kateqoriyasını seçin'}
                {step === 2 && 'Brend / Marka seçin'}
                {step === 3 && 'Cihaz modelini seçin'}
                {step === 4 && 'Ehtiyat hissəsi növünü seçin'}
              </div>
            </div>
          </div>
          <button type="button" className="btn-close" onClick={onClose}></button>
        </div>

        {/* Mərhələ 2, 3 və 4 üçün Sürətli filtr qutusu */}
        {step > 1 && (
          <div className="p-3 border-bottom bg-surface-subtle">
            <div className="input-group input-group-sm">
              <span className="input-group-text bg-transparent border-end-0">
                <i className="bi bi-search text-muted"></i>
              </span>
              <input
                type="text"
                className="form-control border-start-0"
                placeholder={step === 2 ? "Brend axtar..." : step === 3 ? "Model axtar..." : "Hissə növü axtar..."}
                value={searchFilter}
                onChange={e => setSearchFilter(e.target.value)}
                autoFocus
              />
              {searchFilter && (
                <button className="btn btn-outline-secondary" type="button" onClick={() => setSearchFilter('')}>
                  <i className="bi bi-x"></i>
                </button>
              )}
            </div>
          </div>
        )}

        {/* Gövdə: Siyahı elementi (Tap.az üslubunda oxlu sətirlər və ikonlar) */}
        <div className="visual-catalog-body">
          {loading ? (
            <div className="text-center py-5 text-muted">
              <div className="spinner-border text-accent spinner-border-sm me-2"></div>
              Yüklənir...
            </div>
          ) : (
            <>
              {/* MƏRHƏLƏ 1: KATEQORİYALAR */}
              {step === 1 && (
                <div className="visual-list">
                  {/* Bütün elanlar seçimi */}
                  <div 
                    className="visual-list-item special"
                    onClick={() => {
                      onSelectResult?.({ categoryId: null, brandId: null, modelId: null, partTypeId: null });
                      onClose();
                    }}
                  >
                    <div className="visual-item-icon bg-accent-subtle">
                      <i className="bi bi-collection-fill text-accent"></i>
                    </div>
                    <div className="visual-item-content">
                      <div className="visual-item-title fw-bold">Bütün elanlar</div>
                      <div className="visual-item-desc">Bütün kateqoriyalar üzrə axtarış</div>
                    </div>
                    <i className="bi bi-chevron-right visual-item-arrow"></i>
                  </div>

                  {categories.map(cat => (
                    <div
                      key={cat.id}
                      className="visual-list-item"
                      onClick={() => handleCategoryClick(cat)}
                    >
                      <div className="visual-item-icon">
                        <i className={`bi ${getCategoryIcon(cat.name)}`}></i>
                      </div>
                      <div className="visual-item-content">
                        <div className="visual-item-title">{cat.name}</div>
                        <div className="visual-item-desc">
                          {cat.count ? `${cat.count} aktiv elan` : 'Ehtiyat hissələri və detallar'}
                        </div>
                      </div>
                      <i className="bi bi-chevron-right visual-item-arrow"></i>
                    </div>
                  ))}
                </div>
              )}

              {/* MƏRHƏLƏ 2: BRENDLƏR */}
              {step === 2 && (
                <div className="visual-list">
                  {/* Bütün bu kateqoriyadakı elanlar */}
                  <div
                    className="visual-list-item special"
                    onClick={() => finishSelection({ brandId: null, modelId: null, partTypeId: null })}
                  >
                    <div className="visual-item-icon bg-accent-subtle">
                      <i className="bi bi-check2-all text-accent"></i>
                    </div>
                    <div className="visual-item-content">
                      <div className="visual-item-title fw-bold">Bütün {selectedCategory?.name} elanları</div>
                      <div className="visual-item-desc">Bütün brendlər üzrə bax</div>
                    </div>
                    <i className="bi bi-chevron-right visual-item-arrow"></i>
                  </div>

                  {brands
                    .filter(b => !searchFilter || b.name.toLowerCase().includes(searchFilter.toLowerCase()))
                    .map(b => (
                      <div
                        key={b.id}
                        className="visual-list-item"
                        onClick={() => handleBrandClick(b)}
                      >
                        <div className="visual-item-icon">
                          <i className="bi bi-tag-fill text-muted"></i>
                        </div>
                        <div className="visual-item-content">
                          <div className="visual-item-title">{b.name}</div>
                        </div>
                        <i className="bi bi-chevron-right visual-item-arrow"></i>
                      </div>
                    ))}
                  {brands.length === 0 && (
                    <div className="text-center py-4 text-muted small">Bu kateqoriyada brend tapılmadı.</div>
                  )}
                </div>
              )}

              {/* MƏRHƏLƏ 3: MODELLƏR */}
              {step === 3 && (
                <div className="visual-list">
                  {/* Bütün bu brenddəki elanlar */}
                  <div
                    className="visual-list-item special"
                    onClick={() => finishSelection({ modelId: null, partTypeId: null })}
                  >
                    <div className="visual-item-icon bg-accent-subtle">
                      <i className="bi bi-check2-all text-accent"></i>
                    </div>
                    <div className="visual-item-content">
                      <div className="visual-item-title fw-bold">Bütün {selectedBrand?.name} modelləri</div>
                      <div className="visual-item-desc">Brend üzrə bütün modelləri göstər</div>
                    </div>
                    <i className="bi bi-chevron-right visual-item-arrow"></i>
                  </div>

                  {models
                    .filter(m => !searchFilter || m.name.toLowerCase().includes(searchFilter.toLowerCase()) || (m.modelCode && m.modelCode.toLowerCase().includes(searchFilter.toLowerCase())))
                    .map(m => (
                      <div
                        key={m.id}
                        className="visual-list-item"
                        onClick={() => handleModelClick(m)}
                      >
                        <div className="visual-item-icon">
                          <i className="bi bi-phone text-muted"></i>
                        </div>
                        <div className="visual-item-content">
                          <div className="visual-item-title">{m.name}</div>
                          {m.modelCode && m.modelCode !== 'OTHER' && (
                            <div className="visual-item-desc">Model kodu: {m.modelCode || m.releaseYear}</div>
                          )}
                        </div>
                        <i className="bi bi-chevron-right visual-item-arrow"></i>
                      </div>
                    ))}
                  {models.length === 0 && (
                    <div className="text-center py-4 text-muted small">Bu brend üçün model tapılmadı.</div>
                  )}
                </div>
              )}

              {/* MƏRHƏLƏ 4: HİSSƏ NÖVÜ */}
              {step === 4 && (
                <div className="visual-list">
                  {/* Bu model üçün bütün hissələr */}
                  <div
                    className="visual-list-item special"
                    onClick={() => finishSelection({ partTypeId: null })}
                  >
                    <div className="visual-item-icon bg-accent-subtle">
                      <i className="bi bi-check2-all text-accent"></i>
                    </div>
                    <div className="visual-item-content">
                      <div className="visual-item-title fw-bold">{selectedBrand?.name} {selectedModel?.name} üçün bütün hissələr</div>
                      <div className="visual-item-desc">Bütün ehtiyat hissələrini göstər</div>
                    </div>
                    <i className="bi bi-chevron-right visual-item-arrow"></i>
                  </div>

                  {partTypes
                    .filter(p => !searchFilter || p.name.toLowerCase().includes(searchFilter.toLowerCase()))
                    .map(p => (
                      <div
                        key={p.id}
                        className="visual-list-item"
                        onClick={() => finishSelection({ partTypeId: p.id })}
                      >
                        <div className="visual-item-icon">
                          <i className={`bi ${getPartTypeIcon(p.name)}`}></i>
                        </div>
                        <div className="visual-item-content">
                          <div className="visual-item-title">{cleanPartTypeName(p.name)}</div>
                        </div>
                        <i className="bi bi-check2 visual-item-arrow text-accent"></i>
                      </div>
                    ))}
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}
