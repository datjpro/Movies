using System.Text.Json;

namespace MoviesApp.Services
{
    public class YouTubeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<YouTubeService> _logger;

        public YouTubeService(HttpClient httpClient, IConfiguration configuration, ILogger<YouTubeService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["YouTubeSettings:ApiKey"] ?? "";
            _logger = logger;
        }

        public async Task<string?> SearchTrailerAsync(string movieTitle, int? year = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogWarning("YouTube API key not configured");
                    return null;
                }

                // Tạo query search
                var searchQuery = $"{movieTitle}";
                if (year.HasValue)
                {
                    searchQuery += $" {year}";
                }
                searchQuery += " trailer";

                var encodedQuery = Uri.EscapeDataString(searchQuery);
                var url = $"https://www.googleapis.com/youtube/v3/search" +
                         $"?part=snippet" +
                         $"&q={encodedQuery}" +
                         $"&type=video" +
                         $"&maxResults=5" +
                         $"&order=relevance" +
                         $"&videoDuration=short" +
                         $"&key={_apiKey}";

                _logger.LogInformation($"Searching YouTube for: {searchQuery}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"YouTube API error: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                var root = document.RootElement;

                if (root.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        if (item.TryGetProperty("id", out var id) && 
                            id.TryGetProperty("videoId", out var videoId))
                        {
                            var videoIdStr = videoId.GetString();
                            if (!string.IsNullOrEmpty(videoIdStr))
                            {
                                // Kiểm tra xem video có phải trailer thực sự không
                                if (item.TryGetProperty("snippet", out var snippet) &&
                                    snippet.TryGetProperty("title", out var title))
                                {
                                    var titleStr = title.GetString()?.ToLower() ?? "";
                                    if (titleStr.Contains("trailer") || 
                                        titleStr.Contains("official trailer") ||
                                        titleStr.Contains("movie trailer"))
                                    {
                                        return $"https://www.youtube.com/watch?v={videoIdStr}";
                                    }
                                }
                            }
                        }
                    }

                    // Nếu không tìm thấy trailer với keyword, lấy video đầu tiên
                    var firstItem = items[0];
                    if (firstItem.TryGetProperty("id", out var firstId) && 
                        firstId.TryGetProperty("videoId", out var firstVideoId))
                    {
                        var videoIdStr = firstVideoId.GetString();
                        if (!string.IsNullOrEmpty(videoIdStr))
                        {
                            return $"https://www.youtube.com/watch?v={videoIdStr}";
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching YouTube for trailer: {movieTitle}");
                return null;
            }
        }

        public static string? GetYouTubeVideoId(string? youtubeUrl)
        {
            if (string.IsNullOrEmpty(youtubeUrl))
                return null;

            // Extract video ID from various YouTube URL formats
            var patterns = new[]
            {
                @"(?:https?://)?(?:www\.)?youtube\.com/watch\?v=([^&]+)",
                @"(?:https?://)?(?:www\.)?youtu\.be/([^?]+)",
                @"(?:https?://)?(?:www\.)?youtube\.com/embed/([^?]+)"
            };

            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(youtubeUrl, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string? GetYouTubeEmbedUrl(string? youtubeUrl)
        {
            var videoId = GetYouTubeVideoId(youtubeUrl);
            return string.IsNullOrEmpty(videoId) ? null : $"https://www.youtube.com/embed/{videoId}";
        }
    }
}
