import sqlite3
import random
import uuid
from datetime import datetime

conn = sqlite3.connect('ehisseler.db')
cursor = conn.cursor()

# Get valid SellerIds
cursor.execute("SELECT Id FROM SellerProfiles")
sellers = [r[0] for r in cursor.fetchall()]
if not sellers:
    sellers = [1]

# Get valid QualityTypeIds
cursor.execute("SELECT Id FROM QualityTypes")
qualities = [r[0] for r in cursor.fetchall()]
if not qualities:
    qualities = [1]

# Get Categories
cursor.execute("SELECT Id FROM Categories")
categories = [r[0] for r in cursor.fetchall()]

adjectives = ["Orijinal", "Yüksək keyfiyyətli", "Yeni", "İşlənmiş", "Ucuz", "Premium", "Zəmanətli", "Test edilmiş", "A klass", "B klass"]
items = ["Ekran", "Batareya", "Ana plata", "Şarj yuvası", "Kamera", "Dinamik", "Mikrofon", "Korpus", "Şleyf", "Düymə"]

total_inserted = 0
for cat_id in categories:
    num_products = random.randint(30, 35)
    
    # Get DeviceModels for this category
    cursor.execute("SELECT Id FROM DeviceModels WHERE CategoryId = ?", (cat_id,))
    models = [r[0] for r in cursor.fetchall()]
    
    # Get PartTypes for this category
    cursor.execute("SELECT PartTypeId FROM CategoryPartTypes WHERE CategoryId = ?", (cat_id,))
    part_types = [r[0] for r in cursor.fetchall()]
    
    if not models or not part_types:
        # If a category doesn't have a model or part type, we might not be able to insert valid rows,
        # but let's try to get ANY model or ANY part type if that happens (though it violates schema logically)
        if not models:
            cursor.execute("SELECT Id FROM DeviceModels LIMIT 1")
            m = cursor.fetchone()
            if m: models = [m[0]]
            else: models = [1]
        if not part_types:
            cursor.execute("SELECT Id FROM SparePartTypes LIMIT 1")
            p = cursor.fetchone()
            if p: part_types = [p[0]]
            else: part_types = [1]
            
    parts_data = []
    for i in range(num_products):
        seller_id = random.choice(sellers)
        model_id = random.choice(models)
        part_type_id = random.choice(part_types)
        quality_id = random.choice(qualities)
        
        title = f"{random.choice(adjectives)} {random.choice(items)} - Kat: {cat_id} #{i+1}"
        desc = f"Bu, {title} üçün ətraflı məlumatdır. Çox keyfiyyətli məhsuldur."
        # Generate short unique SKU
        sku = f"SKU-{cat_id}-{random.randint(1000, 9999)}-{uuid.uuid4().hex[:4].upper()}"
        price = str(round(random.uniform(10.0, 300.0), 2))
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
    
    total_inserted += num_products

conn.commit()
conn.close()
print(f"Successfully inserted {total_inserted} spare parts across {len(categories)} categories.")
