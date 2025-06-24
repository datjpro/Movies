using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesApp.Services;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureVideoController : ControllerBase
    {
        private readonly IVideoTokenService _videoTokenService;
        private readonly ILogger<SecureVideoController> _logger;
        private readonly IConfiguration _configuration;

        public SecureVideoController(
            IVideoTokenService videoTokenService,
            ILogger<SecureVideoController> logger,
            IConfiguration configuration)
        {
            _videoTokenService = videoTokenService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo secure token để truy cập video
        /// </summary>
        [HttpPost("generate-token")]
        [Authorize] // Chỉ user đã login mới có thể tạo token
        public async Task<IActionResult> GenerateVideoToken([FromBody] GenerateTokenRequest request)
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value ?? "anonymous";
                
                // Kiểm tra quyền truy cập video (có thể thêm logic kiểm tra subscription, payment, etc.)
                if (!await CanUserAccessVideo(userId, request.VideoId))
                {
                    return Forbid("Access denied to this video");
                }

                var expiration = TimeSpan.FromHours(request.ExpirationHours ?? 2);
                var token = await _videoTokenService.GenerateVideoTokenAsync(request.VideoId, userId, expiration);

                return Ok(new
                {
                    token,
                    expiresIn = expiration.TotalSeconds,
                    expiresAt = DateTime.UtcNow.Add(expiration),
                    videoId = request.VideoId,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating video token for VideoId: {request.VideoId}");
                return StatusCode(500, new { error = "Internal server error", success = false });
            }
        }

        /// <summary>
        /// Lấy secure URL để streaming video
        /// </summary>
        [HttpGet("video-url/{videoId}")]
        [Authorize]
        public async Task<IActionResult> GetSecureVideoUrl(string videoId, [FromQuery] int? expirationHours = 2)
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value ?? "anonymous";

                if (!await CanUserAccessVideo(userId, videoId))
                {
                    return Forbid("Access denied to this video");
                }

                var expiration = TimeSpan.FromHours(expirationHours ?? 2);
                var secureUrl = await _videoTokenService.CreateSecureVideoUrlAsync(videoId, userId, expiration);

                return Ok(new
                {
                    secureUrl,
                    expiresIn = expiration.TotalSeconds,
                    expiresAt = DateTime.UtcNow.Add(expiration),
                    videoId,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating secure video URL for VideoId: {videoId}");
                return StatusCode(500, new { error = "Internal server error", success = false });
            }
        }

        /// <summary>
        /// Validate token và trả về video stream
        /// </summary>
        [HttpGet("stream/{videoId}")]
        public async Task<IActionResult> StreamSecureVideo(string videoId, [FromQuery] string token)
        {
            try
            {
                // Validate token
                var tokenInfo = await _videoTokenService.ValidateVideoTokenAsync(token);
                if (tokenInfo == null || !tokenInfo.IsValid || tokenInfo.VideoId != videoId)
                {
                    return Unauthorized(new { error = "Invalid or expired token", success = false });
                }

                // Additional security checks
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Optional: Rate limiting per user
                if (!await CheckRateLimit(tokenInfo.UserId, clientIp))
                {
                    return StatusCode(429, new { error = "Rate limit exceeded", success = false });
                }

                // Proxy request to CDN với token validation
                var cdnBaseUrl = _configuration["CDN:BaseUrl"] ?? "http://localhost:5288";
                var cdnUrl = $"{cdnBaseUrl}/api/v1/videos/{videoId}/mp4";

                using var httpClient = new HttpClient();
                
                // Forward headers để maintain streaming capability
                foreach (var header in Request.Headers)
                {
                    if (header.Key.ToLower() == "range")
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.AsEnumerable());
                    }
                }

                var response = await httpClient.GetAsync(cdnUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(new { error = "Video not found", success = false });
                }

                // Log access for security monitoring
                _logger.LogInformation($"Secure video access: VideoId={videoId}, UserId={tokenInfo.UserId}, IP={clientIp}");

                // Return the stream with same headers
                var content = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "video/mp4";
                
                return new FileStreamResult(content, contentType)
                {
                    EnableRangeProcessing = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error streaming secure video: VideoId={videoId}");
                return StatusCode(500, new { error = "Internal server error", success = false });
            }
        }

        /// <summary>
        /// Revoke token (logout hoặc security breach)
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            try
            {
                await _videoTokenService.RevokeTokenAsync(request.Token);
                return Ok(new { message = "Token revoked successfully", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return StatusCode(500, new { error = "Internal server error", success = false });
            }
        }        /// <summary>
        /// Kiểm tra user có quyền truy cập video không
        /// </summary>
        private Task<bool> CanUserAccessVideo(string userId, string videoId)
        {
            // TODO: Implement business logic
            // - Kiểm tra subscription status
            // - Kiểm tra payment
            // - Kiểm tra geo-restriction
            // - Kiểm tra age restriction
            // - etc.

            // Hiện tại cho phép tất cả authenticated users
            return Task.FromResult(!string.IsNullOrEmpty(userId) && userId != "anonymous");
        }

        /// <summary>
        /// Rate limiting để prevent abuse
        /// </summary>
        private Task<bool> CheckRateLimit(string userId, string? clientIp)
        {
            // TODO: Implement rate limiting logic
            // - X requests per minute per user
            // - X requests per minute per IP
            // - X concurrent streams per user

            return Task.FromResult(true); // Temporarily allow all
        }
    }

    public class GenerateTokenRequest
    {
        public string VideoId { get; set; } = string.Empty;
        public int? ExpirationHours { get; set; }
    }

    public class RevokeTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
