using MoviesApp.Models;
using MoviesApp.Services;

namespace MoviesApp.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalMovies { get; set; }
        public int TotalEpisodes { get; set; }
        public int TotalGenres { get; set; }
        public int TotalCountries { get; set; }
        public int TotalCategories { get; set; }
        
        public int ActiveUsers { get; set; }
        public int PremiumUsers { get; set; }
        public int FreeUsers { get; set; }
        
        public List<UserActivity> RecentActivities { get; set; } = new List<UserActivity>();
        public List<Phim> RecentMovies { get; set; } = new List<Phim>();
        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
        
        public Dictionary<string, int> MoviesByGenre { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsersByRole { get; set; } = new Dictionary<string, int>();
    }
}
