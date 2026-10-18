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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string FirstName, string LastName, string Phone, string? Email, string Password, UserRole Role, string? Code);
    public record SendRegisterOtpRequest(string Email);
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

        // GET api/Auth/Me — cari istifadəçi (frontend header üçün)
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
                role = User.FindFirstValue(ClaimTypes.Role),
                email = User.FindFirstValue(ClaimTypes.Email)
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
                return BadRequest(new { message = "Məlumatlar yanlışdır." });

            // Geri uyğunluq (legacy plain text passwords) üçün
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
                return BadRequest(new { message = "İstifadəçi adı və ya şifrə yanlışdır." });

            if (user.IsBlocked)
            {
                var blockLog = await _context.ActivityLogs
                    .Where(l => l.EntityName == "User" && l.EntityId == user.Id && l.Action == "BlockUser")
                    .OrderByDescending(l => l.Date)
                    .FirstOrDefaultAsync();
                var reasonText = blockLog != null ? blockLog.Details : "Zəhmət olmasa adminlə əlaqə saxlayın.";
                return BadRequest(new { message = $"Hesabınız bloklanıb. {reasonText}" });
            }

            if (user.Role != UserRole.Admin && user.ApprovalStatus == ApprovalStatus.Pending)
                return BadRequest(new { message = "Hesabınız yoxlamadadır. Admin təsdiqlədikdən sonra daxil ola bilərsiniz." });

            if (user.Role != UserRole.Admin && user.ApprovalStatus == ApprovalStatus.Rejected)
            {
                var history = await _context.ApprovalHistories
                    .Where(h => h.TargetUserId == user.Id && h.Status == ApprovalStatus.Rejected)
                    .OrderByDescending(h => h.ActionDate)
                    .FirstOrDefaultAsync();
                var reason = user.RejectionReason ?? history?.Reason ?? "Qeydiyyatınız admin tərəfindən rədd edilib.";
                return BadRequest(new { message = $"Qeydiyyatınız rədd edilib. Səbəb: {reason}" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new
            {
                success = true,
                username = user.Username,
                fullName = $"{user.FirstName} {user.LastName}",
                role = user.Role.ToString(),
                email = user.Email
            });
        }

        // POST api/Auth/SendRegisterOtp
        [HttpPost("[action]")]
        public async Task<IActionResult> SendRegisterOtp([FromBody] SendRegisterOtpRequest req)
        {
            var email = req.Email?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "E-poçt ünvanı mütləqdir." });

            if (await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower()))
                return BadRequest(new { message = "Bu email ünvanı artıq qeydiyyatdan keçib." });

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();
            OtpStore.SetCode("REG_" + email, code, TimeSpan.FromMinutes(10));

            var emailBody = $@"
                <div style=""font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 8px; background-color: #ffffff;"">
                    <div style=""text-align: center; margin-bottom: 20px;"">
                        <h2 style=""color: #2b3a4a; margin: 0;"">E-Hisse.az — Qeydiyyat Təsdiqi</h2>
                    </div>
                    <p style=""font-size: 15px; color: #333;"">Salam,</p>
                    <p style=""font-size: 14px; color: #555; line-height: 1.6;"">
                        E-Hisse platformasında qeydiyyatı tamamlamaq üçün təsdiq kodunuz:
                    </p>
                    <div style=""text-align: center; margin: 25px 0;"">
                        <span style=""display: inline-block; font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #27ae60; background: #f0fff4; padding: 12px 30px; border-radius: 8px; border: 1px dashed #27ae60;"">
                            {code}
                        </span>
                    </div>
                    <p style=""font-size: 13px; color: #777;"">Kod 10 dəqiqə ərzində etibarlıdır.</p>
                    <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"" />
                    <p style=""font-size: 12px; color: #aaa; text-align: center;"">© 2026 E-Hisse.az</p>
                </div>";

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.SendEmailAsync(email, $"{code} — E-Hisse Qeydiyyat Təsdiq Kodu", emailBody);
                }
                catch { }
            });

            return Ok(new { success = true, message = "Təsdiq kodu email ünvanınıza göndərildi!" });
        }

        // POST api/Auth/Register
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName) ||
                string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                return BadRequest(new { message = "Ad, Soyad, Mobil nömrə və Şifrə məcburidir. Şifrə minimum 6 simvol olmalıdır." });

            string? normalizedTel = null;
            if (!PhoneNumberHelper.TryNormalizeAzNumber(req.Phone, out string _, out string tel))
                return BadRequest(new { message = "Mobil nömrə düzgün formatda deyil." });
            normalizedTel = tel;

            var email = req.Email?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(email) && await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower()))
                return BadRequest(new { message = "Bu email ünvanı artıq mövcuddur." });

            if (await _context.Users.AnyAsync(u => u.Phone == normalizedTel))
                return BadRequest(new { message = "Bu nömrə artıq mövcuddur." });

            if (req.Role == UserRole.Admin)
                return BadRequest(new { message = "Admin kimi qeydiyyatdan keçmək mümkün deyil." });

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(req.Code))
            {
                if (!OtpStore.ValidateCode("REG_" + email, req.Code))
                    return BadRequest(new { message = "Emailə göndərilən təsdiq kodu yanlışdır və ya vaxtı bitib." });
            }

            string generatedUsername = normalizedTel;

            var newUser = new User
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Phone = normalizedTel,
                Email = email,
                Username = generatedUsername,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = req.Role,
                ApprovalStatus = ApprovalStatus.Approved,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Satıcı profilini birbaşa yaradırıq ki, elan yerləşdirə bilsin
            if (!await _context.SellerProfiles.AnyAsync(s => s.UserId == newUser.Id))
            {
                var sellerProfile = new SellerProfile
                {
                    UserId = newUser.Id,
                    CompanyName = $"{newUser.FirstName} {newUser.LastName} ({newUser.Role})",
                    ContactPerson = $"{newUser.FirstName} {newUser.LastName}",
                    Phone = newUser.Phone,
                    WhatsAppNumber = newUser.Phone,
                    Rating = 5.0m,
                    ShowContactInfoInAds = true,
                    IsPhoneVerified = true
                };
                _context.SellerProfiles.Add(sellerProfile);
                await _context.SaveChangesAsync();
            }

            // Avtomatik daxil edirik (Auto-login)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Username),
                new Claim(ClaimTypes.GivenName, $"{newUser.FirstName} {newUser.LastName}"),
                new Claim(ClaimTypes.Role, newUser.Role.ToString()),
                new Claim(ClaimTypes.Email, newUser.Email ?? string.Empty)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Qeydiyyat məlumatlarını və şifrəni istifadəçinin poçtuna göndəririk
            if (!string.IsNullOrWhiteSpace(email))
            {
                var roleName = req.Role == UserRole.Anbardar ? "Anbardar (Satıcı)" : "Usta (Alıcı)";
                var emailBody = $@"
                    <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; background-color: #ffffff;"">
                        <div style=""text-align: center; margin-bottom: 20px;"">
                            <h2 style=""color: #2b3a4a; margin: 0;"">E-Hisse.az Platformasına Xoş Gəlmisiniz!</h2>
                        </div>
                        <p style=""font-size: 15px; color: #333;"">Hörmətli <b>{req.FirstName} {req.LastName}</b>,</p>
                        <p style=""font-size: 14px; color: #555; line-height: 1.6;"">
                            Qeydiyyatınız uğurla tamamlandı. Platformaya daxil oldunuz və dərhal elanlarınızı yerləşdirə bilərsiniz.
                        </p>
                        <div style=""background-color: #f7f9fa; border-left: 4px solid #27ae60; padding: 15px; margin: 20px 0; border-radius: 4px;"">
                            <h4 style=""margin: 0 0 10px 0; color: #333;"">Giriş Məlumatlarınız:</h4>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>İstifadəçi adı (Mobil nömrə):</b> {normalizedTel}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>E-poçt ünvanı:</b> {email}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>Şifrəniz:</b> {req.Password}</p>
                            <p style=""margin: 5px 0; font-size: 14px;""><b>Rolunuz:</b> {roleName}</p>
                        </div>
                        <p style=""font-size: 13px; color: #888;"">Təhlükəsizliyiniz üçün şifrənizi heç kimlə paylaşmayın.</p>
                        <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"" />
                        <p style=""font-size: 12px; color: #aaa; text-align: center;"">© 2026 E-Hisse.az. Bütün hüquqlar qorunur.</p>
                    </div>";

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendEmailAsync(email, "E-Hisse.az — Qeydiyyat və Giriş Məlumatları", emailBody);
                    }
                    catch { }
                });
            }

            return Ok(new
            {
                success = true,
                message = "Qeydiyyatınız uğurla tamamlandı!",
                username = newUser.Username,
                fullName = $"{newUser.FirstName} {newUser.LastName}",
                role = newUser.Role.ToString(),
                email = newUser.Email
            });
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
