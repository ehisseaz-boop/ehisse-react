const fs = require('fs');
const parts = [];
const images = [];
const models = [
  {id: 1, name: 'iPhone 15 Pro Max', brand: 'Apple'},
  {id: 2, name: 'iPhone 15 Pro', brand: 'Apple'},
  {id: 3, name: 'iPhone 14 Pro Max', brand: 'Apple'},
  {id: 4, name: 'iPhone 13', brand: 'Apple'},
  {id: 5, name: 'Galaxy S24 Ultra', brand: 'Samsung'},
  {id: 6, name: 'Galaxy S23', brand: 'Samsung'},
  {id: 7, name: 'Galaxy A54', brand: 'Samsung'},
  {id: 8, name: 'Xiaomi 13 Pro', brand: 'Xiaomi'},
  {id: 9, name: 'Redmi Note 12 Pro', brand: 'Xiaomi'},
  {id: 10, name: 'P60 Pro', brand: 'Huawei'},
  {id: 11, name: 'iPad Pro 12.9 (2024)', brand: 'Apple'},
  {id: 12, name: 'iPad Air (2024)', brand: 'Apple'},
  {id: 13, name: 'Galaxy Tab S9 Ultra', brand: 'Samsung'},
  {id: 14, name: 'MacBook Pro 14 (M3)', brand: 'Apple'},
  {id: 15, name: 'MacBook Air 13 (M2)', brand: 'Apple'},
  {id: 16, name: 'XPS 13 Plus', brand: 'Dell'},
  {id: 17, name: 'XPS 15', brand: 'Dell'}
];
const partTypes = [
  {id: 1, name: 'Ekran'},
  {id: 2, name: 'Batareya'},
  {id: 3, name: 'Arxa Kamera'},
  {id: 5, name: 'Ana Plata'},
  {id: 11, name: 'Klaviatura'}
];
const qualities = ['Orijinal Zavod', 'A-Class', 'B-Class', 'İşlənmiş Orijinal'];

let imageId = 1;

for(let i = 1; i <= 100; i++) {
  const model = models[i % models.length];
  const type = partTypes[i % partTypes.length];
  const qual = qualities[i % qualities.length];
  const seller = (i % 3) + 1;
  const qId = (i % 4) + 1;
  
  parts.push(`new SparePart { Id = ${i}, SKU = "SKU-${i}", SellerId = ${seller}, DeviceModelId = ${model.id}, PartTypeId = ${type.id}, QualityTypeId = ${qId}, Title = "${model.brand} ${model.name} ${type.name} - ${qual}", Description = "Yüksək keyfiyyətli ehtiyat hissəsi.", Price = ${10 + (i * 3) % 150}.00m, Currency = "AZN", StockQuantity = ${i % 20 + 1}, MinOrderQuantity = 1, WarrantyMonths = ${i % 12}, ViewCount = ${i * 13 % 300}, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1) }`);
  
  images.push(`new SparePartImage { Id = ${imageId++}, SparePartId = ${i}, ImageUrl = "/images/images (${(i % 7) + 1}).jpg", IsMain = true, DisplayOrder = 1 }`);
}

fs.writeFileSync('seed_parts.txt', parts.join(',\n'));
fs.writeFileSync('seed_images.txt', images.join(',\n'));
