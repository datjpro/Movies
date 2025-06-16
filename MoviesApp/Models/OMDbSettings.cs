namespace MoviesApp.Models
{
    public class OMDbSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "http://www.omdbapi.com/";
    }
}
