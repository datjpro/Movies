using System.Text;
using System.Text.Json;

namespace MoviesApp.Services
{
    public class CDNService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _cdnBaseUrl;

        public CDNService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cdnBaseUrl = _configuration["CDN:BaseUrl"] ?? "http://localhost:5288";
        }        /// <summary>
        /// Upload video file to CDN and get streaming URL
        /// </summary>
        /// <param name="videoFile">Video file to upload</param>
        /// <param name="episodeId">Episode identifier for organizing videos</param>
        /// <returns>Upload result with video URL</returns>
        public async Task<CDNUploadResult> UploadVideoAsync(IFormFile videoFile, string episodeId)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(videoFile.OpenReadStream());
                
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(videoFile.ContentType);
                content.Add(fileContent, "videoFile", videoFile.FileName);
                content.Add(new StringContent(episodeId), "videoId");

                var response = await _httpClient.PostAsync($"{_cdnBaseUrl}/api/v1/videos/upload", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse.TryGetProperty("success", out var successProp) && successProp.GetBoolean() && 
                        apiResponse.TryGetProperty("data", out var dataProp))
                    {
                        var streamingUrl = "";
                        var videoId = "";

                        if (dataProp.TryGetProperty("streamingUrl", out var urlProp))
                            streamingUrl = urlProp.GetString() ?? "";
                        
                        if (dataProp.TryGetProperty("videoId", out var idProp))
                            videoId = idProp.GetString() ?? "";                        return new CDNUploadResult
                        {
                            Success = true,
                            VideoUrl = streamingUrl,
                            VideoId = videoId,
                            Message = "Upload thành công"
                        };
                    }
                    else
                    {
                        return new CDNUploadResult
                        {
                            Success = false,
                            Message = "Lỗi: Response không hợp lệ từ CDN"
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new CDNUploadResult
                    {
                        Success = false,
                        Message = $"Lỗi upload: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new CDNUploadResult
                {
                    Success = false,
                    Message = $"Lỗi kết nối CDN: {ex.Message}"
                };
            }
        }        /// <summary>
        /// Delete video from CDN
        /// </summary>
        /// <param name="videoUrl">Video URL to delete</param>
        /// <returns>Delete result</returns>
        public async Task<CDNDeleteResult> DeleteVideoAsync(string videoUrl)
        {
            try
            {
                // Extract video ID from URL or use URL as identifier
                var videoId = ExtractVideoIdFromUrl(videoUrl);
                
                var response = await _httpClient.DeleteAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return new CDNDeleteResult
                    {
                        Success = true,
                        Message = "Xóa video thành công"
                    };
                }
                else
                {
                    return new CDNDeleteResult
                    {
                        Success = false,
                        Message = $"Lỗi xóa video: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new CDNDeleteResult
                {
                    Success = false,
                    Message = $"Lỗi kết nối: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Check video processing status
        /// </summary>
        /// <param name="videoUrl">Video URL to check</param>
        /// <returns>Video status information</returns>
        public async Task<VideoProcessingStatus> CheckVideoStatusAsync(string videoUrl)
        {
            try
            {
                var videoId = ExtractVideoIdFromUrl(videoUrl);
                var response = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/v1/videos/{videoId}/status");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var status = JsonSerializer.Deserialize<VideoProcessingStatus>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return status ?? new VideoProcessingStatus { Status = "Unknown", IsReady = false };
                }
                
                return new VideoProcessingStatus { Status = "Error", IsReady = false };
            }
            catch
            {
                return new VideoProcessingStatus { Status = "Error", IsReady = false };
            }
        }

        /// <summary>
        /// Extract video ID from CDN URL
        /// </summary>
        /// <param name="videoUrl">Full video URL</param>
        /// <returns>Video ID or filename</returns>
        private string ExtractVideoIdFromUrl(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
                return "";

            // If it's a full CDN URL, extract the video ID from the path
            var uri = new Uri(videoUrl);
            var segments = uri.Segments;
            
            // Return the last segment (filename) as video ID
            return segments.LastOrDefault()?.TrimEnd('/') ?? videoUrl;
        }

        /// <summary>
        /// Check if CDN server is available
        /// </summary>
        /// <returns>True if CDN is available</returns>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get video processing status
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <returns>Processing status</returns>
        public async Task<VideoProcessingStatus> GetProcessingStatusAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_cdnBaseUrl}/api/video/status/{videoId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var status = JsonSerializer.Deserialize<VideoProcessingStatus>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return status ?? new VideoProcessingStatus { Status = "Unknown" };
                }
                
                return new VideoProcessingStatus { Status = "Error" };
            }
            catch
            {
                return new VideoProcessingStatus { Status = "Error" };
            }
        }    }

    // DTOs for CDN communication
    public class CDNUploadResult
    {
        public bool Success { get; set; }
        public string VideoUrl { get; set; } = "";
        public string VideoId { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class CDNDeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }

    public class CDNUploadResponse
    {
        public string VideoId { get; set; } = "";
        public string StreamingUrl { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class VideoProcessingStatus
    {
        public string Status { get; set; } = ""; // Processing, Completed, Error
        public bool IsReady { get; set; }
        public int ProcessingProgress { get; set; }
        public string Message { get; set; } = "";
        public List<string> AvailableQualities { get; set; } = new();
    }
}
