using Microsoft.EntityFrameworkCore;
using System;
using EHisse.Domain.Entities;
using EHisse.Domain.Enums;

namespace EHisse.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DeviceBrand> DeviceBrands { get; set; }
        public DbSet<CategoryBrand> CategoryBrands { get; set; }
        public DbSet<DeviceModel> DeviceModels { get; set; }
        public DbSet<SparePartType> SparePartTypes { get; set; }
        public DbSet<CategoryPartType> CategoryPartTypes { get; set; }
        public DbSet<QualityType> QualityTypes { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<SparePartImage> SparePartImages { get; set; }
        public DbSet<SparePartSaleAudit> SparePartSaleAudits { get; set; }
        public DbSet<SparePartReview> SparePartReviews { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<SellerReview> SellerReviews { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryBrand>()
                .HasKey(cb => new { cb.CategoryId, cb.BrandId });

            modelBuilder.Entity<CategoryPartType>()
                .HasKey(cp => new { cp.CategoryId, cp.PartTypeId });

            Seed(modelBuilder);
        }

        public static void Seed(ModelBuilder modelBuilder)
        {
            // ============================================
            // LOOKUPS - Admin paneldən idarə olunan
            // ============================================

            // 1. Categories (Cihaz kateqoriyaları)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Telefon", DisplayOrder = 1, IsActive = true },
                new Category { Id = 2, Name = "Planşet", DisplayOrder = 2, IsActive = true },
                new Category { Id = 3, Name = "Noutbuk", DisplayOrder = 3, IsActive = true }
            );

            // 2. Brands (Brendlər)
            modelBuilder.Entity<DeviceBrand>().HasData(
                new DeviceBrand { Id = 1, Name = "Apple", IsActive = true },
                new DeviceBrand { Id = 2, Name = "Samsung", IsActive = true },
                new DeviceBrand { Id = 3, Name = "Xiaomi", IsActive = true },
                new DeviceBrand { Id = 4, Name = "Huawei", IsActive = true },
                new DeviceBrand { Id = 5, Name = "Dell", IsActive = true },
                new DeviceBrand { Id = 6, Name = "Lenovo", IsActive = true },
                new DeviceBrand { Id = 7, Name = "Asus", IsActive = true }
            );

            // 3. Category-Brand Mapping (Hər kateqoriyada hansı brendlər var)
            modelBuilder.Entity<CategoryBrand>().HasData(
                // Telefon brendləri
                new { CategoryId = 1, BrandId = 1 }, // Apple
                new { CategoryId = 1, BrandId = 2 }, // Samsung
                new { CategoryId = 1, BrandId = 3 }, // Xiaomi
                new { CategoryId = 1, BrandId = 4 }, // Huawei

                // Planşet brendləri
                new { CategoryId = 2, BrandId = 1 }, // Apple iPad
                new { CategoryId = 2, BrandId = 2 }, // Samsung Tab
                new { CategoryId = 2, BrandId = 4 }, // Huawei

                // Noutbuk brendləri
                new { CategoryId = 3, BrandId = 1 }, // MacBook
                new { CategoryId = 3, BrandId = 5 }, // Dell
                new { CategoryId = 3, BrandId = 6 }, // Lenovo
                new { CategoryId = 3, BrandId = 7 }  // Asus
            );

            // 4. Device Models (Cihaz modelləri)
            modelBuilder.Entity<DeviceModel>().HasData(
                // Apple Telefonlar
                new DeviceModel { Id = 1, CategoryId = 1, BrandId = 1, Name = "iPhone 15 Pro Max", ModelCode = "A2849", ReleaseYear = 2023, IsActive = true },
                new DeviceModel { Id = 2, CategoryId = 1, BrandId = 1, Name = "iPhone 15 Pro", ModelCode = "A2848", ReleaseYear = 2023, IsActive = true },
                new DeviceModel { Id = 3, CategoryId = 1, BrandId = 1, Name = "iPhone 14 Pro Max", ModelCode = "A2651", ReleaseYear = 2022, IsActive = true },
                new DeviceModel { Id = 4, CategoryId = 1, BrandId = 1, Name = "iPhone 13", ModelCode = "A2482", ReleaseYear = 2021, IsActive = true },

                // Samsung Telefonlar
                new DeviceModel { Id = 5, CategoryId = 1, BrandId = 2, Name = "Galaxy S24 Ultra", ModelCode = "SM-S928", ReleaseYear = 2024, IsActive = true },
                new DeviceModel { Id = 6, CategoryId = 1, BrandId = 2, Name = "Galaxy S23", ModelCode = "SM-S911", ReleaseYear = 2023, IsActive = true },
                new DeviceModel { Id = 7, CategoryId = 1, BrandId = 2, Name = "Galaxy A54", ModelCode = "SM-A546", ReleaseYear = 2023, IsActive = true },

                // Xiaomi Telefonlar
                new DeviceModel { Id = 8, CategoryId = 1, BrandId = 3, Name = "Xiaomi 13 Pro", ModelCode = "2210132C", ReleaseYear = 2023, IsActive = true },
                new DeviceModel { Id = 9, CategoryId = 1, BrandId = 3, Name = "Redmi Note 12 Pro", ModelCode = "2211133C", ReleaseYear = 2023, IsActive = true },

                // Huawei Telefonlar
                new DeviceModel { Id = 10, CategoryId = 1, BrandId = 4, Name = "P60 Pro", ModelCode = "ALN-AL00", ReleaseYear = 2023, IsActive = true },

                // Apple Planşetlər
                new DeviceModel { Id = 11, CategoryId = 2, BrandId = 1, Name = "iPad Pro 12.9 (2024)", ModelCode = "A2764", ReleaseYear = 2024, IsActive = true },
                new DeviceModel { Id = 12, CategoryId = 2, BrandId = 1, Name = "iPad Air (2024)", ModelCode = "A2589", ReleaseYear = 2024, IsActive = true },

                // Samsung Planşetlər
                new DeviceModel { Id = 13, CategoryId = 2, BrandId = 2, Name = "Galaxy Tab S9 Ultra", ModelCode = "SM-X916", ReleaseYear = 2023, IsActive = true },

                // Apple Noutbuklar
                new DeviceModel { Id = 14, CategoryId = 3, BrandId = 1, Name = "MacBook Pro 14 (M3)", ModelCode = "MRX33", ReleaseYear = 2023, IsActive = true },
                new DeviceModel { Id = 15, CategoryId = 3, BrandId = 1, Name = "MacBook Air 13 (M2)", ModelCode = "MLXY3", ReleaseYear = 2022, IsActive = true },

                // Dell Noutbuklar
                new DeviceModel { Id = 16, CategoryId = 3, BrandId = 5, Name = "XPS 13 Plus", ModelCode = "9320", ReleaseYear = 2024, IsActive = true },
                new DeviceModel { Id = 17, CategoryId = 3, BrandId = 5, Name = "XPS 15", ModelCode = "9530", ReleaseYear = 2023, IsActive = true }
            );

            // 5. Part Types
            modelBuilder.Entity<SparePartType>().HasData(
                new SparePartType { Id = 1, Name = "Ekranlar", Key = "SCREEN", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 2, Name = "Batareyalar", Key = "BATTERY", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 3, Name = "Kameralar", Key = "CAM_REAR", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 4, Name = "Kamera şüşələri", Key = "CAM_FRONT", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 5, Name = "Motherboard", Key = "MB", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 6, Name = "Arxa qapaqlar", Key = "BACK_COVER", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 7, Name = "Dinamiklər, zənglər və vibromotorlar", Key = "SPEAKER", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 8, Name = "Charging Port", Key = "CHARGING_PORT", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 9, Name = "Sim tutucular", Key = "SIM_TRAY", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 10, Name = "Home düymələri və skanerlər", Key = "BUTTONS", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 11, Name = "Keyboard", Key = "KEYBOARD", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 12, Name = "Touchpad", Key = "TOUCHPAD", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 13, Name = "Ram", Key = "RAM", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 14, Name = "Storage", Key = "STORAGE", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 15, Name = "Adapter", Key = "ADAPTER", IsPortable = false, IsAccessory = true },
                new SparePartType { Id = 16, Name = "Sensor və ekran şüşələri", Key = "SENSOR_GLASS", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 17, Name = "Antenlər", Key = "ANTENNA", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 18, Name = "Dinamik setkaları", Key = "SPEAKER_NET", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 19, Name = "Korpus hissələri", Key = "BODY_PARTS", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 20, Name = "Korpuslar", Key = "BODY", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 21, Name = "Mikrofonlar", Key = "MIC", IsPortable = false, IsAccessory = false },
                new SparePartType { Id = 22, Name = "OCA filmlər, arxa işıqlar və polarizatorlar", Key = "OCA_POLARIZER", IsPortable = false, IsAccessory = false }
            );

            // 6. Category-PartType Mapping
            modelBuilder.Entity<CategoryPartType>().HasData(
                new CategoryPartType { CategoryId = 1, PartTypeId = 1 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 2 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 3 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 4 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 5 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 6 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 7 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 8 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 9 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 10 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 16 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 17 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 18 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 19 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 20 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 21 },
                new CategoryPartType { CategoryId = 1, PartTypeId = 22 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 1 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 2 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 3 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 4 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 5 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 6 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 8 },
                new CategoryPartType { CategoryId = 2, PartTypeId = 10 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 1 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 2 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 5 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 11 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 12 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 13 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 14 },
                new CategoryPartType { CategoryId = 3, PartTypeId = 15 }
            );

            // 7. Quality Types
            modelBuilder.Entity<QualityType>().HasData(
                new QualityType { Id = 1, Name = "Zavod istehsalı (yeni)", Code = "ORIGINAL_NEW", Description = "Rəsmi zavod istehsalı, heç işlənməyib", DisplayOrder = 1, IsActive = true },
                new QualityType { Id = 2, Name = "Zavod istehsalı (işlənmiş)", Code = "ORIGINAL_USED", Description = "Rəsmi zavod, başqa telefondan çıxarılıb", DisplayOrder = 2, IsActive = true },
                new QualityType { Id = 3, Name = "A-Class (Premium)", Code = "A_CLASS", Description = "Yüksək keyfiyyətli alternativ, zavoda yaxın", DisplayOrder = 3, IsActive = true },
                new QualityType { Id = 4, Name = "B-Class (Standart)", Code = "B_CLASS", Description = "Orta keyfiyyət, əlverişli qiymət", DisplayOrder = 4, IsActive = true },
                new QualityType { Id = 5, Name = "C-Class (Ekonom)", Code = "C_CLASS", Description = "Əsas funksiyalar, ən aşağı qiymət", DisplayOrder = 5, IsActive = true }
            );

            // 8. Users & Seller Profiles
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "elcin", Email = "elcin@example.com", PasswordHash = "123", Role = UserRole.Usta, FirstName = "Elçin", LastName = "Məmmədov", Phone = "+994501234567", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 2, Username = "rashad", Email = "rashad@example.com", PasswordHash = "123", Role = UserRole.Anbardar, FirstName = "Rəşad", LastName = "Əliyev", Phone = "+994551234568", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 3, Username = "nigar", Email = "nigar@example.com", PasswordHash = "123", Role = UserRole.Usta, FirstName = "Nigar", LastName = "Həsənova", Phone = "+994701234569", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 4, Username = "admin_user", Email = "admin@example.com", PasswordHash = "admin_panel2026#", Role = UserRole.Admin, FirstName = "Baş", LastName = "Administrator", Phone = "+994000000000", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 5, Username = "samir", Email = "samir@example.com", PasswordHash = "123", Role = UserRole.Usta, FirstName = "Samir", LastName = "Quliyev", Phone = "+994501231122", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 6, Username = "vuqar", Email = "vuqar@example.com", PasswordHash = "123", Role = UserRole.Anbardar, FirstName = "Vuqar", LastName = "Hesenov", Phone = "+994551233344", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 7, Username = "leyla", Email = "leyla@example.com", PasswordHash = "123", Role = UserRole.Usta, FirstName = "Leyla", LastName = "Memmedova", Phone = "+994701235566", ApprovalStatus = ApprovalStatus.Approved },
                new User { Id = 8, Username = "taleh", Email = "taleh@example.com", PasswordHash = "123", Role = UserRole.Anbardar, FirstName = "Taleh", LastName = "Qarayev", Phone = "+994511237788", ApprovalStatus = ApprovalStatus.Pending },
                new User { Id = 9, Username = "rustam", Email = "rustam@example.com", PasswordHash = "123", Role = UserRole.Usta, FirstName = "Rustam", LastName = "Aliyev", Phone = "+994501239900", ApprovalStatus = ApprovalStatus.Pending }
            );

            modelBuilder.Entity<SellerProfile>().HasData(
                new SellerProfile {
                    Id = 1,
                    UserId = 1,
                    CompanyName = "TechMaster Servis",
                    ContactPerson = "Elçin Məmmədov",
                    Phone = "+994501234567",
                    Address = "Bakı, Nəsimi rayonu, 28 May küç. 15",
                    Rating = 4.8m,
                    IsPhoneVerified = true
                },
                new SellerProfile {
                    Id = 2,
                    UserId = 2,
                    CompanyName = "Mobile Parts Store",
                    ContactPerson = "Rəşad Əliyev",
                    Phone = "+994551234568",
                    Address = "Bakı, Yasamal rayonu, Cəfər Cabbarlı küç. 44",
                    Rating = 4.5m,
                    IsPhoneVerified = true
                },
                new SellerProfile {
                    Id = 3,
                    UserId = 3,
                    CompanyName = "SmartFix Təmir Mərkəzi",
                    ContactPerson = "Nigar Həsənova",
                    Phone = "+994701234569",
                    Address = "Bakı, Nərimanov rayonu, Əhmədli küç. 9",
                    Rating = 4.9m,
                    IsPhoneVerified = true
                }
            );

            // 9. Spare Parts
                        modelBuilder.Entity<SparePart>().HasData(
                new SparePart { Id = 1, SKU = "SKU-1", SellerId = 1, DeviceModelId = 2, PartTypeId = 1, QualityTypeId = 3, Title = "iPhone 15 Pro Ekran - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 41.25m, Currency = "AZN", StockQuantity = 4, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 356, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 2, SKU = "SKU-2", SellerId = 1, DeviceModelId = 2, PartTypeId = 19, QualityTypeId = 1, Title = "iPhone 15 Pro Korpus Hissəsi - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 23.12m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 268, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 3, SKU = "SKU-3", SellerId = 2, DeviceModelId = 1, PartTypeId = 7, QualityTypeId = 5, Title = "iPhone 15 Pro Max Dinamik - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 40.86m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 152, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 4, SKU = "SKU-4", SellerId = 2, DeviceModelId = 3, PartTypeId = 19, QualityTypeId = 3, Title = "iPhone 14 Pro Max Korpus Hissəsi - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 31.77m, Currency = "AZN", StockQuantity = 11, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 62, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 5, SKU = "SKU-5", SellerId = 2, DeviceModelId = 7, PartTypeId = 4, QualityTypeId = 3, Title = "Galaxy A54 Ön Kamera - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 94.52m, Currency = "AZN", StockQuantity = 2, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 383, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 6, SKU = "SKU-6", SellerId = 1, DeviceModelId = 9, PartTypeId = 4, QualityTypeId = 4, Title = "Redmi Note 12 Pro Ön Kamera - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 87.29m, Currency = "AZN", StockQuantity = 20, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 463, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 7, SKU = "SKU-7", SellerId = 1, DeviceModelId = 10, PartTypeId = 7, QualityTypeId = 1, Title = "P60 Pro Dinamik - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 102.58m, Currency = "AZN", StockQuantity = 10, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 50, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 8, SKU = "SKU-8", SellerId = 2, DeviceModelId = 2, PartTypeId = 18, QualityTypeId = 3, Title = "iPhone 15 Pro Dinamik Setkası - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 99.0m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 93, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 9, SKU = "SKU-9", SellerId = 3, DeviceModelId = 6, PartTypeId = 7, QualityTypeId = 3, Title = "Galaxy S23 Dinamik - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 141.13m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 321, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 10, SKU = "SKU-10", SellerId = 2, DeviceModelId = 9, PartTypeId = 8, QualityTypeId = 2, Title = "Redmi Note 12 Pro Şarj Portu - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 63.12m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 122, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 11, SKU = "SKU-11", SellerId = 2, DeviceModelId = 1, PartTypeId = 8, QualityTypeId = 1, Title = "iPhone 15 Pro Max Şarj Portu - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 66.16m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 118, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 12, SKU = "SKU-12", SellerId = 2, DeviceModelId = 6, PartTypeId = 7, QualityTypeId = 4, Title = "Galaxy S23 Dinamik - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 133.86m, Currency = "AZN", StockQuantity = 15, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 83, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 13, SKU = "SKU-13", SellerId = 3, DeviceModelId = 3, PartTypeId = 8, QualityTypeId = 5, Title = "iPhone 14 Pro Max Şarj Portu - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 46.78m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 229, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 14, SKU = "SKU-14", SellerId = 1, DeviceModelId = 7, PartTypeId = 17, QualityTypeId = 2, Title = "Galaxy A54 Antenna - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 81.33m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 396, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 15, SKU = "SKU-15", SellerId = 3, DeviceModelId = 2, PartTypeId = 5, QualityTypeId = 2, Title = "iPhone 15 Pro Ana Plata - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 69.1m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 207, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 16, SKU = "SKU-16", SellerId = 2, DeviceModelId = 10, PartTypeId = 20, QualityTypeId = 5, Title = "P60 Pro Korpus - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 145.95m, Currency = "AZN", StockQuantity = 1, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 358, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 17, SKU = "SKU-17", SellerId = 1, DeviceModelId = 9, PartTypeId = 9, QualityTypeId = 3, Title = "Redmi Note 12 Pro Sim Tutucu - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 51.09m, Currency = "AZN", StockQuantity = 6, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 242, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 18, SKU = "SKU-18", SellerId = 3, DeviceModelId = 5, PartTypeId = 22, QualityTypeId = 2, Title = "Galaxy S24 Ultra OCA Film - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 137.77m, Currency = "AZN", StockQuantity = 10, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 440, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 19, SKU = "SKU-19", SellerId = 2, DeviceModelId = 10, PartTypeId = 7, QualityTypeId = 2, Title = "P60 Pro Dinamik - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 116.75m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 498, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 20, SKU = "SKU-20", SellerId = 1, DeviceModelId = 1, PartTypeId = 16, QualityTypeId = 4, Title = "iPhone 15 Pro Max Sensor Şüşə - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 25.66m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 459, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 21, SKU = "SKU-21", SellerId = 3, DeviceModelId = 4, PartTypeId = 2, QualityTypeId = 2, Title = "iPhone 13 Batareya - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 142.57m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 384, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 22, SKU = "SKU-22", SellerId = 3, DeviceModelId = 2, PartTypeId = 5, QualityTypeId = 2, Title = "iPhone 15 Pro Ana Plata - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 76.54m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 94, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 23, SKU = "SKU-23", SellerId = 3, DeviceModelId = 9, PartTypeId = 19, QualityTypeId = 2, Title = "Redmi Note 12 Pro Korpus Hissəsi - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 115.74m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 375, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 24, SKU = "SKU-24", SellerId = 3, DeviceModelId = 7, PartTypeId = 17, QualityTypeId = 4, Title = "Galaxy A54 Antenna - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 73.21m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 125, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 25, SKU = "SKU-25", SellerId = 3, DeviceModelId = 6, PartTypeId = 1, QualityTypeId = 5, Title = "Galaxy S23 Ekran - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 42.22m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 13, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 26, SKU = "SKU-26", SellerId = 1, DeviceModelId = 1, PartTypeId = 8, QualityTypeId = 1, Title = "iPhone 15 Pro Max Şarj Portu - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 130.35m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 273, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 27, SKU = "SKU-27", SellerId = 3, DeviceModelId = 5, PartTypeId = 21, QualityTypeId = 2, Title = "Galaxy S24 Ultra Mikrofon - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 28.52m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 305, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 28, SKU = "SKU-28", SellerId = 1, DeviceModelId = 4, PartTypeId = 21, QualityTypeId = 4, Title = "iPhone 13 Mikrofon - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 23.21m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 191, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 29, SKU = "SKU-29", SellerId = 3, DeviceModelId = 7, PartTypeId = 20, QualityTypeId = 1, Title = "Galaxy A54 Korpus - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 101.48m, Currency = "AZN", StockQuantity = 4, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 41, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 30, SKU = "SKU-30", SellerId = 1, DeviceModelId = 6, PartTypeId = 4, QualityTypeId = 2, Title = "Galaxy S23 Ön Kamera - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 36.63m, Currency = "AZN", StockQuantity = 15, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 81, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 31, SKU = "SKU-31", SellerId = 1, DeviceModelId = 3, PartTypeId = 9, QualityTypeId = 4, Title = "iPhone 14 Pro Max Sim Tutucu - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 132.43m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 236, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 32, SKU = "SKU-32", SellerId = 1, DeviceModelId = 2, PartTypeId = 2, QualityTypeId = 5, Title = "iPhone 15 Pro Batareya - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 145.66m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 95, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 33, SKU = "SKU-33", SellerId = 2, DeviceModelId = 8, PartTypeId = 21, QualityTypeId = 2, Title = "Xiaomi 13 Pro Mikrofon - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 136.34m, Currency = "AZN", StockQuantity = 6, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 204, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 34, SKU = "SKU-34", SellerId = 2, DeviceModelId = 7, PartTypeId = 9, QualityTypeId = 4, Title = "Galaxy A54 Sim Tutucu - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 69.22m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 348, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 35, SKU = "SKU-35", SellerId = 1, DeviceModelId = 3, PartTypeId = 7, QualityTypeId = 3, Title = "iPhone 14 Pro Max Dinamik - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 145.62m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 386, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 36, SKU = "SKU-36", SellerId = 1, DeviceModelId = 11, PartTypeId = 6, QualityTypeId = 1, Title = "iPad Pro 12.9 Arxa Qapaq - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 91.78m, Currency = "AZN", StockQuantity = 17, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 480, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 37, SKU = "SKU-37", SellerId = 1, DeviceModelId = 11, PartTypeId = 1, QualityTypeId = 5, Title = "iPad Pro 12.9 Ekran - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 129.19m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 314, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 38, SKU = "SKU-38", SellerId = 1, DeviceModelId = 13, PartTypeId = 4, QualityTypeId = 4, Title = "Galaxy Tab S9 Ultra Ön Kamera - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 141.82m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 136, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 39, SKU = "SKU-39", SellerId = 1, DeviceModelId = 13, PartTypeId = 1, QualityTypeId = 5, Title = "Galaxy Tab S9 Ultra Ekran - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 68.69m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 299, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 40, SKU = "SKU-40", SellerId = 3, DeviceModelId = 12, PartTypeId = 5, QualityTypeId = 2, Title = "iPad Air Ana Plata - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 110.27m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 145, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 41, SKU = "SKU-41", SellerId = 2, DeviceModelId = 11, PartTypeId = 5, QualityTypeId = 4, Title = "iPad Pro 12.9 Ana Plata - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 140.06m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 14, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 42, SKU = "SKU-42", SellerId = 3, DeviceModelId = 13, PartTypeId = 2, QualityTypeId = 1, Title = "Galaxy Tab S9 Ultra Batareya - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 39.84m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 77, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 43, SKU = "SKU-43", SellerId = 2, DeviceModelId = 11, PartTypeId = 4, QualityTypeId = 3, Title = "iPad Pro 12.9 Ön Kamera - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 32.08m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 370, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 44, SKU = "SKU-44", SellerId = 2, DeviceModelId = 13, PartTypeId = 1, QualityTypeId = 5, Title = "Galaxy Tab S9 Ultra Ekran - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 140.45m, Currency = "AZN", StockQuantity = 4, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 490, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 45, SKU = "SKU-45", SellerId = 3, DeviceModelId = 12, PartTypeId = 2, QualityTypeId = 1, Title = "iPad Air Batareya - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 87.45m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 154, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 46, SKU = "SKU-46", SellerId = 3, DeviceModelId = 11, PartTypeId = 6, QualityTypeId = 2, Title = "iPad Pro 12.9 Arxa Qapaq - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 98.79m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 268, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 47, SKU = "SKU-47", SellerId = 3, DeviceModelId = 12, PartTypeId = 1, QualityTypeId = 1, Title = "iPad Air Ekran - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 69.3m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 32, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 48, SKU = "SKU-48", SellerId = 1, DeviceModelId = 12, PartTypeId = 3, QualityTypeId = 3, Title = "iPad Air Arxa Kamera - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 113.77m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 371, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 49, SKU = "SKU-49", SellerId = 1, DeviceModelId = 13, PartTypeId = 1, QualityTypeId = 1, Title = "Galaxy Tab S9 Ultra Ekran - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 142.31m, Currency = "AZN", StockQuantity = 5, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 289, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 50, SKU = "SKU-50", SellerId = 1, DeviceModelId = 12, PartTypeId = 3, QualityTypeId = 4, Title = "iPad Air Arxa Kamera - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 15.86m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 470, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 51, SKU = "SKU-51", SellerId = 3, DeviceModelId = 12, PartTypeId = 4, QualityTypeId = 2, Title = "iPad Air Ön Kamera - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 24.39m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 462, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 52, SKU = "SKU-52", SellerId = 1, DeviceModelId = 13, PartTypeId = 3, QualityTypeId = 2, Title = "Galaxy Tab S9 Ultra Arxa Kamera - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 146.67m, Currency = "AZN", StockQuantity = 6, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 461, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 53, SKU = "SKU-53", SellerId = 2, DeviceModelId = 11, PartTypeId = 3, QualityTypeId = 3, Title = "iPad Pro 12.9 Arxa Kamera - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 122.31m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 146, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 54, SKU = "SKU-54", SellerId = 1, DeviceModelId = 13, PartTypeId = 2, QualityTypeId = 4, Title = "Galaxy Tab S9 Ultra Batareya - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 130.2m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 112, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 55, SKU = "SKU-55", SellerId = 1, DeviceModelId = 12, PartTypeId = 5, QualityTypeId = 2, Title = "iPad Air Ana Plata - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 13.31m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 214, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 56, SKU = "SKU-56", SellerId = 2, DeviceModelId = 12, PartTypeId = 2, QualityTypeId = 3, Title = "iPad Air Batareya - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 99.81m, Currency = "AZN", StockQuantity = 13, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 357, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 57, SKU = "SKU-57", SellerId = 2, DeviceModelId = 12, PartTypeId = 1, QualityTypeId = 1, Title = "iPad Air Ekran - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 35.0m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 29, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 58, SKU = "SKU-58", SellerId = 3, DeviceModelId = 13, PartTypeId = 8, QualityTypeId = 3, Title = "Galaxy Tab S9 Ultra Şarj Portu - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 120.1m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 320, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 59, SKU = "SKU-59", SellerId = 1, DeviceModelId = 11, PartTypeId = 8, QualityTypeId = 5, Title = "iPad Pro 12.9 Şarj Portu - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 45.66m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 10, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 60, SKU = "SKU-60", SellerId = 2, DeviceModelId = 13, PartTypeId = 4, QualityTypeId = 3, Title = "Galaxy Tab S9 Ultra Ön Kamera - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 19.8m, Currency = "AZN", StockQuantity = 11, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 329, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 61, SKU = "SKU-61", SellerId = 3, DeviceModelId = 13, PartTypeId = 2, QualityTypeId = 3, Title = "Galaxy Tab S9 Ultra Batareya - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 53.3m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 177, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 62, SKU = "SKU-62", SellerId = 1, DeviceModelId = 13, PartTypeId = 5, QualityTypeId = 5, Title = "Galaxy Tab S9 Ultra Ana Plata - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 36.86m, Currency = "AZN", StockQuantity = 13, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 356, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 63, SKU = "SKU-63", SellerId = 3, DeviceModelId = 13, PartTypeId = 5, QualityTypeId = 4, Title = "Galaxy Tab S9 Ultra Ana Plata - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 126.72m, Currency = "AZN", StockQuantity = 10, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 156, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 64, SKU = "SKU-64", SellerId = 2, DeviceModelId = 12, PartTypeId = 6, QualityTypeId = 4, Title = "iPad Air Arxa Qapaq - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 71.9m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 271, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 65, SKU = "SKU-65", SellerId = 2, DeviceModelId = 13, PartTypeId = 3, QualityTypeId = 1, Title = "Galaxy Tab S9 Ultra Arxa Kamera - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 82.16m, Currency = "AZN", StockQuantity = 20, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 181, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 66, SKU = "SKU-66", SellerId = 1, DeviceModelId = 11, PartTypeId = 5, QualityTypeId = 2, Title = "iPad Pro 12.9 Ana Plata - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 30.63m, Currency = "AZN", StockQuantity = 2, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 135, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 67, SKU = "SKU-67", SellerId = 2, DeviceModelId = 13, PartTypeId = 2, QualityTypeId = 4, Title = "Galaxy Tab S9 Ultra Batareya - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 134.06m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 109, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 68, SKU = "SKU-68", SellerId = 1, DeviceModelId = 12, PartTypeId = 8, QualityTypeId = 2, Title = "iPad Air Şarj Portu - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 101.85m, Currency = "AZN", StockQuantity = 1, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 467, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 69, SKU = "SKU-69", SellerId = 3, DeviceModelId = 12, PartTypeId = 4, QualityTypeId = 2, Title = "iPad Air Ön Kamera - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 82.51m, Currency = "AZN", StockQuantity = 2, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 295, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 70, SKU = "SKU-70", SellerId = 2, DeviceModelId = 11, PartTypeId = 10, QualityTypeId = 2, Title = "iPad Pro 12.9 Home Düyməsi - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 103.46m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 314, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 71, SKU = "SKU-71", SellerId = 2, DeviceModelId = 17, PartTypeId = 14, QualityTypeId = 5, Title = "XPS 15 Yaddaş - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 135.61m, Currency = "AZN", StockQuantity = 16, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 240, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 72, SKU = "SKU-72", SellerId = 2, DeviceModelId = 15, PartTypeId = 12, QualityTypeId = 5, Title = "MacBook Air 13 Touchpad - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 97.74m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 235, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 73, SKU = "SKU-73", SellerId = 2, DeviceModelId = 16, PartTypeId = 11, QualityTypeId = 3, Title = "XPS 13 Plus Klaviatura - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 54.76m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 51, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 74, SKU = "SKU-74", SellerId = 3, DeviceModelId = 15, PartTypeId = 11, QualityTypeId = 4, Title = "MacBook Air 13 Klaviatura - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 31.39m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 42, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 75, SKU = "SKU-75", SellerId = 2, DeviceModelId = 17, PartTypeId = 13, QualityTypeId = 5, Title = "XPS 15 RAM - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 68.21m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 436, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 76, SKU = "SKU-76", SellerId = 2, DeviceModelId = 17, PartTypeId = 1, QualityTypeId = 5, Title = "XPS 15 Ekran - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 76.78m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 162, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 77, SKU = "SKU-77", SellerId = 1, DeviceModelId = 17, PartTypeId = 11, QualityTypeId = 4, Title = "XPS 15 Klaviatura - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 48.21m, Currency = "AZN", StockQuantity = 16, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 24, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 78, SKU = "SKU-78", SellerId = 2, DeviceModelId = 16, PartTypeId = 14, QualityTypeId = 2, Title = "XPS 13 Plus Yaddaş - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 138.74m, Currency = "AZN", StockQuantity = 20, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 283, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 79, SKU = "SKU-79", SellerId = 3, DeviceModelId = 17, PartTypeId = 1, QualityTypeId = 1, Title = "XPS 15 Ekran - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 70.01m, Currency = "AZN", StockQuantity = 15, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 103, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 80, SKU = "SKU-80", SellerId = 1, DeviceModelId = 16, PartTypeId = 14, QualityTypeId = 3, Title = "XPS 13 Plus Yaddaş - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 73.66m, Currency = "AZN", StockQuantity = 11, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 399, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 81, SKU = "SKU-81", SellerId = 1, DeviceModelId = 16, PartTypeId = 14, QualityTypeId = 3, Title = "XPS 13 Plus Yaddaş - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 75.84m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 36, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 82, SKU = "SKU-82", SellerId = 1, DeviceModelId = 15, PartTypeId = 2, QualityTypeId = 1, Title = "MacBook Air 13 Batareya - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 142.95m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 0, ViewCount = 439, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 83, SKU = "SKU-83", SellerId = 2, DeviceModelId = 15, PartTypeId = 11, QualityTypeId = 2, Title = "MacBook Air 13 Klaviatura - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 103.72m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 495, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 84, SKU = "SKU-84", SellerId = 1, DeviceModelId = 17, PartTypeId = 12, QualityTypeId = 3, Title = "XPS 15 Touchpad - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 94.82m, Currency = "AZN", StockQuantity = 4, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 408, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 85, SKU = "SKU-85", SellerId = 1, DeviceModelId = 16, PartTypeId = 2, QualityTypeId = 5, Title = "XPS 13 Plus Batareya - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 140.04m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 356, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 86, SKU = "SKU-86", SellerId = 3, DeviceModelId = 17, PartTypeId = 11, QualityTypeId = 1, Title = "XPS 15 Klaviatura - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 106.69m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 62, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 87, SKU = "SKU-87", SellerId = 3, DeviceModelId = 14, PartTypeId = 1, QualityTypeId = 3, Title = "MacBook Pro 14 Ekran - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 69.97m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 45, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 88, SKU = "SKU-88", SellerId = 2, DeviceModelId = 16, PartTypeId = 1, QualityTypeId = 4, Title = "XPS 13 Plus Ekran - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 24.78m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 335, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 89, SKU = "SKU-89", SellerId = 3, DeviceModelId = 15, PartTypeId = 14, QualityTypeId = 2, Title = "MacBook Air 13 Yaddaş - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 83.04m, Currency = "AZN", StockQuantity = 9, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 325, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 90, SKU = "SKU-90", SellerId = 3, DeviceModelId = 17, PartTypeId = 15, QualityTypeId = 4, Title = "XPS 15 Adapter - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 92.95m, Currency = "AZN", StockQuantity = 11, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 446, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 91, SKU = "SKU-91", SellerId = 1, DeviceModelId = 14, PartTypeId = 12, QualityTypeId = 4, Title = "MacBook Pro 14 Touchpad - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 115.07m, Currency = "AZN", StockQuantity = 19, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 322, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 92, SKU = "SKU-92", SellerId = 2, DeviceModelId = 16, PartTypeId = 1, QualityTypeId = 4, Title = "XPS 13 Plus Ekran - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 35.46m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 191, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 93, SKU = "SKU-93", SellerId = 3, DeviceModelId = 16, PartTypeId = 12, QualityTypeId = 5, Title = "XPS 13 Plus Touchpad - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 133.22m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 15, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 94, SKU = "SKU-94", SellerId = 3, DeviceModelId = 15, PartTypeId = 2, QualityTypeId = 2, Title = "MacBook Air 13 Batareya - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 66.9m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 398, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 95, SKU = "SKU-95", SellerId = 1, DeviceModelId = 17, PartTypeId = 15, QualityTypeId = 4, Title = "XPS 15 Adapter - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 23.03m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 217, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 96, SKU = "SKU-96", SellerId = 3, DeviceModelId = 16, PartTypeId = 13, QualityTypeId = 4, Title = "XPS 13 Plus RAM - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 84.33m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 391, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 97, SKU = "SKU-97", SellerId = 2, DeviceModelId = 16, PartTypeId = 13, QualityTypeId = 4, Title = "XPS 13 Plus RAM - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 52.93m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 71, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 98, SKU = "SKU-98", SellerId = 3, DeviceModelId = 16, PartTypeId = 2, QualityTypeId = 5, Title = "XPS 13 Plus Batareya - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 35.92m, Currency = "AZN", StockQuantity = 7, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 388, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 99, SKU = "SKU-99", SellerId = 1, DeviceModelId = 16, PartTypeId = 12, QualityTypeId = 1, Title = "XPS 13 Plus Touchpad - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 51.47m, Currency = "AZN", StockQuantity = 12, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 101, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 100, SKU = "SKU-100", SellerId = 1, DeviceModelId = 14, PartTypeId = 5, QualityTypeId = 3, Title = "MacBook Pro 14 Ana Plata - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 146.4m, Currency = "AZN", StockQuantity = 18, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 159, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 101, SKU = "SKU-101", SellerId = 3, DeviceModelId = 17, PartTypeId = 2, QualityTypeId = 1, Title = "XPS 15 Batareya - Orijinal Zavod", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 49.81m, Currency = "AZN", StockQuantity = 16, MinOrderQuantity = 1, WarrantyMonths = 3, ViewCount = 235, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 102, SKU = "SKU-102", SellerId = 2, DeviceModelId = 15, PartTypeId = 1, QualityTypeId = 3, Title = "MacBook Air 13 Ekran - A-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 25.97m, Currency = "AZN", StockQuantity = 3, MinOrderQuantity = 1, WarrantyMonths = 6, ViewCount = 215, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 103, SKU = "SKU-103", SellerId = 1, DeviceModelId = 14, PartTypeId = 1, QualityTypeId = 2, Title = "MacBook Pro 14 Ekran - İşlənmiş Orijinal", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 123.56m, Currency = "AZN", StockQuantity = 10, MinOrderQuantity = 1, WarrantyMonths = 1, ViewCount = 53, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 104, SKU = "SKU-104", SellerId = 3, DeviceModelId = 14, PartTypeId = 14, QualityTypeId = 5, Title = "MacBook Pro 14 Yaddaş - C-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 120.7m, Currency = "AZN", StockQuantity = 8, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 407, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] },
                new SparePart { Id = 105, SKU = "SKU-105", SellerId = 2, DeviceModelId = 17, PartTypeId = 15, QualityTypeId = 4, Title = "XPS 15 Adapter - B-Class", Description = "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", Price = 130.42m, Currency = "AZN", StockQuantity = 14, MinOrderQuantity = 1, WarrantyMonths = 12, ViewCount = 166, IsActive = true, ModerationStatus = ApprovalStatus.Approved, CreatedDate = new DateTime(2024, 1, 1), LastModifiedDate = new DateTime(2024, 1, 1), RowVersion = new byte[0] }
            );

            // ============================================
            // SPARE PART IMAGES
            // ============================================
            modelBuilder.Entity<SparePartImage>().HasData(
                new SparePartImage { Id = 1, SparePartId = 1, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 2, SparePartId = 2, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 3, SparePartId = 3, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 4, SparePartId = 4, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 5, SparePartId = 5, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 6, SparePartId = 6, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 7, SparePartId = 7, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 8, SparePartId = 8, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 9, SparePartId = 9, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 10, SparePartId = 10, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 11, SparePartId = 11, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 12, SparePartId = 12, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 13, SparePartId = 13, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 14, SparePartId = 14, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 15, SparePartId = 15, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 16, SparePartId = 16, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 17, SparePartId = 17, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 18, SparePartId = 18, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 19, SparePartId = 19, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 20, SparePartId = 20, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 21, SparePartId = 21, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 22, SparePartId = 22, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 23, SparePartId = 23, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 24, SparePartId = 24, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 25, SparePartId = 25, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 26, SparePartId = 26, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 27, SparePartId = 27, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 28, SparePartId = 28, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 29, SparePartId = 29, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 30, SparePartId = 30, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 31, SparePartId = 31, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 32, SparePartId = 32, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 33, SparePartId = 33, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 34, SparePartId = 34, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 35, SparePartId = 35, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 36, SparePartId = 36, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 37, SparePartId = 37, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 38, SparePartId = 38, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 39, SparePartId = 39, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 40, SparePartId = 40, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 41, SparePartId = 41, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 42, SparePartId = 42, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 43, SparePartId = 43, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 44, SparePartId = 44, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 45, SparePartId = 45, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 46, SparePartId = 46, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 47, SparePartId = 47, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 48, SparePartId = 48, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 49, SparePartId = 49, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 50, SparePartId = 50, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 51, SparePartId = 51, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 52, SparePartId = 52, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 53, SparePartId = 53, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 54, SparePartId = 54, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 55, SparePartId = 55, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 56, SparePartId = 56, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 57, SparePartId = 57, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 58, SparePartId = 58, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 59, SparePartId = 59, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 60, SparePartId = 60, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 61, SparePartId = 61, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 62, SparePartId = 62, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 63, SparePartId = 63, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 64, SparePartId = 64, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 65, SparePartId = 65, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 66, SparePartId = 66, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 67, SparePartId = 67, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 68, SparePartId = 68, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 69, SparePartId = 69, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 70, SparePartId = 70, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 71, SparePartId = 71, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 72, SparePartId = 72, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 73, SparePartId = 73, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 74, SparePartId = 74, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 75, SparePartId = 75, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 76, SparePartId = 76, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 77, SparePartId = 77, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 78, SparePartId = 78, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 79, SparePartId = 79, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 80, SparePartId = 80, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 81, SparePartId = 81, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 82, SparePartId = 82, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 83, SparePartId = 83, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 84, SparePartId = 84, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 85, SparePartId = 85, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 86, SparePartId = 86, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 87, SparePartId = 87, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 88, SparePartId = 88, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 89, SparePartId = 89, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 90, SparePartId = 90, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 91, SparePartId = 91, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 92, SparePartId = 92, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 93, SparePartId = 93, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 94, SparePartId = 94, ImageUrl = "/images/images (4).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 95, SparePartId = 95, ImageUrl = "/images/images (5).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 96, SparePartId = 96, ImageUrl = "/images/images (6).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 97, SparePartId = 97, ImageUrl = "/images/images (7).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 98, SparePartId = 98, ImageUrl = "/images/images (1).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 99, SparePartId = 99, ImageUrl = "/images/images (2).jpg", IsMain = true, DisplayOrder = 1 },
new SparePartImage { Id = 100, SparePartId = 100, ImageUrl = "/images/images (3).jpg", IsMain = true, DisplayOrder = 1 }
            );
        }
    }
}



