using EHisseler.Web.Models;
using EHisseler.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisseler.Web.Controllers
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
        //  Legacy şəkilləri wwwroot-dan qaytarır (köhnə seed məlumatları)
        // ─────────────────────────────────────────────────────────────
        [HttpGet("/Image/GetImage")]
        public IActionResult GetImage(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return NotFound();

            if (_blobService.IsLegacyPath(path))
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
                if (System.IO.File.Exists(wwwrootPath))
                    return PhysicalFile(wwwrootPath, GetMimeType(wwwrootPath));
            }

            // Blob path-i üçün redirect
            var url = _blobService.GetUrl(path);
            if (string.IsNullOrEmpty(url))
            {
                var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default.png");
                if (System.IO.File.Exists(defaultPath))
                    return PhysicalFile(defaultPath, "image/png");
                return NotFound();
            }

            return Redirect(url);
        }

        // ─────────────────────────────────────────────────────────────
        //  Şəkil yükləmə — Blob Storage-a yükləyir, DB-a path yazır
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

            // Fayl validasiyası
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Fayl seçilməyib." });

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = "Yalnız JPG, PNG və WEBP formatları qəbul edilir." });

            if (file.Length > MaxFileSizeMb * 1024 * 1024)
                return BadRequest(new { message = $"Fayl ölçüsü {MaxFileSizeMb} MB-dan böyük ola bilməz." });

            // Blob Storage-a yüklə
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
                ImageUrl = blobPath,           // ← DB-da yalnız blob path saxlanır
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
