using MoviesApp.Models;
using System.Text.Json;

namespace MoviesApp.Services
{
    public class OMDbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OMDbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var omdbSettings = configuration.GetSection("OMDbSettings").Get<OMDbSettings>();
            _apiKey = omdbSettings?.ApiKey ?? throw new ArgumentException("OMDb API Key not found in configuration");
            _baseUrl = omdbSettings?.BaseUrl ?? "http://www.omdbapi.com/";
        }

        public async Task<OMDbMovieResponse?> GetMovieByTitleAsync(string title)
        {
            try
            {
                var url = $"{_baseUrl}?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OMDbMovieResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OMDbMovieResponse?> GetMovieByImdbIdAsync(string imdbId)
        {
            try
            {
                var url = $"{_baseUrl}?i={imdbId}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OMDbMovieResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class OMDbMovieResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Rated { get; set; } = string.Empty;
        public string Released { get; set; } = string.Empty;
        public string Runtime { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Writer { get; set; } = string.Empty;
        public string Actors { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Awards { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string imdbRating { get; set; } = string.Empty;
        public string imdbVotes { get; set; } = string.Empty;
        public string imdbID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string totalSeasons { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Metascore { get; set; } = string.Empty;
        public string BoxOffice { get; set; } = string.Empty;
    }
}
