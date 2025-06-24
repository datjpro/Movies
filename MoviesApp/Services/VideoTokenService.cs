using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MoviesApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MoviesApp.Services
{
    public class VideoTokenService : IVideoTokenService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VideoTokenService> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public VideoTokenService(
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<VideoTokenService> logger)
        {
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _tokenHandler = new JwtSecurityTokenHandler();
        }        public Task<string> GenerateVideoTokenAsync(string videoId, string userId, TimeSpan expiration)
        {
            try
            {
                var tokenInfo = new VideoTokenInfo
                {
                    VideoId = videoId,
                    UserId = userId,
                    ExpiresAt = DateTime.UtcNow.Add(expiration)
                };

                // Tạo JWT token cho video access
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration["JwtSettings:SecretKey"] ?? "CCFilm-Video-Security-Key-2025-SuperSecret"));
                
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("video_id", videoId),
                    new Claim("user_id", userId),
                    new Claim("issued_at", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("token_type", "video_access")
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = tokenInfo.ExpiresAt,
                    SigningCredentials = credentials,
                    Issuer = _configuration["JwtSettings:Issuer"] ?? "CCFilm",
                    Audience = _configuration["JwtSettings:Audience"] ?? "CCFilm-Users"
                };

                var token = _tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = _tokenHandler.WriteToken(token);

                // Cache token info để validation nhanh hơn
                var cacheKey = $"video_token_{tokenString.GetHashCode()}";
                _cache.Set(cacheKey, tokenInfo, expiration);                _logger.LogInformation($"Generated video token for VideoId: {videoId}, UserId: {userId}, Expires: {tokenInfo.ExpiresAt}");

                return Task.FromResult(tokenString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating video token for VideoId: {videoId}, UserId: {userId}");
                throw;
            }
        }        public Task<VideoTokenInfo?> ValidateVideoTokenAsync(string token)
        {
            try
            {                if (string.IsNullOrEmpty(token))
                    return Task.FromResult<VideoTokenInfo?>(null);

                // Kiểm tra cache trước
                var cacheKey = $"video_token_{token.GetHashCode()}";                if (_cache.TryGetValue(cacheKey, out VideoTokenInfo? cachedInfo))
                {
                    if (cachedInfo != null && cachedInfo.IsValid)
                        return Task.FromResult<VideoTokenInfo?>(cachedInfo);
                    
                    // Token đã hết hạn, xóa khỏi cache
                    _cache.Remove(cacheKey);
                    return Task.FromResult<VideoTokenInfo?>(null);
                }

                // Validate JWT token
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration["JwtSettings:SecretKey"] ?? "CCFilm-Video-Security-Key-2025-SuperSecret"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"] ?? "CCFilm",
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"] ?? "CCFilm-Users",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var videoId = jwtToken.Claims.FirstOrDefault(x => x.Type == "video_id")?.Value;
                    var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
                    var tokenType = jwtToken.Claims.FirstOrDefault(x => x.Type == "token_type")?.Value;                    if (tokenType != "video_access" || string.IsNullOrEmpty(videoId) || string.IsNullOrEmpty(userId))
                        return Task.FromResult<VideoTokenInfo?>(null);

                    var tokenInfo = new VideoTokenInfo
                    {
                        VideoId = videoId,
                        UserId = userId,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    return Task.FromResult<VideoTokenInfo?>(tokenInfo.IsValid ? tokenInfo : null);
                }

                return Task.FromResult<VideoTokenInfo?>(null);
            }            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid video token provided");
                return Task.FromResult<VideoTokenInfo?>(null);
            }
        }        public Task<string> CreateSecureVideoUrlAsync(string videoId, string userId, TimeSpan? expiration = null)
        {
            try
            {
                var tokenExpiration = expiration ?? TimeSpan.FromHours(2); // Default 2 hours
                var token = GenerateVideoTokenAsync(videoId, userId, tokenExpiration).Result;

                // Tạo secure URL với token
                var baseUrl = _configuration["CDN:BaseUrl"] ?? "http://localhost:5288";
                var secureUrl = $"{baseUrl}/api/v1/videos/secure/{videoId}/mp4?token={token}";

                _logger.LogInformation($"Created secure video URL for VideoId: {videoId}, UserId: {userId}");

                return Task.FromResult(secureUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating secure video URL for VideoId: {videoId}, UserId: {userId}");
                throw;
            }
        }        public Task RevokeTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return Task.CompletedTask;

                var cacheKey = $"video_token_{token.GetHashCode()}";
                _cache.Remove(cacheKey);

                // Thêm token vào blacklist cache
                var blacklistKey = $"blacklist_token_{token.GetHashCode()}";
                _cache.Set(blacklistKey, true, TimeSpan.FromDays(1)); // Blacklist trong 1 ngày

                _logger.LogInformation($"Revoked video token: {token[..10]}...");
                return Task.CompletedTask;
            }            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking video token");
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Kiểm tra token có bị blacklist không
        /// </summary>
        public bool IsTokenBlacklisted(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            var blacklistKey = $"blacklist_token_{token.GetHashCode()}";
            return _cache.TryGetValue(blacklistKey, out _);
        }

        /// <summary>
        /// Tạo signed hash cho additional security
        /// </summary>
        private string CreateSignedHash(string data, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}
