import React from 'react';
import { usePageSeo } from '../utils/seo';

function Terms() {
  usePageSeo({
    title: 'İstifadə Şərtləri',
    description: 'e-hisse.az platformasının istifadə qaydaları və şərtləri.'
  });

  return (
    <div className="container py-4" style={{ maxWidth: '860px' }}>
      <div className="product-card p-4 p-md-5">
        <h1 className="section-title mb-4">İstifadə Şərtləri (Kullanım Koşulları)</h1>
        <p className="text-muted">Son yenilənmə tarixi: 14 Oktyabr 2026</p>

        <section className="mb-4">
          <h4 className="text-main">1. Qəbul edilmə</h4>
          <p className="text-muted">
            «e-hisse.az» platformasında qeydiyyatdan keçməklə və ya xidmətlərdən istifadə etməklə bu İstifadə Şərtlərini tam qəbul etmiş olursunuz.
          </p>
        </section>

        <section className="mb-4">
          <h4 className="text-main">2. Satıcı və Usta Öhdəlikləri</h4>
          <p className="text-muted">
            Yerləşdirilən ehtiyat hissələrinin qiyməti, keyfiyyət tipi (Orijinal, Servis, OEM, A klass) və stok vəziyyəti dəqiq göstərilməlidir.
            Yanıltıcı məlumat verən və ya saxta elan yerləşdirən hesablar xəbərdarlıq edilmədən bloklana bilər.
          </p>
        </section>

        <section className="mb-4">
          <h4 className="text-main">3. Məsuliyyətin məhdudlaşdırılması</h4>
          <p className="text-muted">
            Platforma alıcı və satıcı arasında vasitəçi məlumat bazası rolunu oynayır. Alqı-satqı zamanı tərəflər arasındakı ödəniş və təhvil-təslim razılaşması birbaşa tərəflərin öz məsuliyyətindədir.
          </p>
        </section>

        <section>
          <h4 className="text-main">4. Dəyişikliklər</h4>
          <p className="text-muted">
            «e-hisse.az» bu şərtləri istənilən vaxt yeniləmək hüququnu özündə saxlayır.
          </p>
        </section>
      </div>
    </div>
  );
}

export default Terms;
