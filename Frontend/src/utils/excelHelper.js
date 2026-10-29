import * as XLSX from 'xlsx';

/**
 * Generates and downloads a clean Excel template for bulk spare part upload.
 */
export function downloadExcelTemplate() {
  const sampleData = [
    {
      'Kateqoriya': 'Telefon',
      'Brend': 'Apple',
      'Model': 'iPhone 11',
      'Hissə Növü': 'Ekranlar',
      'Keyfiyyət': 'Zavod istehsalı (yeni)',
      'Qiymət (AZN)': 120,
      'Stok Sayı': 5,
      'Artikul / SKU (İstəyə bağlı)': 'IP11-SCR-ORG',
      'Zəmanət (Ay)': 3,
      'Qeyd / Təsvir': 'Tam orijinal ekran, zəmanətlə'
    },
    {
      'Kateqoriya': 'Telefon',
      'Brend': 'Samsung',
      'Model': 'Galaxy A10',
      'Hissə Növü': 'Batareyalar',
      'Keyfiyyət': 'A-Class (Premium)',
      'Qiymət (AZN)': 25,
      'Stok Sayı': 10,
      'Artikul / SKU (İstəyə bağlı)': 'A10-BAT-01',
      'Zəmanət (Ay)': 1,
      'Qeyd / Təsvir': 'Yeni batareya'
    }
  ];

  const guideData = [
    { 'Sütun Adı': 'Kateqoriya', 'Məcburi': 'Bəli', 'İzah': 'Məsələn: Telefon, Planşet, Noutbuk' },
    { 'Sütun Adı': 'Brend', 'Məcburi': 'Bəli', 'İzah': 'Məsələn: Apple, Samsung, Xiaomi, Huawei və s.' },
    { 'Sütun Adı': 'Model', 'Məcburi': 'Bəli', 'İzah': 'Dəqiq model adı: iPhone 11, Galaxy A10, Redmi Note 11' },
    { 'Sütun Adı': 'Hissə Növü', 'Məcburi': 'Bəli', 'İzah': 'Ekranlar, Batareyalar, Şarj Portu, Arxa Qapaq və s.' },
    { 'Sütun Adı': 'Keyfiyyət', 'Məcburi': 'Bəli', 'İzah': 'Orijinal, A+ Servis, Dublikat, İşlənmiş Orijinal' },
    { 'Sütun Adı': 'Qiymət (AZN)', 'Məcburi': 'Bəli', 'İzah': 'Yalnız rəqəm (məs: 45 və ya 45.5)' },
    { 'Sütun Adı': 'Stok Sayı', 'Məcburi': 'Bəli', 'İzah': 'Anbardakı mövcud say (məs: 10)' },
    { 'Sütun Adı': 'Artikul / SKU', 'Məcburi': 'Xeyr', 'İzah': 'Öz daxili anbar kodunuz (boş qala bilər)' },
    { 'Sütun Adı': 'Zəmanət (Ay)', 'Məcburi': 'Xeyr', 'İzah': 'Ay sayı (0, 1, 3, 6 və s.)' },
    { 'Sütun Adı': 'Qeyd / Təsvir', 'Məcburi': 'Xeyr', 'İzah': 'Məhsul haqqında əlavə qeydlər' }
  ];

  const wb = XLSX.utils.book_new();
  const wsSample = XLSX.utils.json_to_sheet(sampleData);
  const wsGuide = XLSX.utils.json_to_sheet(guideData);

  // Sütun enləri
  wsSample['!cols'] = [
    { wch: 15 }, { wch: 15 }, { wch: 22 }, { wch: 18 }, { wch: 16 },
    { wch: 14 }, { wch: 12 }, { wch: 25 }, { wch: 14 }, { wch: 35 }
  ];

  wsGuide['!cols'] = [{ wch: 20 }, { wch: 12 }, { wch: 45 }];

  XLSX.utils.book_append_sheet(wb, wsSample, 'Elanlar');
  XLSX.utils.book_append_sheet(wb, wsGuide, 'Təlimat');

  XLSX.writeFile(wb, 'e-hisse_elan_numunesi.xlsx');
}

/**
 * Exports current warehouse parts to Excel.
 */
