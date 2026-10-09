import re
import random
from datetime import datetime

# Read the file
file_path = r'C:\Users\user\Desktop\e-hisseler\Backend\Models\AppDbContext.cs'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Define categories and their models
cat_models = {
    1: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], # Telefon
    2: [11, 12, 13], # Planshet
    3: [14, 15, 16, 17] # Noutbuk
}

# Define part types for each category
cat_part_types = {
    1: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 16, 17, 18, 19, 20, 21, 22],
    2: [1, 2, 3, 4, 5, 6, 8, 10],
    3: [1, 2, 5, 11, 12, 13, 14, 15]
}

# Quality types
qualities = [1, 2, 3, 4, 5]

# Sellers
sellers = [1, 2, 3]

# Device model names mapping (approximate for titles)
model_names = {
    1: "iPhone 15 Pro Max", 2: "iPhone 15 Pro", 3: "iPhone 14 Pro Max", 4: "iPhone 13",
    5: "Galaxy S24 Ultra", 6: "Galaxy S23", 7: "Galaxy A54",
    8: "Xiaomi 13 Pro", 9: "Redmi Note 12 Pro", 10: "P60 Pro",
    11: "iPad Pro 12.9", 12: "iPad Air", 13: "Galaxy Tab S9 Ultra",
    14: "MacBook Pro 14", 15: "MacBook Air 13", 16: "XPS 13 Plus", 17: "XPS 15"
}

part_names = {
    1: "Ekran", 2: "Batareya", 3: "Arxa Kamera", 4: "Ön Kamera", 5: "Ana Plata",
    6: "Arxa Qapaq", 7: "Dinamik", 8: "Şarj Portu", 9: "Sim Tutucu", 10: "Home Düyməsi",
    11: "Klaviatura", 12: "Touchpad", 13: "RAM", 14: "Yaddaş", 15: "Adapter",
    16: "Sensor Şüşə", 17: "Antenna", 18: "Dinamik Setkası", 19: "Korpus Hissəsi",
    20: "Korpus", 21: "Mikrofon", 22: "OCA Film"
}

quality_names = {
    1: "Orijinal Zavod", 2: "İşlənmiş Orijinal", 3: "A-Class", 4: "B-Class", 5: "C-Class"
}

parts = []
part_id = 1

random.seed(42)

for cat_id in [1, 2, 3]:
    for _ in range(35):
        model_id = random.choice(cat_models[cat_id])
        part_type = random.choice(cat_part_types[cat_id])
        quality = random.choice(qualities)
        seller = random.choice(sellers)
        
        title = f"{model_names[model_id]} {part_names[part_type]} - {quality_names[quality]}"
        desc = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı."
        price = round(random.uniform(10.0, 150.0), 2)
        stock = random.randint(1, 20)
        views = random.randint(10, 500)
        warranty = random.choice([0, 1, 3, 6, 12])
        
        part_str = f'                new SparePart {{ Id = {part_id}, SKU = "SKU-{part_id}", SellerId = {seller}, DeviceModelId = {model_id}, PartTypeId = {part_type}, QualityTypeId = {quality}, Title = "{title}", Description = "{desc}", Price = {price}m, Currency = "AZN", StockQuantity = {stock}, MinOrderQuantity = 1, WarrantyMonths = {warranty}, ViewCount = {views}, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] }}'
        parts.append(part_str)
        part_id += 1

parts_block = "            modelBuilder.Entity<SparePart>().HasData(\n" + ",\n".join(parts) + "\n            );"

# Regex replacement
# We look for: modelBuilder.Entity<SparePart>().HasData( ... );
pattern = re.compile(r'modelBuilder\.Entity<SparePart>\(\)\.HasData\([\s\S]*?\n\s*\);', re.MULTILINE)

new_content = pattern.sub(parts_block, content)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(new_content)

print(f"Generated {len(parts)} products and updated AppDbContext.cs")
