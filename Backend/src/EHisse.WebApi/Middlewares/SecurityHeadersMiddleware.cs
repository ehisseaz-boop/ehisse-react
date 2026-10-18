namespace EHisse.WebApi.Middlewares;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Clickjacking hücumlarından qorunma (Saytın gizli iframe içində açılmasının qarşısını alır)
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // 2. MIME sniffing hücumlarından qorunma (Brauzerin fayl tipini təhlükəli şəkildə dəyişməsinə icazə vermir)
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // 3. XSS hücumlarına qarşı köhnə brauzer filtri
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // 4. Referrer sızmasının qarşısını alır
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // 5. Brauzer xüsusiyyətlərini (kamera, mikrofon və s.) icazəsiz saytlara bağlayır
        context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

        // 6. Server texnologiyası haqqında məlumat verən başlıqları təmizləyir
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        await _next(context);
    }
}
