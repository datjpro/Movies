namespace MoviesApp.Models
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenLifetimeInMinutes { get; set; } = 60;
        public int RefreshTokenLifetimeInDays { get; set; } = 7;
    }
}
