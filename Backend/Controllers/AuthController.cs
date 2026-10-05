using EHisseler.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisseler.Web.Controllers
{
    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string FirstName, string LastName, string Phone, string? Email, string Password, UserRole Role);
    public record ForgotPasswordRequest(string Username);
    public record ResetPasswordRequest(string Username, string NewPassword);

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/Auth/Me — cari istifadəçi (frontend header üçün)
        [HttpGet]
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

        // POST api/Auth/Login
        [HttpPost]
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
        [HttpPost]
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

            return Ok(new { success = true, message = "Qeydiyyat qəbul edildi. Admin təsdiqindən sonra daxil ola bilərsiniz." });
        }

        // POST api/Auth/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }

        // POST api/Auth/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var username = req.Username?.Trim() ?? string.Empty;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user == null)
                return NotFound(new { message = "Belə bir istifadəçi tapılmadı." });

            // Real tətbiqdə burada token yaradılıb email/SMS göndərilməlidir.
            return Ok(new { success = true, username = user.Username });
        }

        // POST api/Auth/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 6)
                return BadRequest(new { message = "Şifrə minimum 6 simvol olmalıdır." });

            var username = req.Username?.Trim() ?? string.Empty;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user == null)
                return NotFound(new { message = "Xəta baş verdi." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
