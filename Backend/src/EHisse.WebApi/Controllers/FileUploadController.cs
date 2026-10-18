using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EHisse.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(IStorageService storageService, ILogger<FileUploadController> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    /// <summary>
    /// Şəkil və ya fayl yükləyir və Cloudflare R2 public URL-ini qaytarır.
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

        try
        {
            var fileUrl = await _storageService.UploadFileAsync(file, cancellationToken);
            return Ok(new
            {
                success = true,
                url = fileUrl,
                fileName = file.FileName,
                size = file.Length,
                contentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fayl yüklənərkən xəta baş verdi: {FileName}", file.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Fayl yüklənməsi zamanı server xətası baş verdi.",
                details = ex.Message
            });
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
