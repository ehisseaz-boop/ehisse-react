import React from 'react';
import { usePageSeo } from '../utils/seo';

function Privacy() {
  usePageSeo({
    title: 'Məxfilik Siyasəti',
    description: 'e-hisse.az platformasının məxfilik siyasəti və istifadəçi məlumatlarının qorunması qaydaları.'
  });

  return (
    <div className="container py-4" style={{ maxWidth: '860px' }}>
      <div className="product-card p-4 p-md-5">
        <h1 className="section-title mb-4">Məxfilik Siyasəti (Gizlilik Politikası)</h1>
        <p className="text-muted">Son yenilənmə tarixi: 14 Oktyabr 2026</p>

        <section className="mb-4">
          <h4 className="text-main">1. Ümumi müddəalar</h4>
          <p className="text-muted">
            «e-hisse.az» platforması olaraq istifadəçilərimizin şəxsi məlumatlarının təhlükəsizliyinə və məxfiliyinə xüsusi önəm veririk.
            Bu Məxfilik Siyasəti xidmətlərimizdən istifadə edərkən hansı məlumatların toplandığını və necə qorunduğunu izah edir.
          </p>
        </section>

        <section className="mb-4">
          <h4 className="text-main">2. Toplanan məlumatlar</h4>
          <p className="text-muted">
            Xidmətdən qeydiyyatdan keçərkən ad, soyad, əlaqə nömrəsi, e-poçt ünvanı və mağaza/anbar məlumatları toplanır.
            Həmçinin sistemin təhlükəsizliyi üçün IP ünvanları və sistem jurnalları (logs) qeydə alınır.
          </p>
        </section>

        <section className="mb-4">
          <h4 className="text-main">3. Məlumatların istifadəsi</h4>
          <p className="text-muted">
            Toplanan məlumatlar elanların yerləşdirilməsi, alıcı və satıcı arasında əlaqənin yaradılması,
            istifadəçi hesabının qorunması və xidmət keyfiyyətinin artırılması məqsədilə istifadə olunur.
          </p>
        </section>

        <section className="mb-4">
          <h4 className="text-main">4. Çərəzlər (Cookies)</h4>
          <p className="text-muted">
            Saytımızda istifadəçi təcrübəsini təkmilləşdirmək və daxil olma sessiyalarını təmin etmək üçün təhlükəsiz çərəzlərdən istifadə olunur.
          </p>
        </section>

        <section>
          <h4 className="text-main">5. Əlaqə</h4>
          <p className="text-muted">
            Məxfilik siyasəti ilə bağlı hər hansı sualınız olarsa, <a href="mailto:info@e-hisse.az" className="link-accent">info@e-hisse.az</a> ünvanı vasitəsilə bizimlə əlaqə saxlaya bilərsiniz.
          </p>
        </section>
      </div>
    </div>
  );
}

export default Privacy;