export function exportWarehouseToExcel(parts = [], companyName = 'Anbar') {
  if (!parts || parts.length === 0) return;

  const data = parts.map(p => ({
    'Elan ID': p.id,
    'Elan Adı': p.title,
    'Brend': p.brandName || '',
    'Model': p.modelName || '',
    'Hissə Növü': p.partTypeName || '',
    'Keyfiyyət': p.qualityTypeName || '',
    'Artikul (SKU)': p.sku || '',
    'Qiymət (AZN)': p.price,
    'Stok Sayı': p.stockQuantity,
    'Baxış Sayı': p.viewCount || 0,
    'Status': p.isActive ? 'Aktiv' : 'Deaktiv',
    'Moderasiya': p.moderationStatus === 'Approved' ? 'Təsdiqlənib' : p.moderationStatus === 'Pending' ? 'Gözləmədə' : 'İmtina edilib'
  }));

  const wb = XLSX.utils.book_new();
  const ws = XLSX.utils.json_to_sheet(data);
  ws['!cols'] = [
    { wch: 10 }, { wch: 35 }, { wch: 15 }, { wch: 20 }, { wch: 18 },
    { wch: 16 }, { wch: 18 }, { wch: 14 }, { wch: 12 }, { wch: 12 },
    { wch: 12 }, { wch: 15 }
  ];

  XLSX.utils.book_append_sheet(wb, ws, 'Elanlarım');
  const safeName = (companyName || 'anbar').replace(/[^a-z0-9]/gi, '_').toLowerCase();
  XLSX.writeFile(wb, `e-hisse_${safeName}_elanlar.xlsx`);
}

/**
 * Parses an uploaded Excel/CSV file into normalized rows.
 */
export async function parseExcelFile(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const data = new Uint8Array(e.target.result);
        const workbook = XLSX.read(data, { type: 'array' });
        const firstSheetName = workbook.SheetNames[0];
        const worksheet = workbook.Sheets[firstSheetName];
        const json = XLSX.utils.sheet_to_json(worksheet, { defval: '' });
        resolve(json);
      } catch (err) {
        reject(new Error('Excel faylı oxunarkən xəta baş verdi: ' + err.message));
      }
    };
    reader.onerror = () => reject(new Error('Fayl oxuna bilmədi.'));
    reader.readAsArrayBuffer(file);
  });
}


/**
 * Generates and downloads an Excel template for bulk Brand/Model catalog upload.
 */
export function downloadCatalogExcelTemplate() {
  const sampleData = [
    {
      'Category': 'Planşet',
      'Brand': 'Apple',
      'Model': 'iPad (10th generation)',
      'Model Numbers': 'A2696 / A2757 / A2777 / A3162'
    },
    {
      'Category': 'Planşet',
      'Brand': 'Apple',
      'Model': 'iPad Air 11-inch (M2)',
      'Model Numbers': 'A2902 / A2903 / A2904'
    },
    {
      'Category': 'Telefon',
      'Brand': 'Samsung',
      'Model': 'Galaxy S24 Ultra',
      'Model Numbers': 'SM-S928B / SM-S928U'
    }
  ];

  const wb = XLSX.utils.book_new();
  const ws = XLSX.utils.json_to_sheet(sampleData);
  ws['!cols'] = [{ wch: 18 }, { wch: 18 }, { wch: 32 }, { wch: 36 }];
  XLSX.utils.book_append_sheet(wb, ws, 'Modeller');
  XLSX.writeFile(wb, 'e-hisse_kataloq_numunesi.xlsx');
}

/**
 * Exports all catalog models to Excel.
 */
export function exportCatalogModelsToExcel(models = []) {
  if (!models || models.length === 0) return;
  const data = models.map(m => ({
    'Category': m.categoryName || '',
    'Brand': m.brandName || '',
    'Model': m.name || '',
    'Model Numbers': m.modelCode || '',
    'Status': m.isActive ? 'Aktiv' : 'Deaktiv'
  }));
  const wb = XLSX.utils.book_new();
  const ws = XLSX.utils.json_to_sheet(data);
  ws['!cols'] = [{ wch: 20 }, { wch: 18 }, { wch: 35 }, { wch: 36 }, { wch: 12 }];
  XLSX.utils.book_append_sheet(wb, ws, 'Modeller');
  XLSX.writeFile(wb, 'e-hisse_butun_modeller.xlsx');
}
