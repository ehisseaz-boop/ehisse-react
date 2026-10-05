import sqlite3
import random
from datetime import datetime

conn = sqlite3.connect('ehisseler.db')
cursor = conn.cursor()

# Get existing IDs
cursor.execute("SELECT Id FROM SellerProfiles LIMIT 1")
seller_id_row = cursor.fetchone()
seller_id = seller_id_row[0] if seller_id_row else 1

cursor.execute("SELECT Id FROM DeviceModels LIMIT 1")
model_id_row = cursor.fetchone()
model_id = model_id_row[0] if model_id_row else 1

cursor.execute("SELECT Id FROM SparePartTypes LIMIT 1")
part_type_id_row = cursor.fetchone()
part_type_id = part_type_id_row[0] if part_type_id_row else 1

cursor.execute("SELECT Id FROM QualityTypes LIMIT 1")
quality_id_row = cursor.fetchone()
quality_id = quality_id_row[0] if quality_id_row else 1

# 1. Insert 30 categories
categories_data = []
base_categories = ["Ekranlar", "Batareyalar", "Ana platalar", "Klaviaturalar", "Kamera", "Dinamiklər", "Səs düymələri", "Korpuslar", "Adapterlər", "Kabellər", "Yaddaş kartları", "RAM", "Prosessorlar", "Kulerlər", "Soyuducu sistemlər", "Sensörlər", "Mikrofonlar", "Wi-Fi modulları", "Bluetooth modulları", "NFC antennaları", "Şarj yuvaları", "Qulaqlıq yuvaları", "Düymələr", "Şüşələr", "Plastik hissələr", "Konnektorlar", "Şleyflər", "Vintlə", "Alətlər dəsti", "Lentalar", "Qoruyucu şüşələr", "Kılıflar", "Powerbanklar", "Qapaqlar", "Çərçivələr"]

for i in range(30):
    name = f"{base_categories[i % len(base_categories)]} - {i+1}"
    categories_data.append((name, i+1, 1))

cursor.executemany("INSERT INTO Categories (Name, DisplayOrder, IsActive) VALUES (?, ?, ?)", categories_data)

# 2. Insert 100 spare parts
parts_data = []
adjectives = ["Orijinal", "Yüksək keyfiyyətli", "Yeni", "İşlənmiş", "Ucuz", "Premium", "Zəmanətli", "Test edilmiş", "A klass", "B klass"]
items = ["Ekran", "Batareya", "Ana plata", "Şarj yuvası", "Kamera", "Dinamik", "Mikrofon", "Korpus", "Şleyf", "Düymə"]

for i in range(100):
    title = f"{random.choice(adjectives)} {random.choice(items)} #{i+1}"
    desc = f"Bu, {title} üçün ətraflı məlumatdır. Çox keyfiyyətli məhsuldur."
    sku = f"SKU-{random.randint(1000, 9999)}-{i+1}"
    price = str(round(random.uniform(10.0, 200.0), 2))
    stock = random.randint(1, 100)
    now_str = datetime.now().isoformat()
    
    parts_data.append((
        seller_id,
        model_id,
        part_type_id,
        quality_id,
        title,
        desc,
        sku,
        price, # Price is TEXT in this schema due to EF Core decimals in SQLite
        "AZN",
        stock,
        1, # MinOrderQuantity
        random.randint(0, 12), # WarrantyMonths
        random.randint(0, 500), # ViewCount
        2, # ModerationStatus (2 = Approved)
        1, # IsActive
        0, # IsDeleted
        now_str,
        now_str,
        b'' # RowVersion
    ))

cursor.executemany("""
    INSERT INTO SpareParts (
        SellerId, DeviceModelId, PartTypeId, QualityTypeId, Title, Description, SKU, 
        Price, Currency, StockQuantity, MinOrderQuantity, WarrantyMonths, ViewCount, 
        ModerationStatus, IsActive, IsDeleted, CreatedDate, LastModifiedDate, RowVersion
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
""", parts_data)

conn.commit()
conn.close()
print("Successfully inserted 30 categories and 100 spare parts.")
