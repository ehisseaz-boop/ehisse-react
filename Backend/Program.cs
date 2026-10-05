using EHisseler.Web.Models;
using EHisseler.Web.Services;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.SetIsOriginAllowed(origin => true) // Allow any frontend port/URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


// Azure Blob Storage servisi — Singleton, BlobServiceClient thread-safe-dir
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

// Register new services
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        // REST API: redirect əvəzinə status kodu qaytar (React frontend özü yönləndirir)
        options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
        options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Hisseler API V1");
 c.RoutePrefix = string.Empty; });

app.UseRouting();

// Enable CORS
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    // Fix any null SKUs in legacy data
    var nullSkus = context.SpareParts.Where(s => s.SKU == null || s.SKU == "").ToList();
    if(nullSkus.Any()) {
        foreach(var item in nullSkus) {
            item.SKU = "LEGACY-" + item.Id;
        }
        context.SaveChanges();
    }
    var categories = context.Categories.ToList();
    foreach (var c in categories)
    {
        // Fix for "Şəbəkə Avadanlıqları"
        if (c.Name.Contains("Avadanl") && (c.Name.Contains("b?k?") || c.Name.Contains("bəkə")))
        {
            c.Name = "Şəbəkə Avadanlıqları";
        }
        // Fix for "Alətlər və Təmir Avadanlıqları"
        else if (c.Name.Contains("T?mir") || c.Name.Contains("Al?tl?r") || c.Name.Contains("Alətlər"))
        {
            c.Name = "Alətlər və Təmir Avadanlıqları";
        }
        else if (c.Name.Contains("Saat"))
        {
            c.Name = "Smart Saat ehtiyat hissələri";
        }
        else if (c.Name.Contains("Qulaq"))
        {
            c.Name = "Qulaqlıq ehtiyat hissələri";
        }
    }
    context.SaveChanges();
    
    // Daha çox kateqoriya əlavə etmək (Seeding)
    if (context.Categories.Count() < 10)
    {
        var newCategories = new[]
        {
            new Category { Name = "Smart Saat ehtiyat hissələri", DisplayOrder = 4, IsActive = true },
            new Category { Name = "Qulaqlıq ehtiyat hissələri", DisplayOrder = 5, IsActive = true },
            new Category { Name = "Kompüter (PC) ehtiyat hissələri", DisplayOrder = 6, IsActive = true },
            new Category { Name = "Televizor ehtiyat hissələri", DisplayOrder = 7, IsActive = true },
            new Category { Name = "Oyun Konsolu ehtiyat hissələri", DisplayOrder = 8, IsActive = true },
            new Category { Name = "Foto/Video Kamera ehtiyat hissələri", DisplayOrder = 9, IsActive = true },
            new Category { Name = "Alətlər və Təmir Avadanlıqları", DisplayOrder = 10, IsActive = true },
            new Category { Name = "Dron ehtiyat hissələri", DisplayOrder = 11, IsActive = true },
            new Category { Name = "Elektrikli Samokat ehtiyat hissələri", DisplayOrder = 12, IsActive = true },
            new Category { Name = "Şəbəkə Avadanlıqları", DisplayOrder = 13, IsActive = true },
            new Category { Name = "Adapterlər və Kabellər", DisplayOrder = 14, IsActive = true }
        };
        
        foreach (var cat in newCategories)
        {
            if (!context.Categories.Any(c => c.Name == cat.Name))
            {
                context.Categories.Add(cat);
            }
        }
        context.SaveChanges();
    }
    
    // YENİ DÜZƏLİŞ: Yeni kateqoriyalar üçün asılı məlumatlar yoxdursa "Digər" adlı default məlumatlar əlavə etmək
    var emptyCategories = context.Categories.Where(c => !context.DeviceModels.Any(m => m.CategoryId == c.Id)).ToList();
    if (emptyCategories.Any())
    {
        var otherBrand = context.DeviceBrands.FirstOrDefault(b => b.Name == "Digər");
        if (otherBrand == null)
        {
            otherBrand = new DeviceBrand { Name = "Digər", IsActive = true };
            context.DeviceBrands.Add(otherBrand);
            context.SaveChanges();
        }

        var otherPartType = context.SparePartTypes.FirstOrDefault(pt => pt.Name == "Digər");
        if (otherPartType == null)
        {
            otherPartType = new SparePartType { Name = "Digər", Key = "other", IsAccessory = false, IsPortable = false };
            context.SparePartTypes.Add(otherPartType);
            context.SaveChanges();
        }

        foreach (var cat in emptyCategories)
        {
            if (!context.CategoryBrands.Any(cb => cb.CategoryId == cat.Id && cb.BrandId == otherBrand.Id))
                context.CategoryBrands.Add(new CategoryBrand { CategoryId = cat.Id, BrandId = otherBrand.Id });
                
            if (!context.CategoryPartTypes.Any(cpt => cpt.CategoryId == cat.Id && cpt.PartTypeId == otherPartType.Id))
                context.CategoryPartTypes.Add(new CategoryPartType { CategoryId = cat.Id, PartTypeId = otherPartType.Id });
                
            context.DeviceModels.Add(new DeviceModel 
            { 
                CategoryId = cat.Id, 
                BrandId = otherBrand.Id, 
                Name = "Digər (Ümumi)", 
                ModelCode = "OTHER", 
                ReleaseYear = DateTime.Now.Year, 
                IsActive = true 
            });
        }
        context.SaveChanges();
    }

    // Seed 15 sample products if needed
    if (context.SpareParts.Count() < 17)
    {
        var firstSeller = context.SellerProfiles.FirstOrDefault();
        var firstModel = context.DeviceModels.FirstOrDefault();
        var firstPartType = context.SparePartTypes.FirstOrDefault();
        var firstQuality = context.QualityTypes.FirstOrDefault();
        
        Console.WriteLine($"Seeding check: Seller={firstSeller?.Id}, Model={firstModel?.Id}, PartType={firstPartType?.Id}, Quality={firstQuality?.Id}");

        if (firstSeller != null && firstModel != null && firstPartType != null && firstQuality != null)
        {
            var rnd = new Random();
            for (int i = 1; i <= 15; i++)
            {
                context.SpareParts.Add(new EHisseler.Web.Models.SparePart
                {
                    Title = $"Nümunəvi Ehtiyat Hissəsi {i}",
                    Description = $"Bu sistem tərəfindən avtomatik əlavə edilmiş test məhsuludur {i}.",
                    Price = rnd.Next(10, 500) + 0.99m,
                    StockQuantity = rnd.Next(1, 100),
                    DeviceModelId = firstModel.Id,
                    PartTypeId = firstPartType.Id,
                    QualityTypeId = firstQuality.Id,
                    SellerId = firstSeller.Id,
                    ModerationStatus = EHisseler.Web.Models.ApprovalStatus.Approved,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    ViewCount = 0,
                    SKU = $"DUMMY-SKU-{i}"
                });
            }
            context.SaveChanges();
            Console.WriteLine("Seeded 15 dummy products successfully.");
        }
        else
        {
            Console.WriteLine("Could not seed dummy products due to missing foreign key relationships.");
        }
    }

    // Seed images for spare parts if they don't have any
    var sparePartsWithoutImages = context.SpareParts.Include(sp => sp.Images).Where(sp => !sp.Images.Any()).ToList();
    if (sparePartsWithoutImages.Any())
    {
        var random = new Random();
        var availableImages = new[] { "/images/images (1).jpg", "/images/images (2).jpg", "/images/images (3).jpg", "/images/images (4).jpg", "/images/images (5).jpg", "/images/images (6).jpg", "/images/images (7).jpg", "/images/images.jpg" };
        
        foreach (var sp in sparePartsWithoutImages)
        {
            sp.Images.Add(new SparePartImage
            {
                ImageUrl = availableImages[random.Next(availableImages.Length)],
                IsMain = true,
                DisplayOrder = 1
            });
        }
        context.SaveChanges();
    }
}

app.Run("http://0.0.0.0:5000");




