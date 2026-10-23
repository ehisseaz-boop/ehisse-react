using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Application.DTOs;
using EHisse.Application.Common;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IBlobStorageService _blobService;
        private readonly AppDbContext _context;

        // Yüklənə bilən fayl növləri
        private static readonly HashSet<string> AllowedExtensions =
            new(System.StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

        private const int MaxFileSizeMb = 5;

        public ImageController(IBlobStorageService blobService, AppDbContext context)
        {
            _blobService = blobService;
            _context = context;
        }

        // ─────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────
        //  Faylları wwwroot-dan və ya Blob-dan qaytarır
        // ─────────────────────────────────────────────────────────────
        [HttpGet("/Image/GetImage")]
        public IActionResult GetImage(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return NotFound();

            // Brauzer və CDN kəşləməsi (şəkillərin hər dəfə təkrar yüklənməməsi və dərhal açılması üçün)
            Response.Headers.Append("Cache-Control", "public, max-age=86400, stale-while-revalidate=604800");

            // Əgər fayl wwwroot daxilində fiziki olaraq varsa (legacy şəkillər və ya uploads/...)
            var cleanPath = path.TrimStart('/').Replace('\\', '/');
            if (!cleanPath.Contains(".."))
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanPath.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(wwwrootPath))
                    return PhysicalFile(wwwrootPath, GetMimeType(wwwrootPath));
            }

            // Blob path-i üçün redirect
            var url = _blobService.GetUrl(path);
            if (string.IsNullOrEmpty(url) || url.StartsWith("/"))
            {
                var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default.png");
                if (System.IO.File.Exists(defaultPath))
                    return PhysicalFile(defaultPath, "image/png");

                // Şəkil tapılmadıqda konsolu 404 ilə doldurmamaq üçün standart fallback SVG qaytarırıq
                var svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"200\" height=\"200\" viewBox=\"0 0 200 200\"><rect width=\"100%\" height=\"100%\" fill=\"#1e293b\"/><text x=\"50%\" y=\"50%\" dominant-baseline=\"middle\" text-anchor=\"middle\" fill=\"#94a3b8\" font-family=\"sans-serif\" font-size=\"14\">Şəkil yoxdur</text></svg>";
                return Content(svg, "image/svg+xml");
            }

            return Redirect(url);
        }

        // ─────────────────────────────────────────────────────────────
        //  Şəkil yükləmə — Blob Storage-a (və ya yerli diskə) yükləyir, DB-a path yazır
        //  POST api/Image/Upload  (multipart/form-data: sparePartId, file, isMain)
        // ─────────────────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "Usta,Anbardar")]
        [RequestSizeLimit(MaxFileSizeMb * 1024 * 1024 + 64 * 1024)]
        public async Task<IActionResult> Upload([FromForm] int sparePartId, IFormFile? file, [FromForm] bool isMain = false)
        {
            // Məhsulun bu istifadəçiyə aid olduğunu yoxla
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sellerProfile == null) return StatusCode(403, new { message = "Satıcı profili tapılmadı." });

            var part = await _context.SpareParts
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == sparePartId && p.SellerId == sellerProfile.Id && !p.IsDeleted);

            if (part == null) return NotFound(new { message = "Elan tapılmadı." });

            // Bir elana maksimum 10 şəkil limiti (Server və baza qoruması)
            const int MaxImagesPerPart = 10;
            if (part.Images.Count >= MaxImagesPerPart)
                return BadRequest(new { message = $"Bir elana maksimum {MaxImagesPerPart} şəkil əlavə edilə bilər." });

            // Fayl validasiyası
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Fayl seçilməyib." });

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = "Yalnız JPG, PNG və WEBP formatları qəbul edilir." });

            if (file.Length > MaxFileSizeMb * 1024 * 1024)
                return BadRequest(new { message = $"Fayl ölçüsü {MaxFileSizeMb} MB-dan böyük ola bilməz." });

            try
            {
                // Blob Storage-a (və ya yerli diskə) yüklə
                var folder = $"parts/{sparePartId}";
                var blobPath = await _blobService.UploadAsync(file, folder);

                // Əgər ilk şəkil və ya isMain=true, köhnəsini main-dən çıxar
                var makeMain = isMain || !part.Images.Any();
                if (makeMain)
                {
                    foreach (var img in part.Images)
                        img.IsMain = false;
                }

                var image = new SparePartImage
                {
                    SparePartId = sparePartId,
                    ImageUrl = blobPath,           // ← DB-da yalnız path saxlanır
                    IsMain = makeMain,
                    DisplayOrder = part.Images.Count + 1
                };

                _context.SparePartImages.Add(image);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Şəkil uğurla yükləndi.",
                    image = new { id = image.Id, isMain = image.IsMain, url = _blobService.GetUrl(blobPath) ?? blobPath }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Şəkil saxlanılarkən xəta baş verdi: " + ex.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  Şəkil silmə — Blob-dan ve DB-dan silir
        //  DELETE api/Image/Delete/5
        // ─────────────────────────────────────────────────────────────
        [HttpDelete("{imageId:int}")]
        [Authorize(Roles = "Usta,Anbardar")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (sellerProfile == null) return StatusCode(403, new { message = "Satıcı profili tapılmadı." });

            var image = await _context.SparePartImages
                .Include(i => i.SparePart)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.SparePart.SellerId == sellerProfile.Id);

            if (image == null) return NotFound(new { message = "Şəkil tapılmadı." });

            var sparePartId = image.SparePartId;
            var wasMain = image.IsMain;

            await _blobService.DeleteAsync(image.ImageUrl);
            _context.SparePartImages.Remove(image);
            await _context.SaveChangesAsync();

            // Silindikdən sonra başqa şəkili main et
            if (wasMain)
            {
                var next = await _context.SparePartImages
                    .Where(i => i.SparePartId == sparePartId)
                    .OrderBy(i => i.DisplayOrder)
                    .FirstOrDefaultAsync();
                if (next != null)
                {
                    next.IsMain = true;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { success = true, message = "Şəkil silindi." });
        }

        // ─────────────────────────────────────────────────────────────
        //  Köməkçi metodlar
        // ─────────────────────────────────────────────────────────────
        private static string GetMimeType(string file)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            return ext switch
            {
                ".png"            => "image/png",
                ".gif"            => "image/gif",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp"           => "image/webp",
                _                 => "application/octet-stream",
            };
        }
    }
}
