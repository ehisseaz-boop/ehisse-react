using EHisse.WebApi.Services;
using EHisse.Application.Interfaces;
using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Application.DTOs;
using EHisse.Application.Common;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string FirstName, string LastName, string Phone, string? Email, string Password, UserRole Role);
    public record ForgotPasswordRequest(string Username);
    public record VerifyResetOtpRequest(string Username, string Code);
    public record ResetPasswordRequest(string Username, string Code, string NewPassword);

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public AuthController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET api/Auth/Me â€” cari istifadÉ™Ã§i (frontend header Ã¼Ã§Ã¼n)
        [HttpGet("[action]")]
        public IActionResult Me()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Ok(new { isAuthenticated = false });

            return Ok(new
            {
                isAuthenticated = true,
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                username = User.FindFirstValue(ClaimTypes.Name),
                fullName = User.FindFirstValue(ClaimTypes.GivenName),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpGet("[action]")]
        public IActionResult Operators()
        {
            return Ok(PhoneNumberHelper.Operators.Select(o => new { code = o.Code, name = o.Name }));
        }

        // POST api/Auth/Login
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var username = req.Username?.Trim() ?? string.Empty;
            var password = req.Password ?? string.Empty;

            // Allow login by Username, Email, or Phone
            string normalizedPhone = username;
            if (PhoneNumberHelper.TryNormalizeAzNumber(username, out string _, out string nTel))
            {
                normalizedPhone = nTel;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower() ||
                (u.Email != null && u.Email.ToLower() == username.ToLower()) ||
                u.Phone == normalizedPhone);

            if (user == null)
                return BadRequest(new { message = "MÉ™lumatlar yanlÄ±ÅŸdÄ±r." });

            // Geri uyÄŸunluq (legacy plain text passwords) Ã¼Ã§Ã¼n
            bool isPasswordValid = false;
            if (!user.PasswordHash.StartsWith("$2"))
            {
                if (user.PasswordHash == password)
                {
                    isPasswordValid = true;
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }

            if (!isPasswordValid)
                return BadRequest(new { message = "Ä°stifadÉ™Ã§i adÄ± vÉ™ ya ÅŸifrÉ™ yanlÄ±ÅŸdÄ±r." });

            if (user.IsBlocked)
            {
                var blockLog = await _context.ActivityLogs
                    .Where(l => l.EntityName == "User" && l.EntityId == user.Id && l.Action == "BlockUser")
                    .OrderByDescending(l => l.Date)
                    .FirstOrDefaultAsync();
                var reasonText = blockLog != null ? blockLog.Details : "ZÉ™hmÉ™t olmasa adminlÉ™ É™laqÉ™ saxlayÄ±n.";
                return BadRequest(new { message = $"HesabÄ±nÄ±z bloklanÄ±b. {reasonText}" });
            }

            if (user.Role != UserRole.Admin && user.ApprovalStatus == ApprovalStatus.Pending)
                return BadRequest(new { message = "HesabÄ±nÄ±z yoxlamadadÄ±r. Admin tÉ™sdiqlÉ™dikdÉ™n sonra daxil ola bilÉ™rsiniz." });

            if (user.Role != UserRole.Admin && user.ApprovalStatus == ApprovalStatus.Rejected)
            {
                var history = await _context.ApprovalHistories
                    .Where(h => h.TargetUserId == user.Id && h.Status == ApprovalStatus.Rejected)
                    .OrderByDescending(h => h.ActionDate)
                    .FirstOrDefaultAsync();
                var reason = user.RejectionReason ?? history?.Reason ?? "QeydiyyatÄ±nÄ±z admin tÉ™rÉ™findÉ™n rÉ™dd edilib.";
                return BadRequest(new { message = $"QeydiyyatÄ±nÄ±z rÉ™dd edilib. SÉ™bÉ™b: {reason}" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new
            {
                success = true,
                username = user.Username,
                fullName = $"{user.FirstName} {user.LastName}",
                role = user.Role.ToString()
            });
        }

        // POST api/Auth/Register
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName) ||
                string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                return BadRequest(new { message = "Ad, Soyad, Mobil nÃ¶mrÉ™ vÉ™ ÅifrÉ™ mÉ™cburidir. ÅifrÉ™ minimum 6 simvol olmalÄ±dÄ±r." });

            string? normalizedTel = null;
            if (!PhoneNumberHelper.TryNormalizeAzNumber(req.Phone, out string _, out string tel))
                return BadRequest(new { message = "Mobil nÃ¶mrÉ™ dÃ¼zgÃ¼n formatda deyil." });
            normalizedTel = tel;

            var email = req.Email?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(email) && await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower()))
                return BadRequest(new { message = "Bu email Ã¼nvanÄ± artÄ±q mÃ¶vcuddur." });

            if (await _context.Users.AnyAsync(u => u.Phone == normalizedTel))
                return BadRequest(new { message = "Bu nÃ¶mrÉ™ artÄ±q mÃ¶vcuddur." });

            if (req.Role == UserRole.Admin)
                return BadRequest(new { message = "Admin kimi qeydiyyatdan keÃ§mÉ™k mÃ¼mkÃ¼n deyil." });

            string generatedUsername = normalizedTel;

            _context.Users.Add(new User
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Phone = normalizedTel,
                Email = email,
                Username = generatedUsername,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = req.Role,
                ApprovalStatus = ApprovalStatus.Pending,
                CreatedDate = System.DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Qeydiyyat mÉ™lumatlarÄ±nÄ± vÉ™ ÅŸifrÉ™ni istifadÉ™Ã§inin poÃ§tuna gÃ¶ndÉ™ririk
            if (!string.IsNullOrWhiteSpace(email))
            {
                var roleName = req.Role == UserRole.Anbardar ? "Anbardar (SatÄ±cÄ±)" : "Usta (AlÄ±cÄ±)";
                var emailBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; background-color: #ffffff;"">
                        <div style=""text-align: center; margin-bottom: 20px;"">
                            <h2 style=""color: #2b3a4a; margin: 0;"">E-Hisse.az PlatformasÄ±na XoÅŸ GÉ™lmisiniz!</h2>
                        </div>
                        <p style=""font-size: 15px; color: #333;"">HÃ¶rmÉ™tli <b>{req.FirstName} {req.LastName}</b>,</p>
                        <p style=""font-size: 14px; color: #555; line-height: 1.6;"">
                            Qeydiyyat mÃ¼raciÉ™tiniz qeydÉ™ alÄ±ndÄ±. HesabÄ±nÄ±z admin tÉ™rÉ™findÉ™n tÉ™sdiqlÉ™ndikdÉ™n sonra platformaya daxil ola bilÉ™rsiniz.
                        </p>
                        <div style=""background-color: #f7f9fa; border-left: 4px solid #f2994a; padding: 15px; margin: 20px 0; border-radius: 4px;"">
                            <h4 style=""margin: 0 0 10px 0; color: #333;"">GiriÅŸ MÉ™lumatlarÄ±nÄ±z:</h4>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>Ä°stifadÉ™Ã§i adÄ± (Mobil nÃ¶mrÉ™):</b> {normalizedTel}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>E-poÃ§t Ã¼nvanÄ±:</b> {email}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>ÅifrÉ™niz:</b> {req.Password}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>Rolunuz:</b> {roleName}</p>
                        </div>
                        <p style=""font-size: 13px; color: #888;"">TÉ™hlÃ¼kÉ™sizliyiniz Ã¼Ã§Ã¼n ÅŸifrÉ™nizi heÃ§ kimlÉ™ paylaÅŸmayÄ±n.</p>
                        <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"" />
                        <p style=""font-size: 12px; color: #aaa; text-align: center;"">Â© 2026 E-Hisse.az. BÃ¼tÃ¼n hÃ¼quqlar qorunur.</p>
                    </div>";

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendEmailAsync(email, "E-Hisse.az â€” Qeydiyyat vÉ™ GiriÅŸ MÉ™lumatlarÄ±", emailBody);
                    }
                    catch { }
                });
            }

            return Ok(new { success = true, message = "Qeydiyyat qÉ™bul edildi. GiriÅŸ mÉ™lumatlarÄ±nÄ±z vÉ™ ÅŸifrÉ™niz e-poÃ§t Ã¼nvanÄ±nÄ±za gÃ¶ndÉ™rildi!" });
        }

        // POST api/Auth/Logout
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }

        // POST api/Auth/ForgotPassword
        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var username = req.Username?.Trim() ?? string.Empty;
            string normalizedPhone = username;
            if (PhoneNumberHelper.TryNormalizeAzNumber(username, out string _, out string nTel))
            {
                normalizedPhone = nTel;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower() ||
                (u.Email != null && u.Email.ToLower() == username.ToLower()) ||
                u.Phone == normalizedPhone);

            if (user == null)
                return NotFound(new { message = "Belə bir istifadəçi tapılmadı." });

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest(new { message = "Hesabınıza bağlı heç bir e-poçt ünvanı tapılmadı. Zəhmət olmasa adminlə əlaqə saxlayın." });

            // 6 rəqəmli OTP kod generasiyası
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            OtpStore.SetCode(user.Username, code, TimeSpan.FromMinutes(10));

            var emailBody = $@"
                <div style=""font-family: Arial, sans-serif; max-width: 550px; margin: 0 auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px; background-color: #ffffff;"">
                    <div style=""text-align: center; margin-bottom: 25px;"">
                        <h2 style=""color: #2b3a4a; margin: 0;"">E-Hisse.az — Şifrə Bərpası</h2>
                    </div>
                    <p style=""font-size: 15px; color: #333;"">Hörmətli <b>{user.FirstName} {user.LastName}</b>,</p>
                    <p style=""font-size: 14px; color: #555; line-height: 1.6;"">
                        Hesabınızın şifrəsini bərpa etmək üçün təsdiq kodunuz aşağıdadır:
                    </p>
                    <div style=""text-align: center; margin: 30px 0;"">
                        <span style=""display: inline-block; font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #2f80ed; background: #f0f7ff; padding: 12px 30px; border-radius: 8px; border: 1px dashed #2f80ed;"">
                            {code}
                        </span>
                    </div>
                    <p style=""font-size: 13px; color: #777; line-height: 1.5;"">
                        Bu kod <b>10 dəqiqə</b> ərzində etibarlıdır. Əgər bu müraciəti siz etməmisinizsə, zəhmət olmasa bu məktubu nəzərə almayın.
                    </p>
                    <hr style=""border: none; border-top: 1px solid #eee; margin: 25px 0;"" />
                    <p style=""font-size: 12px; color: #aaa; text-align: center;"">© 2026 E-Hisse.az. Bütün hüquqlar qorunur.</p>
                </div>";

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendEmailAsync(user.Email, $"{code} — E-Hisse.az Şifrə Bərpa Kodu", emailBody);
                }
                catch { }
            });

            // Maskalanmış email göstəririk: m***i@gmail.com
            string maskedEmail = user.Email;
            var parts = user.Email.Split('@');
            if (parts.Length == 2 && parts[0].Length > 2)
            {
                maskedEmail = parts[0][0] + new string('*', parts[0].Length - 2) + parts[0][^1] + "@" + parts[1];
            }

            return Ok(new { success = true, username = user.Username, email = maskedEmail });
        }

        // POST api/Auth/VerifyResetOtp
        [HttpPost("[action]")]
        public IActionResult VerifyResetOtp([FromBody] VerifyResetOtpRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Code) || req.Code.Length != 6)
                return BadRequest(new { message = "6 rəqəmli təsdiq kodunu daxil edin." });

            if (!OtpStore.ValidateCode(req.Username, req.Code))
                return BadRequest(new { message = "Daxil edilmiş kod yanlışdır və ya vaxtı bitib." });

            // Yenidən şifrə dəyişmə zamanı yoxlamaq üçün 10 dəqiqəlik icazə tokeni qoyuruq
            OtpStore.SetCode("RESET_PERMIT_" + req.Username, "PERMITTED", TimeSpan.FromMinutes(10));
            return Ok(new { success = true });
        }

        // POST api/Auth/ResetPassword
        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 6)
                return BadRequest(new { message = "Şifrə minimum 6 simvol olmalıdır." });

            var username = req.Username?.Trim() ?? string.Empty;

            // Əgər kod vasitəsilə təsdiqlənməyibsə icazə vermirik
            if (!OtpStore.ValidateCode("RESET_PERMIT_" + username, "PERMITTED"))
            {
                // Əgər birbaşa req.Code ilə göndəribsə
                if (!OtpStore.ValidateCode(username, req.Code))
                    return BadRequest(new { message = "Təsdiq kodu yanlışdır və ya vaxtı bitib. Zəhmət olmasa yenidən cəhd edin." });
            }

            string normalizedPhone = username;
            if (PhoneNumberHelper.TryNormalizeAzNumber(username, out string _, out string nTel))
            {
                normalizedPhone = nTel;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower() ||
                (u.Email != null && u.Email.ToLower() == username.ToLower()) ||
                u.Phone == normalizedPhone);

            if (user == null)
                return NotFound(new { message = "Xəta baş verdi. İstifadəçi tapılmadı." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var emailBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 550px; margin: 0 auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px; background-color: #ffffff;"">
                        <div style=""text-align: center; margin-bottom: 25px;"">
                            <h2 style=""color: #2b3a4a; margin: 0;"">Şifrəniz Yeniləndi</h2>
                        </div>
                        <p style=""font-size: 15px; color: #333;"">Hörmətli <b>{user.FirstName} {user.LastName}</b>,</p>
                        <p style=""font-size: 14px; color: #555; line-height: 1.6;"">
                            Hesabınızın şifrəsi uğurla dəyişdirildi. İndi yeni şifrənizlə daxil ola bilərsiniz.
                        </p>
                        <hr style=""border: none; border-top: 1px solid #eee; margin: 25px 0;"" />
                        <p style=""font-size: 12px; color: #aaa; text-align: center;"">© 2026 E-Hisse.az. Bütün hüquqlar qorunur.</p>
                    </div>";

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendEmailAsync(user.Email, "E-Hisse.az — Şifrəniz Yeniləndi", emailBody);
                    }
                    catch { }
                });
            }

            return Ok(new { success = true, message = "Şifrəniz uğurla yeniləndi!" });
        }
    }
}

