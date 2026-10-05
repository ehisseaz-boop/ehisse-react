using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EHisseler.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", nullable: true),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TargetUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryBrands",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryBrands", x => new { x.CategoryId, x.BrandId });
                });

            migrationBuilder.CreateTable(
                name: "CategoryPartTypes",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPartTypes", x => new { x.CategoryId, x.PartTypeId });
                });

            migrationBuilder.CreateTable(
                name: "DeviceBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QualityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SparePartTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    IsPortable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAccessory = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ModelCode = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceModels_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceModels_DeviceBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "DeviceBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    WhatsAppNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    ShowContactInfoInAds = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SellerId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    QualityTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MinOrderQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    WarrantyMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ModerationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectionReason = table.Column<string>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpareParts_DeviceModels_DeviceModelId",
                        column: x => x.DeviceModelId,
                        principalTable: "DeviceModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpareParts_QualityTypes_QualityTypeId",
                        column: x => x.QualityTypeId,
                        principalTable: "QualityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpareParts_SellerProfiles_SellerId",
                        column: x => x.SellerId,
                        principalTable: "SellerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpareParts_SparePartTypes_PartTypeId",
                        column: x => x.PartTypeId,
                        principalTable: "SparePartTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SparePartId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartImages_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SparePartId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartReviews_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartSaleAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SparePartId = table.Column<int>(type: "INTEGER", nullable: false),
                    SellerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousStock = table.Column<int>(type: "INTEGER", nullable: false),
                    NewStock = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartSaleAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartSaleAudits_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, "Telefon" },
                    { 2, 2, true, "Planşet" },
                    { 3, 3, true, "Noutbuk" }
                });

            migrationBuilder.InsertData(
                table: "CategoryBrands",
                columns: new[] { "BrandId", "CategoryId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 4, 2 },
                    { 1, 3 },
                    { 5, 3 },
                    { 6, 3 },
                    { 7, 3 }
                });

            migrationBuilder.InsertData(
                table: "CategoryPartTypes",
                columns: new[] { "CategoryId", "PartTypeId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 8 },
                    { 1, 9 },
                    { 1, 10 },
                    { 1, 16 },
                    { 1, 17 },
                    { 1, 18 },
                    { 1, 19 },
                    { 1, 20 },
                    { 1, 21 },
                    { 1, 22 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 8 },
                    { 2, 10 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 5 },
                    { 3, 11 },
                    { 3, 12 },
                    { 3, 13 },
                    { 3, 14 },
                    { 3, 15 }
                });

            migrationBuilder.InsertData(
                table: "DeviceBrands",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Apple" },
                    { 2, true, "Samsung" },
                    { 3, true, "Xiaomi" },
                    { 4, true, "Huawei" },
                    { 5, true, "Dell" },
                    { 6, true, "Lenovo" },
                    { 7, true, "Asus" }
                });

            migrationBuilder.InsertData(
                table: "QualityTypes",
                columns: new[] { "Id", "Code", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "ORIGINAL_NEW", "Rəsmi zavod istehsalı, heç işlənməyib", 1, true, "Zavod istehsalı (yeni)" },
                    { 2, "ORIGINAL_USED", "Rəsmi zavod, başqa telefondan çıxarılıb", 2, true, "Zavod istehsalı (işlənmiş)" },
                    { 3, "A_CLASS", "Yüksək keyfiyyətli alternativ, zavoda yaxın", 3, true, "A-Class (Premium)" },
                    { 4, "B_CLASS", "Orta keyfiyyət, əlverişli qiymət", 4, true, "B-Class (Standart)" },
                    { 5, "C_CLASS", "Æsas funksiyalar, ən aşağı qiymət", 5, true, "C-Class (Ekonom)" }
                });

            migrationBuilder.InsertData(
                table: "SparePartTypes",
                columns: new[] { "Id", "IsAccessory", "IsPortable", "Key", "Name" },
                values: new object[,]
                {
                    { 1, false, false, "SCREEN", "Telefon üçün ekranlar" },
                    { 2, false, false, "BATTERY", "Telefon üçün batareyalar" },
                    { 3, false, false, "CAM_REAR", "Kameralar" },
                    { 4, false, false, "CAM_FRONT", "Kamera şüşələri" },
                    { 5, false, false, "MB", "Motherboard" },
                    { 6, false, false, "BACK_COVER", "Arxa qapaqlar" },
                    { 7, false, false, "SPEAKER", "Dinamiklər, zənglər və vibromotorlar" },
                    { 8, false, false, "CHARGING_PORT", "Charging Port" },
                    { 9, false, false, "SIM_TRAY", "Sim tutucular" },
                    { 10, false, false, "BUTTONS", "Home düymələri və skanerlər" },
                    { 11, false, false, "KEYBOARD", "Keyboard" },
                    { 12, false, false, "TOUCHPAD", "Touchpad" },
                    { 13, false, false, "RAM", "Ram" },
                    { 14, false, false, "STORAGE", "Storage" },
                    { 15, true, false, "ADAPTER", "Adapter" },
                    { 16, false, false, "SENSOR_GLASS", "Telefon üçün sensorlar və ekran şüşələri" },
                    { 17, false, false, "ANTENNA", "Antenlər" },
                    { 18, false, false, "SPEAKER_NET", "Dinamik setkaları" },
                    { 19, false, false, "BODY_PARTS", "Korpus hissələri" },
                    { 20, false, false, "BODY", "Korpuslar" },
                    { 21, false, false, "MIC", "Mikrofonlar" },
                    { 22, false, false, "OCA_POLARIZER", "OCA filmlər, arxa işıqlar və polarizatorlar" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ApprovalStatus", "CreatedDate", "Email", "FirstName", "IsBlocked", "LastName", "PasswordHash", "Phone", "Role", "Username" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7169), "elcin@example.com", "Elçin", false, "Məmmədov", "123", "+994501234567", 2, "elcin" },
                    { 2, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7186), "rashad@example.com", "Rəşad", false, "Æ liyev", "123", "+994551234568", 3, "rashad" },
                    { 3, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7190), "nigar@example.com", "Nigar", false, "Həsənova", "123", "+994701234569", 2, "nigar" },
                    { 4, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7193), "admin@example.com", "Baş", false, "Administrator", "admin", "+994000000000", 1, "admin" },
                    { 5, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7195), "samir@example.com", "Samir", false, "Quliyev", "123", "+994501231122", 2, "samir" },
                    { 6, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7197), "vuqar@example.com", "Vuqar", false, "Hesenov", "123", "+994551233344", 3, "vuqar" },
                    { 7, 2, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7200), "leyla@example.com", "Leyla", false, "Memmedova", "123", "+994701235566", 2, "leyla" },
                    { 8, 1, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7202), "taleh@example.com", "Taleh", false, "Qarayev", "123", "+994511237788", 3, "taleh" },
                    { 9, 1, new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7205), "rustam@example.com", "Rustam", false, "Aliyev", "123", "+994501239900", 2, "rustam" }
                });

            migrationBuilder.InsertData(
                table: "DeviceModels",
                columns: new[] { "Id", "BrandId", "CategoryId", "IsActive", "ModelCode", "Name", "ReleaseYear" },
                values: new object[,]
                {
                    { 1, 1, 1, true, "A2849", "iPhone 15 Pro Max", 2023 },
                    { 2, 1, 1, true, "A2848", "iPhone 15 Pro", 2023 },
                    { 3, 1, 1, true, "A2651", "iPhone 14 Pro Max", 2022 },
                    { 4, 1, 1, true, "A2482", "iPhone 13", 2021 },
                    { 5, 2, 1, true, "SM-S928", "Galaxy S24 Ultra", 2024 },
                    { 6, 2, 1, true, "SM-S911", "Galaxy S23", 2023 },
                    { 7, 2, 1, true, "SM-A546", "Galaxy A54", 2023 },
                    { 8, 3, 1, true, "2210132C", "Xiaomi 13 Pro", 2023 },
                    { 9, 3, 1, true, "2211133C", "Redmi Note 12 Pro", 2023 },
                    { 10, 4, 1, true, "ALN-AL00", "P60 Pro", 2023 },
                    { 11, 1, 2, true, "A2764", "iPad Pro 12.9 (2024)", 2024 },
                    { 12, 1, 2, true, "A2589", "iPad Air (2024)", 2024 },
                    { 13, 2, 2, true, "SM-X916", "Galaxy Tab S9 Ultra", 2023 },
                    { 14, 1, 3, true, "MRX33", "MacBook Pro 14 (M3)", 2023 },
                    { 15, 1, 3, true, "MLXY3", "MacBook Air 13 (M2)", 2022 },
                    { 16, 5, 3, true, "9320", "XPS 13 Plus", 2024 },
                    { 17, 5, 3, true, "9530", "XPS 15", 2023 }
                });

            migrationBuilder.InsertData(
                table: "SellerProfiles",
                columns: new[] { "Id", "Address", "CompanyName", "ContactPerson", "IsPhoneVerified", "Phone", "Rating", "ShowContactInfoInAds", "UserId", "WhatsAppNumber" },
                values: new object[,]
                {
                    { 1, "Bakı, Nəsimi rayonu, 28 May küç. 15", "TechMaster Servis", "Elçin Məmmədov", true, "+994501234567", 4.8m, true, 1, null },
                    { 2, "Bakı, Yasamal rayonu, Cəfər Cabbarlı küç. 44", "Mobile Parts Store", "Rəşad Æliyev", true, "+994551234568", 4.5m, true, 2, null },
                    { 3, "Bakı, Nərimanov rayonu, Æhmədli küç. 9", "SmartFix Təmir Mərkəzi", "Nigar Həsənova", true, "+994701234569", 4.9m, true, 3, null }
                });

            migrationBuilder.InsertData(
                table: "SpareParts",
                columns: new[] { "Id", "CreatedDate", "Currency", "Description", "DeviceModelId", "DiscountPrice", "IsActive", "IsDeleted", "LastModifiedDate", "MinOrderQuantity", "ModerationStatus", "PartTypeId", "Price", "QualityTypeId", "RejectionReason", "RowVersion", "SKU", "SellerId", "StockQuantity", "Title", "ViewCount", "WarrantyMonths" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 13.00m, 2, null, new byte[0], "SKU-1", 2, 2, "Apple iPhone 15 Pro Batareya - A-Class", 13, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 16.00m, 3, null, new byte[0], "SKU-2", 3, 3, "Apple iPhone 14 Pro Max Arxa Kamera - B-Class", 26, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 19.00m, 4, null, new byte[0], "SKU-3", 1, 4, "Apple iPhone 13 Ana Plata - İşlənmiş Orijinal", 39, 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 22.00m, 1, null, new byte[0], "SKU-4", 2, 5, "Samsung Galaxy S24 Ultra Klaviatura - Orijinal Zavod", 52, 4 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 25.00m, 2, null, new byte[0], "SKU-5", 3, 6, "Samsung Galaxy S23 Ekran - A-Class", 65, 5 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 28.00m, 3, null, new byte[0], "SKU-6", 1, 7, "Samsung Galaxy A54 Batareya - B-Class", 78, 6 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 31.00m, 4, null, new byte[0], "SKU-7", 2, 8, "Xiaomi Xiaomi 13 Pro Arxa Kamera - İşlənmiş Orijinal", 91, 7 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 34.00m, 1, null, new byte[0], "SKU-8", 3, 9, "Xiaomi Redmi Note 12 Pro Ana Plata - Orijinal Zavod", 104, 8 },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 37.00m, 2, null, new byte[0], "SKU-9", 1, 10, "Huawei P60 Pro Klaviatura - A-Class", 117, 9 },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 40.00m, 3, null, new byte[0], "SKU-10", 2, 11, "Apple iPad Pro 12.9 (2024) Ekran - B-Class", 130, 10 },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 43.00m, 4, null, new byte[0], "SKU-11", 3, 12, "Apple iPad Air (2024) Batareya - İşlənmiş Orijinal", 143, 11 },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 46.00m, 1, null, new byte[0], "SKU-12", 1, 13, "Samsung Galaxy Tab S9 Ultra Arxa Kamera - Orijinal Zavod", 156, 0 },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 49.00m, 2, null, new byte[0], "SKU-13", 2, 14, "Apple MacBook Pro 14 (M3) Ana Plata - A-Class", 169, 1 },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 52.00m, 3, null, new byte[0], "SKU-14", 3, 15, "Apple MacBook Air 13 (M2) Klaviatura - B-Class", 182, 2 },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 55.00m, 4, null, new byte[0], "SKU-15", 1, 16, "Dell XPS 13 Plus Ekran - İşlənmiş Orijinal", 195, 3 },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 17, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 58.00m, 1, null, new byte[0], "SKU-16", 2, 17, "Dell XPS 15 Batareya - Orijinal Zavod", 208, 4 },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 1, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 61.00m, 2, null, new byte[0], "SKU-17", 3, 18, "Apple iPhone 15 Pro Max Arxa Kamera - A-Class", 221, 5 },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 64.00m, 3, null, new byte[0], "SKU-18", 1, 19, "Apple iPhone 15 Pro Ana Plata - B-Class", 234, 6 },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 67.00m, 4, null, new byte[0], "SKU-19", 2, 20, "Apple iPhone 14 Pro Max Klaviatura - İşlənmiş Orijinal", 247, 7 },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 70.00m, 1, null, new byte[0], "SKU-20", 3, 1, "Apple iPhone 13 Ekran - Orijinal Zavod", 260, 8 },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 73.00m, 2, null, new byte[0], "SKU-21", 1, 2, "Samsung Galaxy S24 Ultra Batareya - A-Class", 273, 9 },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 76.00m, 3, null, new byte[0], "SKU-22", 2, 3, "Samsung Galaxy S23 Arxa Kamera - B-Class", 286, 10 },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 79.00m, 4, null, new byte[0], "SKU-23", 3, 4, "Samsung Galaxy A54 Ana Plata - İşlənmiş Orijinal", 299, 11 },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 82.00m, 1, null, new byte[0], "SKU-24", 1, 5, "Xiaomi Xiaomi 13 Pro Klaviatura - Orijinal Zavod", 12, 0 },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 85.00m, 2, null, new byte[0], "SKU-25", 2, 6, "Xiaomi Redmi Note 12 Pro Ekran - A-Class", 25, 1 },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 88.00m, 3, null, new byte[0], "SKU-26", 3, 7, "Huawei P60 Pro Batareya - B-Class", 38, 2 },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 91.00m, 4, null, new byte[0], "SKU-27", 1, 8, "Apple iPad Pro 12.9 (2024) Arxa Kamera - İşlənmiş Orijinal", 51, 3 },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 94.00m, 1, null, new byte[0], "SKU-28", 2, 9, "Apple iPad Air (2024) Ana Plata - Orijinal Zavod", 64, 4 },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 97.00m, 2, null, new byte[0], "SKU-29", 3, 10, "Samsung Galaxy Tab S9 Ultra Klaviatura - A-Class", 77, 5 },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 100.00m, 3, null, new byte[0], "SKU-30", 1, 11, "Apple MacBook Pro 14 (M3) Ekran - B-Class", 90, 6 },
                    { 31, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 103.00m, 4, null, new byte[0], "SKU-31", 2, 12, "Apple MacBook Air 13 (M2) Batareya - İşlənmiş Orijinal", 103, 7 },
                    { 32, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 106.00m, 1, null, new byte[0], "SKU-32", 3, 13, "Dell XPS 13 Plus Arxa Kamera - Orijinal Zavod", 116, 8 },
                    { 33, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 17, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 109.00m, 2, null, new byte[0], "SKU-33", 1, 14, "Dell XPS 15 Ana Plata - A-Class", 129, 9 },
                    { 34, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 1, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 112.00m, 3, null, new byte[0], "SKU-34", 2, 15, "Apple iPhone 15 Pro Max Klaviatura - B-Class", 142, 10 },
                    { 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 115.00m, 4, null, new byte[0], "SKU-35", 3, 16, "Apple iPhone 15 Pro Ekran - İşlənmiş Orijinal", 155, 11 },
                    { 36, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 118.00m, 1, null, new byte[0], "SKU-36", 1, 17, "Apple iPhone 14 Pro Max Batareya - Orijinal Zavod", 168, 0 },
                    { 37, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 121.00m, 2, null, new byte[0], "SKU-37", 2, 18, "Apple iPhone 13 Arxa Kamera - A-Class", 181, 1 },
                    { 38, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 124.00m, 3, null, new byte[0], "SKU-38", 3, 19, "Samsung Galaxy S24 Ultra Ana Plata - B-Class", 194, 2 },
                    { 39, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 127.00m, 4, null, new byte[0], "SKU-39", 1, 20, "Samsung Galaxy S23 Klaviatura - İşlənmiş Orijinal", 207, 3 },
                    { 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 130.00m, 1, null, new byte[0], "SKU-40", 2, 1, "Samsung Galaxy A54 Ekran - Orijinal Zavod", 220, 4 },
                    { 41, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 133.00m, 2, null, new byte[0], "SKU-41", 3, 2, "Xiaomi Xiaomi 13 Pro Batareya - A-Class", 233, 5 },
                    { 42, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 136.00m, 3, null, new byte[0], "SKU-42", 1, 3, "Xiaomi Redmi Note 12 Pro Arxa Kamera - B-Class", 246, 6 },
                    { 43, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 139.00m, 4, null, new byte[0], "SKU-43", 2, 4, "Huawei P60 Pro Ana Plata - İşlənmiş Orijinal", 259, 7 },
                    { 44, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 142.00m, 1, null, new byte[0], "SKU-44", 3, 5, "Apple iPad Pro 12.9 (2024) Klaviatura - Orijinal Zavod", 272, 8 },
                    { 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 145.00m, 2, null, new byte[0], "SKU-45", 1, 6, "Apple iPad Air (2024) Ekran - A-Class", 285, 9 },
                    { 46, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 148.00m, 3, null, new byte[0], "SKU-46", 2, 7, "Samsung Galaxy Tab S9 Ultra Batareya - B-Class", 298, 10 },
                    { 47, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 151.00m, 4, null, new byte[0], "SKU-47", 3, 8, "Apple MacBook Pro 14 (M3) Arxa Kamera - İşlənmiş Orijinal", 11, 11 },
                    { 48, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 154.00m, 1, null, new byte[0], "SKU-48", 1, 9, "Apple MacBook Air 13 (M2) Ana Plata - Orijinal Zavod", 24, 0 },
                    { 49, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 157.00m, 2, null, new byte[0], "SKU-49", 2, 10, "Dell XPS 13 Plus Klaviatura - A-Class", 37, 1 },
                    { 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 17, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 10.00m, 3, null, new byte[0], "SKU-50", 3, 11, "Dell XPS 15 Ekran - B-Class", 50, 2 },
                    { 51, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 1, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 13.00m, 4, null, new byte[0], "SKU-51", 1, 12, "Apple iPhone 15 Pro Max Batareya - İşlənmiş Orijinal", 63, 3 },
                    { 52, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 16.00m, 1, null, new byte[0], "SKU-52", 2, 13, "Apple iPhone 15 Pro Arxa Kamera - Orijinal Zavod", 76, 4 },
                    { 53, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 19.00m, 2, null, new byte[0], "SKU-53", 3, 14, "Apple iPhone 14 Pro Max Ana Plata - A-Class", 89, 5 },
                    { 54, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 22.00m, 3, null, new byte[0], "SKU-54", 1, 15, "Apple iPhone 13 Klaviatura - B-Class", 102, 6 },
                    { 55, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 25.00m, 4, null, new byte[0], "SKU-55", 2, 16, "Samsung Galaxy S24 Ultra Ekran - İşlənmiş Orijinal", 115, 7 },
                    { 56, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 28.00m, 1, null, new byte[0], "SKU-56", 3, 17, "Samsung Galaxy S23 Batareya - Orijinal Zavod", 128, 8 },
                    { 57, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 31.00m, 2, null, new byte[0], "SKU-57", 1, 18, "Samsung Galaxy A54 Arxa Kamera - A-Class", 141, 9 },
                    { 58, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 34.00m, 3, null, new byte[0], "SKU-58", 2, 19, "Xiaomi Xiaomi 13 Pro Ana Plata - B-Class", 154, 10 },
                    { 59, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 37.00m, 4, null, new byte[0], "SKU-59", 3, 20, "Xiaomi Redmi Note 12 Pro Klaviatura - İşlənmiş Orijinal", 167, 11 },
                    { 60, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 40.00m, 1, null, new byte[0], "SKU-60", 1, 1, "Huawei P60 Pro Ekran - Orijinal Zavod", 180, 0 },
                    { 61, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 43.00m, 2, null, new byte[0], "SKU-61", 2, 2, "Apple iPad Pro 12.9 (2024) Batareya - A-Class", 193, 1 },
                    { 62, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 46.00m, 3, null, new byte[0], "SKU-62", 3, 3, "Apple iPad Air (2024) Arxa Kamera - B-Class", 206, 2 },
                    { 63, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 49.00m, 4, null, new byte[0], "SKU-63", 1, 4, "Samsung Galaxy Tab S9 Ultra Ana Plata - İşlənmiş Orijinal", 219, 3 },
                    { 64, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 52.00m, 1, null, new byte[0], "SKU-64", 2, 5, "Apple MacBook Pro 14 (M3) Klaviatura - Orijinal Zavod", 232, 4 },
                    { 65, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 55.00m, 2, null, new byte[0], "SKU-65", 3, 6, "Apple MacBook Air 13 (M2) Ekran - A-Class", 245, 5 },
                    { 66, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 58.00m, 3, null, new byte[0], "SKU-66", 1, 7, "Dell XPS 13 Plus Batareya - B-Class", 258, 6 },
                    { 67, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 17, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 61.00m, 4, null, new byte[0], "SKU-67", 2, 8, "Dell XPS 15 Arxa Kamera - İşlənmiş Orijinal", 271, 7 },
                    { 68, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 1, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 64.00m, 1, null, new byte[0], "SKU-68", 3, 9, "Apple iPhone 15 Pro Max Ana Plata - Orijinal Zavod", 284, 8 },
                    { 69, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 67.00m, 2, null, new byte[0], "SKU-69", 1, 10, "Apple iPhone 15 Pro Klaviatura - A-Class", 297, 9 },
                    { 70, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 70.00m, 3, null, new byte[0], "SKU-70", 2, 11, "Apple iPhone 14 Pro Max Ekran - B-Class", 10, 10 },
                    { 71, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 73.00m, 4, null, new byte[0], "SKU-71", 3, 12, "Apple iPhone 13 Batareya - İşlənmiş Orijinal", 23, 11 },
                    { 72, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 76.00m, 1, null, new byte[0], "SKU-72", 1, 13, "Samsung Galaxy S24 Ultra Arxa Kamera - Orijinal Zavod", 36, 0 },
                    { 73, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 79.00m, 2, null, new byte[0], "SKU-73", 2, 14, "Samsung Galaxy S23 Ana Plata - A-Class", 49, 1 },
                    { 74, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 82.00m, 3, null, new byte[0], "SKU-74", 3, 15, "Samsung Galaxy A54 Klaviatura - B-Class", 62, 2 },
                    { 75, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 85.00m, 4, null, new byte[0], "SKU-75", 1, 16, "Xiaomi Xiaomi 13 Pro Ekran - İşlənmiş Orijinal", 75, 3 },
                    { 76, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 88.00m, 1, null, new byte[0], "SKU-76", 2, 17, "Xiaomi Redmi Note 12 Pro Batareya - Orijinal Zavod", 88, 4 },
                    { 77, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 91.00m, 2, null, new byte[0], "SKU-77", 3, 18, "Huawei P60 Pro Arxa Kamera - A-Class", 101, 5 },
                    { 78, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 94.00m, 3, null, new byte[0], "SKU-78", 1, 19, "Apple iPad Pro 12.9 (2024) Ana Plata - B-Class", 114, 6 },
                    { 79, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 97.00m, 4, null, new byte[0], "SKU-79", 2, 20, "Apple iPad Air (2024) Klaviatura - İşlənmiş Orijinal", 127, 7 },
                    { 80, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 100.00m, 1, null, new byte[0], "SKU-80", 3, 1, "Samsung Galaxy Tab S9 Ultra Ekran - Orijinal Zavod", 140, 8 },
                    { 81, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 103.00m, 2, null, new byte[0], "SKU-81", 1, 2, "Apple MacBook Pro 14 (M3) Batareya - A-Class", 153, 9 },
                    { 82, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 106.00m, 3, null, new byte[0], "SKU-82", 2, 3, "Apple MacBook Air 13 (M2) Arxa Kamera - B-Class", 166, 10 },
                    { 83, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 109.00m, 4, null, new byte[0], "SKU-83", 3, 4, "Dell XPS 13 Plus Ana Plata - İşlənmiş Orijinal", 179, 11 },
                    { 84, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 17, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 112.00m, 1, null, new byte[0], "SKU-84", 1, 5, "Dell XPS 15 Klaviatura - Orijinal Zavod", 192, 0 },
                    { 85, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 1, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 115.00m, 2, null, new byte[0], "SKU-85", 2, 6, "Apple iPhone 15 Pro Max Ekran - A-Class", 205, 1 },
                    { 86, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 2, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 118.00m, 3, null, new byte[0], "SKU-86", 3, 7, "Apple iPhone 15 Pro Batareya - B-Class", 218, 2 },
                    { 87, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 3, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 121.00m, 4, null, new byte[0], "SKU-87", 1, 8, "Apple iPhone 14 Pro Max Arxa Kamera - İşlənmiş Orijinal", 231, 3 },
                    { 88, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 4, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 124.00m, 1, null, new byte[0], "SKU-88", 2, 9, "Apple iPhone 13 Ana Plata - Orijinal Zavod", 244, 4 },
                    { 89, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 5, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 127.00m, 2, null, new byte[0], "SKU-89", 3, 10, "Samsung Galaxy S24 Ultra Klaviatura - A-Class", 257, 5 },
                    { 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 6, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 130.00m, 3, null, new byte[0], "SKU-90", 1, 11, "Samsung Galaxy S23 Ekran - B-Class", 270, 6 },
                    { 91, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 7, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 133.00m, 4, null, new byte[0], "SKU-91", 2, 12, "Samsung Galaxy A54 Batareya - İşlənmiş Orijinal", 283, 7 },
                    { 92, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 8, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 136.00m, 1, null, new byte[0], "SKU-92", 3, 13, "Xiaomi Xiaomi 13 Pro Arxa Kamera - Orijinal Zavod", 296, 8 },
                    { 93, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 9, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 139.00m, 2, null, new byte[0], "SKU-93", 1, 14, "Xiaomi Redmi Note 12 Pro Ana Plata - A-Class", 9, 9 },
                    { 94, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 10, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 142.00m, 3, null, new byte[0], "SKU-94", 2, 15, "Huawei P60 Pro Klaviatura - B-Class", 22, 10 },
                    { 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 11, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 145.00m, 4, null, new byte[0], "SKU-95", 3, 16, "Apple iPad Pro 12.9 (2024) Ekran - İşlənmiş Orijinal", 35, 11 },
                    { 96, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 12, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 148.00m, 1, null, new byte[0], "SKU-96", 1, 17, "Apple iPad Air (2024) Batareya - Orijinal Zavod", 48, 0 },
                    { 97, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 13, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 3, 151.00m, 2, null, new byte[0], "SKU-97", 2, 18, "Samsung Galaxy Tab S9 Ultra Arxa Kamera - A-Class", 61, 1 },
                    { 98, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 14, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, 154.00m, 3, null, new byte[0], "SKU-98", 3, 19, "Apple MacBook Pro 14 (M3) Ana Plata - B-Class", 74, 2 },
                    { 99, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 15, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 11, 157.00m, 4, null, new byte[0], "SKU-99", 1, 20, "Apple MacBook Air 13 (M2) Klaviatura - İşlənmiş Orijinal", 87, 3 },
                    { 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi.", 16, null, true, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 10.00m, 1, null, new byte[0], "SKU-100", 2, 1, "Dell XPS 13 Plus Ekran - Orijinal Zavod", 100, 4 }
                });

            migrationBuilder.InsertData(
                table: "SparePartImages",
                columns: new[] { "Id", "DisplayOrder", "ImageUrl", "IsMain", "SparePartId" },
                values: new object[,]
                {
                    { 1, 1, "/images/images (2).jpg", true, 1 },
                    { 2, 1, "/images/images (3).jpg", true, 2 },
                    { 3, 1, "/images/images (4).jpg", true, 3 },
                    { 4, 1, "/images/images (5).jpg", true, 4 },
                    { 5, 1, "/images/images (6).jpg", true, 5 },
                    { 6, 1, "/images/images (7).jpg", true, 6 },
                    { 7, 1, "/images/images (1).jpg", true, 7 },
                    { 8, 1, "/images/images (2).jpg", true, 8 },
                    { 9, 1, "/images/images (3).jpg", true, 9 },
                    { 10, 1, "/images/images (4).jpg", true, 10 },
                    { 11, 1, "/images/images (5).jpg", true, 11 },
                    { 12, 1, "/images/images (6).jpg", true, 12 },
                    { 13, 1, "/images/images (7).jpg", true, 13 },
                    { 14, 1, "/images/images (1).jpg", true, 14 },
                    { 15, 1, "/images/images (2).jpg", true, 15 },
                    { 16, 1, "/images/images (3).jpg", true, 16 },
                    { 17, 1, "/images/images (4).jpg", true, 17 },
                    { 18, 1, "/images/images (5).jpg", true, 18 },
                    { 19, 1, "/images/images (6).jpg", true, 19 },
                    { 20, 1, "/images/images (7).jpg", true, 20 },
                    { 21, 1, "/images/images (1).jpg", true, 21 },
                    { 22, 1, "/images/images (2).jpg", true, 22 },
                    { 23, 1, "/images/images (3).jpg", true, 23 },
                    { 24, 1, "/images/images (4).jpg", true, 24 },
                    { 25, 1, "/images/images (5).jpg", true, 25 },
                    { 26, 1, "/images/images (6).jpg", true, 26 },
                    { 27, 1, "/images/images (7).jpg", true, 27 },
                    { 28, 1, "/images/images (1).jpg", true, 28 },
                    { 29, 1, "/images/images (2).jpg", true, 29 },
                    { 30, 1, "/images/images (3).jpg", true, 30 },
                    { 31, 1, "/images/images (4).jpg", true, 31 },
                    { 32, 1, "/images/images (5).jpg", true, 32 },
                    { 33, 1, "/images/images (6).jpg", true, 33 },
                    { 34, 1, "/images/images (7).jpg", true, 34 },
                    { 35, 1, "/images/images (1).jpg", true, 35 },
                    { 36, 1, "/images/images (2).jpg", true, 36 },
                    { 37, 1, "/images/images (3).jpg", true, 37 },
                    { 38, 1, "/images/images (4).jpg", true, 38 },
                    { 39, 1, "/images/images (5).jpg", true, 39 },
                    { 40, 1, "/images/images (6).jpg", true, 40 },
                    { 41, 1, "/images/images (7).jpg", true, 41 },
                    { 42, 1, "/images/images (1).jpg", true, 42 },
                    { 43, 1, "/images/images (2).jpg", true, 43 },
                    { 44, 1, "/images/images (3).jpg", true, 44 },
                    { 45, 1, "/images/images (4).jpg", true, 45 },
                    { 46, 1, "/images/images (5).jpg", true, 46 },
                    { 47, 1, "/images/images (6).jpg", true, 47 },
                    { 48, 1, "/images/images (7).jpg", true, 48 },
                    { 49, 1, "/images/images (1).jpg", true, 49 },
                    { 50, 1, "/images/images (2).jpg", true, 50 },
                    { 51, 1, "/images/images (3).jpg", true, 51 },
                    { 52, 1, "/images/images (4).jpg", true, 52 },
                    { 53, 1, "/images/images (5).jpg", true, 53 },
                    { 54, 1, "/images/images (6).jpg", true, 54 },
                    { 55, 1, "/images/images (7).jpg", true, 55 },
                    { 56, 1, "/images/images (1).jpg", true, 56 },
                    { 57, 1, "/images/images (2).jpg", true, 57 },
                    { 58, 1, "/images/images (3).jpg", true, 58 },
                    { 59, 1, "/images/images (4).jpg", true, 59 },
                    { 60, 1, "/images/images (5).jpg", true, 60 },
                    { 61, 1, "/images/images (6).jpg", true, 61 },
                    { 62, 1, "/images/images (7).jpg", true, 62 },
                    { 63, 1, "/images/images (1).jpg", true, 63 },
                    { 64, 1, "/images/images (2).jpg", true, 64 },
                    { 65, 1, "/images/images (3).jpg", true, 65 },
                    { 66, 1, "/images/images (4).jpg", true, 66 },
                    { 67, 1, "/images/images (5).jpg", true, 67 },
                    { 68, 1, "/images/images (6).jpg", true, 68 },
                    { 69, 1, "/images/images (7).jpg", true, 69 },
                    { 70, 1, "/images/images (1).jpg", true, 70 },
                    { 71, 1, "/images/images (2).jpg", true, 71 },
                    { 72, 1, "/images/images (3).jpg", true, 72 },
                    { 73, 1, "/images/images (4).jpg", true, 73 },
                    { 74, 1, "/images/images (5).jpg", true, 74 },
                    { 75, 1, "/images/images (6).jpg", true, 75 },
                    { 76, 1, "/images/images (7).jpg", true, 76 },
                    { 77, 1, "/images/images (1).jpg", true, 77 },
                    { 78, 1, "/images/images (2).jpg", true, 78 },
                    { 79, 1, "/images/images (3).jpg", true, 79 },
                    { 80, 1, "/images/images (4).jpg", true, 80 },
                    { 81, 1, "/images/images (5).jpg", true, 81 },
                    { 82, 1, "/images/images (6).jpg", true, 82 },
                    { 83, 1, "/images/images (7).jpg", true, 83 },
                    { 84, 1, "/images/images (1).jpg", true, 84 },
                    { 85, 1, "/images/images (2).jpg", true, 85 },
                    { 86, 1, "/images/images (3).jpg", true, 86 },
                    { 87, 1, "/images/images (4).jpg", true, 87 },
                    { 88, 1, "/images/images (5).jpg", true, 88 },
                    { 89, 1, "/images/images (6).jpg", true, 89 },
                    { 90, 1, "/images/images (7).jpg", true, 90 },
                    { 91, 1, "/images/images (1).jpg", true, 91 },
                    { 92, 1, "/images/images (2).jpg", true, 92 },
                    { 93, 1, "/images/images (3).jpg", true, 93 },
                    { 94, 1, "/images/images (4).jpg", true, 94 },
                    { 95, 1, "/images/images (5).jpg", true, 95 },
                    { 96, 1, "/images/images (6).jpg", true, 96 },
                    { 97, 1, "/images/images (7).jpg", true, 97 },
                    { 98, 1, "/images/images (1).jpg", true, 98 },
                    { 99, 1, "/images/images (2).jpg", true, 99 },
                    { 100, 1, "/images/images (3).jpg", true, 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_BrandId",
                table: "DeviceModels",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_CategoryId",
                table: "DeviceModels",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfiles_UserId",
                table: "SellerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SparePartImages_SparePartId",
                table: "SparePartImages",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartReviews_SparePartId",
                table: "SparePartReviews",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_DeviceModelId",
                table: "SpareParts",
                column: "DeviceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_QualityTypeId",
                table: "SpareParts",
                column: "QualityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartTypeId",
                table: "SpareParts",
                column: "PartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_SellerId",
                table: "SpareParts",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartSaleAudits_SparePartId",
                table: "SparePartSaleAudits",
                column: "SparePartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "ApprovalHistories");

            migrationBuilder.DropTable(
                name: "CategoryBrands");

            migrationBuilder.DropTable(
                name: "CategoryPartTypes");

            migrationBuilder.DropTable(
                name: "SparePartImages");

            migrationBuilder.DropTable(
                name: "SparePartReviews");

            migrationBuilder.DropTable(
                name: "SparePartSaleAudits");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "DeviceModels");

            migrationBuilder.DropTable(
                name: "QualityTypes");

            migrationBuilder.DropTable(
                name: "SellerProfiles");

            migrationBuilder.DropTable(
                name: "SparePartTypes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "DeviceBrands");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
