using MoviesApp.Models;

namespace MoviesApp.Services
{
    public interface IVideoTokenService
    {
        /// <summary>
        /// Tạo token để truy cập video với thời gian hết hạn
        /// </summary>
        Task<string> GenerateVideoTokenAsync(string videoId, string userId, TimeSpan expiration);
        
        /// <summary>
        /// Xác thực video token và trả về thông tin video
        /// </summary>
        Task<VideoTokenInfo?> ValidateVideoTokenAsync(string token);
        
        /// <summary>
        /// Tạo signed URL với token embedded
        /// </summary>
        Task<string> CreateSecureVideoUrlAsync(string videoId, string userId, TimeSpan? expiration = null);
        
        /// <summary>
        /// Xóa token khỏi cache (logout/revoke)
        /// </summary>
        Task RevokeTokenAsync(string token);
    }

    public class VideoTokenInfo
    {
        public string VideoId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsValid => DateTime.UtcNow < ExpiresAt;
    }
}
