using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EHisse.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PostgresInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TargetUserId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryBrands",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryBrands", x => new { x.CategoryId, x.BrandId });
                });

            migrationBuilder.CreateTable(
                name: "CategoryPartTypes",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    PartTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPartTypes", x => new { x.CategoryId, x.PartTypeId });
                });

            migrationBuilder.CreateTable(
                name: "DeviceBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QualityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SparePartTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    IsPortable = table.Column<bool>(type: "boolean", nullable: false),
                    IsAccessory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModelCode = table.Column<string>(type: "text", nullable: false),
                    ReleaseYear = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    ContactPerson = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    WhatsAppNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false),
                    ShowContactInfoInAds = table.Column<bool>(type: "boolean", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    DeviceModelId = table.Column<int>(type: "integer", nullable: false),
                    PartTypeId = table.Column<int>(type: "integer", nullable: false),
                    QualityTypeId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    MinOrderQuantity = table.Column<int>(type: "integer", nullable: false),
                    WarrantyMonths = table.Column<int>(type: "integer", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    ModerationStatus = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    IsUpdateSubmission = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    LocationAddress = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SparePartId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SparePartId = table.Column<int>(type: "integer", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SparePartId = table.Column<int>(type: "integer", nullable: false),
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PreviousStock = table.Column<int>(type: "integer", nullable: false),
                    NewStock = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    { 5, "C_CLASS", "Əsas funksiyalar, ən aşağı qiymət", 5, true, "C-Class (Ekonom)" }
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
                columns: new[] { "Id", "ApprovalStatus", "CreatedDate", "Email", "FirstName", "IsBlocked", "LastName", "PasswordHash", "Phone", "RejectionReason", "Role", "Username" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 10, 9, 0, 29, 38, 898, DateTimeKind.Local).AddTicks(1294), "elcin@example.com", "Elçin", false, "Məmmədov", "123", "+994501234567", null, 2, "elcin" },
                    { 2, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7625), "rashad@example.com", "Rəşad", false, "Əliyev", "123", "+994551234568", null, 3, "rashad" },
                    { 3, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7666), "nigar@example.com", "Nigar", false, "Həsənova", "123", "+994701234569", null, 2, "nigar" },
                    { 4, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7671), "admin@example.com", "Baş", false, "Administrator", "admin", "+994000000000", null, 1, "admin" },
                    { 5, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7673), "samir@example.com", "Samir", false, "Quliyev", "123", "+994501231122", null, 2, "samir" },
                    { 6, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7675), "vuqar@example.com", "Vuqar", false, "Hesenov", "123", "+994551233344", null, 3, "vuqar" },
                    { 7, 2, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7678), "leyla@example.com", "Leyla", false, "Memmedova", "123", "+994701235566", null, 2, "leyla" },
                    { 8, 1, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7680), "taleh@example.com", "Taleh", false, "Qarayev", "123", "+994511237788", null, 3, "taleh" },
                    { 9, 1, new DateTime(2026, 10, 9, 0, 29, 38, 899, DateTimeKind.Local).AddTicks(7682), "rustam@example.com", "Rustam", false, "Aliyev", "123", "+994501239900", null, 2, "rustam" }
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
                    { 2, "Bakı, Yasamal rayonu, Cəfər Cabbarlı küç. 44", "Mobile Parts Store", "Rəşad Əliyev", true, "+994551234568", 4.5m, true, 2, null },
                    { 3, "Bakı, Nərimanov rayonu, Əhmədli küç. 9", "SmartFix Təmir Mərkəzi", "Nigar Həsənova", true, "+994701234569", 4.9m, true, 3, null }
                });

            migrationBuilder.InsertData(
                table: "SpareParts",
                columns: new[] { "Id", "CreatedDate", "Currency", "Description", "DeviceModelId", "DiscountPrice", "IsActive", "IsDeleted", "IsUpdateSubmission", "LastModifiedDate", "Latitude", "LocationAddress", "Longitude", "MinOrderQuantity", "ModerationStatus", "PartTypeId", "Price", "QualityTypeId", "RejectionReason", "RowVersion", "SKU", "SellerId", "StockQuantity", "Title", "ViewCount", "WarrantyMonths" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 41.25m, 3, null, new byte[0], "SKU-1", 1, 4, "iPhone 15 Pro Ekran - A-Class", 356, 12 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 19, 23.12m, 1, null, new byte[0], "SKU-2", 1, 8, "iPhone 15 Pro Korpus Hissəsi - Orijinal Zavod", 268, 12 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 1, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 40.86m, 5, null, new byte[0], "SKU-3", 2, 19, "iPhone 15 Pro Max Dinamik - C-Class", 152, 0 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 3, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 19, 31.77m, 3, null, new byte[0], "SKU-4", 2, 11, "iPhone 14 Pro Max Korpus Hissəsi - A-Class", 62, 0 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 7, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 94.52m, 3, null, new byte[0], "SKU-5", 2, 2, "Galaxy A54 Ön Kamera - A-Class", 383, 6 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 9, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 87.29m, 4, null, new byte[0], "SKU-6", 1, 20, "Redmi Note 12 Pro Ön Kamera - B-Class", 463, 3 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 10, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 102.58m, 1, null, new byte[0], "SKU-7", 1, 10, "P60 Pro Dinamik - Orijinal Zavod", 50, 1 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 18, 99.0m, 3, null, new byte[0], "SKU-8", 2, 12, "iPhone 15 Pro Dinamik Setkası - A-Class", 93, 3 },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 6, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 141.13m, 3, null, new byte[0], "SKU-9", 3, 3, "Galaxy S23 Dinamik - A-Class", 321, 1 },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 9, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 63.12m, 2, null, new byte[0], "SKU-10", 2, 18, "Redmi Note 12 Pro Şarj Portu - İşlənmiş Orijinal", 122, 3 },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 1, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 66.16m, 1, null, new byte[0], "SKU-11", 2, 3, "iPhone 15 Pro Max Şarj Portu - Orijinal Zavod", 118, 12 },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 6, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 133.86m, 4, null, new byte[0], "SKU-12", 2, 15, "Galaxy S23 Dinamik - B-Class", 83, 3 },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 3, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 46.78m, 5, null, new byte[0], "SKU-13", 3, 19, "iPhone 14 Pro Max Şarj Portu - C-Class", 229, 12 },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 7, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 17, 81.33m, 2, null, new byte[0], "SKU-14", 1, 3, "Galaxy A54 Antenna - İşlənmiş Orijinal", 396, 0 },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 69.1m, 2, null, new byte[0], "SKU-15", 3, 3, "iPhone 15 Pro Ana Plata - İşlənmiş Orijinal", 207, 6 },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 10, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 20, 145.95m, 5, null, new byte[0], "SKU-16", 2, 1, "P60 Pro Korpus - C-Class", 358, 0 },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 9, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 9, 51.09m, 3, null, new byte[0], "SKU-17", 1, 6, "Redmi Note 12 Pro Sim Tutucu - A-Class", 242, 0 },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 5, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 22, 137.77m, 2, null, new byte[0], "SKU-18", 3, 10, "Galaxy S24 Ultra OCA Film - İşlənmiş Orijinal", 440, 12 },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 10, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 116.75m, 2, null, new byte[0], "SKU-19", 2, 18, "P60 Pro Dinamik - İşlənmiş Orijinal", 498, 12 },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 1, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 16, 25.66m, 4, null, new byte[0], "SKU-20", 1, 12, "iPhone 15 Pro Max Sensor Şüşə - B-Class", 459, 3 },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 4, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 142.57m, 2, null, new byte[0], "SKU-21", 3, 3, "iPhone 13 Batareya - İşlənmiş Orijinal", 384, 6 },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 76.54m, 2, null, new byte[0], "SKU-22", 3, 18, "iPhone 15 Pro Ana Plata - İşlənmiş Orijinal", 94, 3 },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 9, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 19, 115.74m, 2, null, new byte[0], "SKU-23", 3, 7, "Redmi Note 12 Pro Korpus Hissəsi - İşlənmiş Orijinal", 375, 3 },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 7, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 17, 73.21m, 4, null, new byte[0], "SKU-24", 3, 8, "Galaxy A54 Antenna - B-Class", 125, 0 },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 6, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 42.22m, 5, null, new byte[0], "SKU-25", 3, 8, "Galaxy S23 Ekran - C-Class", 13, 0 },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 1, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 130.35m, 1, null, new byte[0], "SKU-26", 1, 3, "iPhone 15 Pro Max Şarj Portu - Orijinal Zavod", 273, 1 },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 5, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 21, 28.52m, 2, null, new byte[0], "SKU-27", 3, 19, "Galaxy S24 Ultra Mikrofon - İşlənmiş Orijinal", 305, 6 },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 4, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 21, 23.21m, 4, null, new byte[0], "SKU-28", 1, 14, "iPhone 13 Mikrofon - B-Class", 191, 6 },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 7, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 20, 101.48m, 1, null, new byte[0], "SKU-29", 3, 4, "Galaxy A54 Korpus - Orijinal Zavod", 41, 6 },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 6, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 36.63m, 2, null, new byte[0], "SKU-30", 1, 15, "Galaxy S23 Ön Kamera - İşlənmiş Orijinal", 81, 6 },
                    { 31, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 3, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 9, 132.43m, 4, null, new byte[0], "SKU-31", 1, 3, "iPhone 14 Pro Max Sim Tutucu - B-Class", 236, 12 },
                    { 32, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 2, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 145.66m, 5, null, new byte[0], "SKU-32", 1, 8, "iPhone 15 Pro Batareya - C-Class", 95, 6 },
                    { 33, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 8, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 21, 136.34m, 2, null, new byte[0], "SKU-33", 2, 6, "Xiaomi 13 Pro Mikrofon - İşlənmiş Orijinal", 204, 0 },
                    { 34, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 7, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 9, 69.22m, 4, null, new byte[0], "SKU-34", 2, 18, "Galaxy A54 Sim Tutucu - B-Class", 348, 6 },
                    { 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 3, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 7, 145.62m, 3, null, new byte[0], "SKU-35", 1, 19, "iPhone 14 Pro Max Dinamik - A-Class", 386, 12 },
                    { 36, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 6, 91.78m, 1, null, new byte[0], "SKU-36", 1, 17, "iPad Pro 12.9 Arxa Qapaq - Orijinal Zavod", 480, 12 },
                    { 37, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 129.19m, 5, null, new byte[0], "SKU-37", 1, 3, "iPad Pro 12.9 Ekran - C-Class", 314, 0 },
                    { 38, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 141.82m, 4, null, new byte[0], "SKU-38", 1, 19, "Galaxy Tab S9 Ultra Ön Kamera - B-Class", 136, 12 },
                    { 39, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 68.69m, 5, null, new byte[0], "SKU-39", 1, 19, "Galaxy Tab S9 Ultra Ekran - C-Class", 299, 12 },
                    { 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 110.27m, 2, null, new byte[0], "SKU-40", 3, 8, "iPad Air Ana Plata - İşlənmiş Orijinal", 145, 6 },
                    { 41, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 140.06m, 4, null, new byte[0], "SKU-41", 2, 3, "iPad Pro 12.9 Ana Plata - B-Class", 14, 6 },
                    { 42, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 39.84m, 1, null, new byte[0], "SKU-42", 3, 9, "Galaxy Tab S9 Ultra Batareya - Orijinal Zavod", 77, 3 },
                    { 43, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 32.08m, 3, null, new byte[0], "SKU-43", 2, 18, "iPad Pro 12.9 Ön Kamera - A-Class", 370, 3 },
                    { 44, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 140.45m, 5, null, new byte[0], "SKU-44", 2, 4, "Galaxy Tab S9 Ultra Ekran - C-Class", 490, 1 },
                    { 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 87.45m, 1, null, new byte[0], "SKU-45", 3, 9, "iPad Air Batareya - Orijinal Zavod", 154, 12 },
                    { 46, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 6, 98.79m, 2, null, new byte[0], "SKU-46", 3, 9, "iPad Pro 12.9 Arxa Qapaq - İşlənmiş Orijinal", 268, 6 },
                    { 47, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 69.3m, 1, null, new byte[0], "SKU-47", 3, 9, "iPad Air Ekran - Orijinal Zavod", 32, 0 },
                    { 48, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 3, 113.77m, 3, null, new byte[0], "SKU-48", 1, 18, "iPad Air Arxa Kamera - A-Class", 371, 6 },
                    { 49, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 142.31m, 1, null, new byte[0], "SKU-49", 1, 5, "Galaxy Tab S9 Ultra Ekran - Orijinal Zavod", 289, 0 },
                    { 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 3, 15.86m, 4, null, new byte[0], "SKU-50", 1, 12, "iPad Air Arxa Kamera - B-Class", 470, 0 },
                    { 51, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 24.39m, 2, null, new byte[0], "SKU-51", 3, 18, "iPad Air Ön Kamera - İşlənmiş Orijinal", 462, 6 },
                    { 52, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 3, 146.67m, 2, null, new byte[0], "SKU-52", 1, 6, "Galaxy Tab S9 Ultra Arxa Kamera - İşlənmiş Orijinal", 461, 6 },
                    { 53, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 3, 122.31m, 3, null, new byte[0], "SKU-53", 2, 8, "iPad Pro 12.9 Arxa Kamera - A-Class", 146, 1 },
                    { 54, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 130.2m, 4, null, new byte[0], "SKU-54", 1, 8, "Galaxy Tab S9 Ultra Batareya - B-Class", 112, 6 },
                    { 55, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 13.31m, 2, null, new byte[0], "SKU-55", 1, 7, "iPad Air Ana Plata - İşlənmiş Orijinal", 214, 3 },
                    { 56, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 99.81m, 3, null, new byte[0], "SKU-56", 2, 13, "iPad Air Batareya - A-Class", 357, 12 },
                    { 57, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 35.0m, 1, null, new byte[0], "SKU-57", 2, 9, "iPad Air Ekran - Orijinal Zavod", 29, 0 },
                    { 58, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 120.1m, 3, null, new byte[0], "SKU-58", 3, 14, "Galaxy Tab S9 Ultra Şarj Portu - A-Class", 320, 12 },
                    { 59, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 45.66m, 5, null, new byte[0], "SKU-59", 1, 14, "iPad Pro 12.9 Şarj Portu - C-Class", 10, 12 },
                    { 60, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 19.8m, 3, null, new byte[0], "SKU-60", 2, 11, "Galaxy Tab S9 Ultra Ön Kamera - A-Class", 329, 3 },
                    { 61, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 53.3m, 3, null, new byte[0], "SKU-61", 3, 14, "Galaxy Tab S9 Ultra Batareya - A-Class", 177, 6 },
                    { 62, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 36.86m, 5, null, new byte[0], "SKU-62", 1, 13, "Galaxy Tab S9 Ultra Ana Plata - C-Class", 356, 1 },
                    { 63, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 126.72m, 4, null, new byte[0], "SKU-63", 3, 10, "Galaxy Tab S9 Ultra Ana Plata - B-Class", 156, 1 },
                    { 64, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 6, 71.9m, 4, null, new byte[0], "SKU-64", 2, 7, "iPad Air Arxa Qapaq - B-Class", 271, 6 },
                    { 65, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 3, 82.16m, 1, null, new byte[0], "SKU-65", 2, 20, "Galaxy Tab S9 Ultra Arxa Kamera - Orijinal Zavod", 181, 0 },
                    { 66, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 30.63m, 2, null, new byte[0], "SKU-66", 1, 2, "iPad Pro 12.9 Ana Plata - İşlənmiş Orijinal", 135, 6 },
                    { 67, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 13, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 134.06m, 4, null, new byte[0], "SKU-67", 2, 19, "Galaxy Tab S9 Ultra Batareya - B-Class", 109, 6 },
                    { 68, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 8, 101.85m, 2, null, new byte[0], "SKU-68", 1, 1, "iPad Air Şarj Portu - İşlənmiş Orijinal", 467, 0 },
                    { 69, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 12, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 4, 82.51m, 2, null, new byte[0], "SKU-69", 3, 2, "iPad Air Ön Kamera - İşlənmiş Orijinal", 295, 1 },
                    { 70, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 11, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 10, 103.46m, 2, null, new byte[0], "SKU-70", 2, 18, "iPad Pro 12.9 Home Düyməsi - İşlənmiş Orijinal", 314, 3 },
                    { 71, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 135.61m, 5, null, new byte[0], "SKU-71", 2, 16, "XPS 15 Yaddaş - C-Class", 240, 3 },
                    { 72, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 12, 97.74m, 5, null, new byte[0], "SKU-72", 2, 9, "MacBook Air 13 Touchpad - C-Class", 235, 0 },
                    { 73, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 11, 54.76m, 3, null, new byte[0], "SKU-73", 2, 18, "XPS 13 Plus Klaviatura - A-Class", 51, 1 },
                    { 74, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 11, 31.39m, 4, null, new byte[0], "SKU-74", 3, 7, "MacBook Air 13 Klaviatura - B-Class", 42, 6 },
                    { 75, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 13, 68.21m, 5, null, new byte[0], "SKU-75", 2, 7, "XPS 15 RAM - C-Class", 436, 6 },
                    { 76, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 76.78m, 5, null, new byte[0], "SKU-76", 2, 12, "XPS 15 Ekran - C-Class", 162, 6 },
                    { 77, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 11, 48.21m, 4, null, new byte[0], "SKU-77", 1, 16, "XPS 15 Klaviatura - B-Class", 24, 6 },
                    { 78, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 138.74m, 2, null, new byte[0], "SKU-78", 2, 20, "XPS 13 Plus Yaddaş - İşlənmiş Orijinal", 283, 0 },
                    { 79, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 70.01m, 1, null, new byte[0], "SKU-79", 3, 15, "XPS 15 Ekran - Orijinal Zavod", 103, 0 },
                    { 80, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 73.66m, 3, null, new byte[0], "SKU-80", 1, 11, "XPS 13 Plus Yaddaş - A-Class", 399, 6 },
                    { 81, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 75.84m, 3, null, new byte[0], "SKU-81", 1, 18, "XPS 13 Plus Yaddaş - A-Class", 36, 3 },
                    { 82, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 142.95m, 1, null, new byte[0], "SKU-82", 1, 7, "MacBook Air 13 Batareya - Orijinal Zavod", 439, 0 },
                    { 83, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 11, 103.72m, 2, null, new byte[0], "SKU-83", 2, 19, "MacBook Air 13 Klaviatura - İşlənmiş Orijinal", 495, 1 },
                    { 84, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 12, 94.82m, 3, null, new byte[0], "SKU-84", 1, 4, "XPS 15 Touchpad - A-Class", 408, 1 },
                    { 85, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 140.04m, 5, null, new byte[0], "SKU-85", 1, 19, "XPS 13 Plus Batareya - C-Class", 356, 6 },
                    { 86, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 11, 106.69m, 1, null, new byte[0], "SKU-86", 3, 8, "XPS 15 Klaviatura - Orijinal Zavod", 62, 3 },
                    { 87, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 14, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 69.97m, 3, null, new byte[0], "SKU-87", 3, 12, "MacBook Pro 14 Ekran - A-Class", 45, 12 },
                    { 88, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 24.78m, 4, null, new byte[0], "SKU-88", 2, 12, "XPS 13 Plus Ekran - B-Class", 335, 6 },
                    { 89, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 83.04m, 2, null, new byte[0], "SKU-89", 3, 9, "MacBook Air 13 Yaddaş - İşlənmiş Orijinal", 325, 12 },
                    { 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 15, 92.95m, 4, null, new byte[0], "SKU-90", 3, 11, "XPS 15 Adapter - B-Class", 446, 1 },
                    { 91, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 14, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 12, 115.07m, 4, null, new byte[0], "SKU-91", 1, 19, "MacBook Pro 14 Touchpad - B-Class", 322, 6 },
                    { 92, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 35.46m, 4, null, new byte[0], "SKU-92", 2, 7, "XPS 13 Plus Ekran - B-Class", 191, 3 },
                    { 93, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 12, 133.22m, 5, null, new byte[0], "SKU-93", 3, 18, "XPS 13 Plus Touchpad - C-Class", 15, 12 },
                    { 94, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 66.9m, 2, null, new byte[0], "SKU-94", 3, 18, "MacBook Air 13 Batareya - İşlənmiş Orijinal", 398, 1 },
                    { 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 15, 23.03m, 4, null, new byte[0], "SKU-95", 1, 8, "XPS 15 Adapter - B-Class", 217, 1 },
                    { 96, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 13, 84.33m, 4, null, new byte[0], "SKU-96", 3, 14, "XPS 13 Plus RAM - B-Class", 391, 12 },
                    { 97, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 13, 52.93m, 4, null, new byte[0], "SKU-97", 2, 8, "XPS 13 Plus RAM - B-Class", 71, 1 },
                    { 98, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 35.92m, 5, null, new byte[0], "SKU-98", 3, 7, "XPS 13 Plus Batareya - C-Class", 388, 6 },
                    { 99, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 16, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 12, 51.47m, 1, null, new byte[0], "SKU-99", 1, 12, "XPS 13 Plus Touchpad - Orijinal Zavod", 101, 3 },
                    { 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 14, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 5, 146.4m, 3, null, new byte[0], "SKU-100", 1, 18, "MacBook Pro 14 Ana Plata - A-Class", 159, 1 },
                    { 101, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 2, 49.81m, 1, null, new byte[0], "SKU-101", 3, 16, "XPS 15 Batareya - Orijinal Zavod", 235, 3 },
                    { 102, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 15, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 25.97m, 3, null, new byte[0], "SKU-102", 2, 3, "MacBook Air 13 Ekran - A-Class", 215, 6 },
                    { 103, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 14, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 1, 123.56m, 2, null, new byte[0], "SKU-103", 1, 10, "MacBook Pro 14 Ekran - İşlənmiş Orijinal", 53, 1 },
                    { 104, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 14, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 14, 120.7m, 5, null, new byte[0], "SKU-104", 3, 8, "MacBook Pro 14 Yaddaş - C-Class", 407, 12 },
                    { 105, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AZN", "Yüksək keyfiyyətli ehtiyat hissəsi. Zəmanətli və etibarlı.", 17, null, true, false, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 1, 2, 15, 130.42m, 4, null, new byte[0], "SKU-105", 2, 14, "XPS 15 Adapter - B-Class", 166, 12 }
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
