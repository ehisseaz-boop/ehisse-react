using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EHisse.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IBlobStorageService _blobService;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(
        IStorageService storageService,
        IBlobStorageService blobService,
        ILogger<FileUploadController> logger)
    {
        _storageService = storageService;
        _blobService = blobService;
        _logger = logger;
    }

    /// <summary>
    /// Şəkil və ya fayl yükləyir və public URL-ini qaytarır.
    /// Cloudflare R2 əlçatan olmadıqda avtomatik olaraq etibarlı yerli/blob storage fallback istifadə olunur.
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Fayl seçilməyib və ya boşdur." });
        }

        // Təhlükəsizlik üçün ölçü yoxlanışı (məsələn, max 10MB)
        const long maxSizeBytes = 10 * 1024 * 1024;
        if (file.Length > maxSizeBytes)
        {
            return BadRequest(new { message = "Faylın həcmi 10 MB-dan çox ola bilməz." });
        }

        // Kiber hücum qorunması: Yalnız təsdiqlənmiş təhlükəsiz şəkil formatlarına icazə verilir (.exe, .php, .sh qadağandır)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Yalnız JPG, JPEG, PNG və ya WEBP formatında şəkillər yükləyə bilərsiniz." });
        }

        // 1. Əvvəlcə Cloudflare R2-yə yükləməyə cəhd edirik (əgər konfiqurasiya edilibsə)
        try
        {
            var fileUrl = await _storageService.UploadFileAsync(file, cancellationToken);
            if (!string.IsNullOrWhiteSpace(fileUrl))
            {
                return Ok(new
                {
                    success = true,
                    url = fileUrl,
                    fileName = file.FileName,
                    size = file.Length,
                    contentType = file.ContentType
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cloudflare R2-yə yüklənmə uğursuz oldu. Yerli/Blob yaddaşa keçilir.");
        }

        // 2. R2 konfiqurasiya olunmayıbsa və ya SSL/şəbəkə xətası verərsə, BlobStorageService (uploads/idcards) fallback işləyir
        try
        {
            var savedPath = await _blobService.UploadAsync(file, "idcards");
            var resolvedUrl = _blobService.GetUrl(savedPath) ?? savedPath;
            if (!resolvedUrl.StartsWith("http") && !resolvedUrl.StartsWith("/"))
            {
                resolvedUrl = "/" + resolvedUrl;
            }

            return Ok(new
            {
                success = true,
                url = resolvedUrl,
                fileName = file.FileName,
                size = file.Length,
                contentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fayl yaddaşa yazılarkən xəta: {FileName}. DataURL formatında saxlanılır.", file.FileName);
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms, cancellationToken);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mime = string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType;
                var dataUri = $"data:{mime};base64,{base64}";
                return Ok(new
                {
                    success = true,
                    url = dataUri,
                    fileName = file.FileName,
                    size = file.Length,
                    contentType = file.ContentType
                });
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "DataURI fallback də uğursuz oldu: {FileName}", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Fayl yüklənməsi zamanı server xətası baş verdi.",
                    details = ex.Message
                });
            }
        }
    }

    /// <summary>
    /// Cloudflare R2-dən faylı silir.
    /// </summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] string fileUrlOrKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileUrlOrKey))
        {
            return BadRequest(new { message = "Fayl URL-i və ya açarı qeyd edilməyib." });
        }

        var isDeleted = await _storageService.DeleteFileAsync(fileUrlOrKey, cancellationToken);
        if (isDeleted)
        {
            return Ok(new { success = true, message = "Fayl uğurla silindi." });
        }

        return BadRequest(new { success = false, message = "Fayl silinə bilmədi və ya tapılmadı." });
    }
}
