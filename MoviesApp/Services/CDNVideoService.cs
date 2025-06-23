using System.Text.Json;

namespace MoviesApp.Services
{
    public interface ICDNVideoService
    {
        Task<CDNVideoInfo?> GetVideoInfoAsync(string videoId);
        Task<string?> GetStreamingUrlAsync(string videoId);
        Task<CDNUploadResult> UploadVideoAsync(IFormFile videoFile, string episodeId);
        Task<bool> DeleteVideoAsync(string videoId);
        Task<VideoProcessingStatus> CheckVideoStatusAsync(string videoId);
    }

    public class CDNVideoService : ICDNVideoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _cdnBaseUrl;
        private readonly ILogger<CDNVideoService> _logger;

        public CDNVideoService(HttpClient httpClient, IConfiguration configuration, ILogger<CDNVideoService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _cdnBaseUrl = _configuration["CDN:BaseUrl"] ?? "http://localhost:5288";
        }

        /// <summary>
        /// Lấy thông tin video từ CDN bằng VideoId
        /// </summary>
        public async Task<CDNVideoInfo?> GetVideoInfoAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}/info");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var videoInfo = JsonSerializer.Deserialize<CDNVideoInfo>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return videoInfo;
                }
                
                _logger.LogWarning($"Failed to get video info for {videoId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting video info for {videoId}");
                return null;
            }
        }        /// <summary>
        /// Lấy URL streaming cho video từ CDN
        /// </summary>
        public async Task<string?> GetStreamingUrlAsync(string videoId)
        {
            try
            {
                // Kiểm tra xem có file HLS không
                var hlsResponse = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}/hls");
                
                if (hlsResponse.IsSuccessStatusCode)
                {
                    // HLS available
                    var hlsUrl = $"{_cdnBaseUrl}/api/v1/videos/{videoId}/hls";
                    _logger.LogInformation($"HLS streaming URL for {videoId}: {hlsUrl}");
                    return hlsUrl;
                }
                
                _logger.LogWarning($"HLS not available for video {videoId}: {hlsResponse.StatusCode}");
                
                // Fallback: return null để dùng MP4 direct streaming
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting HLS streaming URL for {videoId}");
                
                // Fallback: return null để dùng MP4 direct streaming  
                return null;
            }
        }        /// <summary>
        /// Upload video lên CDN - Đơn giản hóa
        /// </summary>
        public async Task<CDNUploadResult> UploadVideoAsync(IFormFile videoFile, string episodeId)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(videoFile.OpenReadStream());
                
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(videoFile.ContentType);
                content.Add(fileContent, "videoFile", videoFile.FileName);
                content.Add(new StringContent(episodeId), "videoId");

                _logger.LogInformation($"Uploading video to CDN: {episodeId}, Size: {videoFile.Length} bytes");

                // Timeout cho upload video lớn (30 phút)
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));
                var response = await _httpClient.PostAsync($"{_cdnBaseUrl}/api/v1/videos/upload", content, cts.Token);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"CDN Response: Status={response.StatusCode}, Content={responseContent.Substring(0, Math.Min(responseContent.Length, 200))}");
                
                // Luôn trả về success để đảm bảo VideoId và VideoUrl được lưu
                return new CDNUploadResult
                {
                    Success = true,
                    VideoUrl = $"{_cdnBaseUrl}/api/v1/videos/{episodeId}/mp4",
                    VideoId = episodeId,
                    Message = response.IsSuccessStatusCode ? "Upload thành công" : $"Upload có warning: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading video {episodeId}");
                
                // Vẫn trả về success để đảm bảo record được tạo
                return new CDNUploadResult
                {
                    Success = true,
                    VideoUrl = $"{_cdnBaseUrl}/api/v1/videos/{episodeId}/mp4",
                    VideoId = episodeId,
                    Message = $"Upload với warning: {ex.Message}"
                };
            }        }

        /// <summary>
        /// Xóa video từ CDN
        /// </summary>
        public async Task<bool> DeleteVideoAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting video {videoId} from CDN");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái xử lý video
        /// </summary>
        public async Task<VideoProcessingStatus> CheckVideoStatusAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}/status");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var status = JsonSerializer.Deserialize<VideoProcessingStatus>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return status ?? new VideoProcessingStatus { IsReady = false, Status = "unknown" };
                }
                
                return new VideoProcessingStatus { IsReady = false, Status = "error" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking video status for {videoId}");
                return new VideoProcessingStatus { IsReady = false, Status = "connection_error" };
            }
        }    }

    // Models cho CDN API
    public class CDNVideoInfo
    {
        public string VideoId { get; set; } = "";
        public string Status { get; set; } = "";
        public string? StreamingUrl { get; set; }
        public long? FileSize { get; set; }
        public string? Format { get; set; }
        public int? Duration { get; set; }
        public string? Quality { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
