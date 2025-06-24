using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace MoviesApp.Middleware
{
    public class VideoSecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<VideoSecurityMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public VideoSecurityMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<VideoSecurityMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ áp dụng cho video requests
            if (IsVideoRequest(context.Request.Path))
            {
                if (!await ValidateVideoRequest(context))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Access denied");
                    return;
                }
            }

            await _next(context);
        }

        private bool IsVideoRequest(PathString path)
        {
            return path.StartsWithSegments("/api/SecureVideo") ||
                   path.StartsWithSegments("/api/v1/videos/secure") ||
                   path.ToString().Contains("/mp4") ||
                   path.ToString().Contains("/hls");
        }        private Task<bool> ValidateVideoRequest(HttpContext context)
        {
            try
            {
                // Rate limiting per IP
                var clientIp = GetClientIpAddress(context);
                var rateLimitKey = $"video_rate_limit_{clientIp}";
                
                if (!_cache.TryGetValue(rateLimitKey, out int requestCount))
                {
                    requestCount = 0;
                }

                requestCount++;
                _cache.Set(rateLimitKey, requestCount, TimeSpan.FromMinutes(1));                // Allow max 30 requests per minute per IP
                if (requestCount > 30)
                {
                    _logger.LogWarning($"Rate limit exceeded for IP: {clientIp}");
                    return Task.FromResult(false);
                }

                // Validate referer
                var referer = context.Request.Headers["Referer"].ToString();
                if (!IsValidReferer(referer))
                {
                    _logger.LogWarning($"Invalid referer: {referer} from IP: {clientIp}");
                    return Task.FromResult(false);
                }

                // Check for suspicious headers                if (HasSuspiciousHeaders(context.Request))
                {
                    _logger.LogWarning($"Suspicious headers detected from IP: {clientIp}");
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating video request");
                return Task.FromResult(false);
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "unknown";
        }

        private bool IsValidReferer(string referer)
        {
            if (string.IsNullOrEmpty(referer))
                return true; // Allow empty referer for direct access

            try
            {
                var refererUri = new Uri(referer);
                var allowedHosts = new[] { "localhost", "127.0.0.1", "ccfilm.com" };
                
                return allowedHosts.Any(host => 
                    refererUri.Host.Equals(host, StringComparison.OrdinalIgnoreCase) ||
                    refererUri.Host.EndsWith($".{host}", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private bool HasSuspiciousHeaders(HttpRequest request)
        {
            var suspiciousHeaders = new[]
            {
                "X-Requested-With",
                "X-Download-Options",
                "X-Force-Download"
            };

            var userAgent = request.Headers["User-Agent"].ToString().ToLower();
            var suspiciousUserAgents = new[]
            {
                "wget",
                "curl",
                "bot",
                "spider",
                "scraper",
                "crawler"
            };

            return suspiciousHeaders.Any(header => request.Headers.ContainsKey(header)) ||
                   suspiciousUserAgents.Any(agent => userAgent.Contains(agent));
        }
    }

    // Extension method để đăng ký middleware
    public static class VideoSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseVideoSecurity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VideoSecurityMiddleware>();
        }
    }
}
