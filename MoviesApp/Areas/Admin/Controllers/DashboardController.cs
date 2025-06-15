using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Areas.Admin.Models;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;

namespace MoviesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class DashboardController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserActivityService _userActivityService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            WebMoviesDbContext context,
            UserManager<ApplicationUser> userManager,
            IUserActivityService userActivityService,
            ILogger<DashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userActivityService = userActivityService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var                viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = await _userManager.Users.CountAsync(),
                    TotalMovies = await _context.Phims.CountAsync(),
                    TotalEpisodes = await _context.TapPhims.CountAsync(),
                    TotalGenres = await _context.TheLoaiPhims.CountAsync(),
                    TotalCountries = await _context.QuocGias.CountAsync(),
                    TotalCategories = await _context.DanhMucs.CountAsync()
                };

                // Count users by roles
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Contains(UserRoles.PremiumUser))
                        viewModel.PremiumUsers++;
                    else if (userRoles.Contains(UserRoles.FreeUser))
                        viewModel.FreeUsers++;
                }                // Active users (simplified - users with recent activity)
                viewModel.ActiveUsers = allUsers.Count(u => u.CreatedAt > DateTime.Now.AddDays(-7));

                // Recent movies
                viewModel.RecentMovies = await _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .OrderByDescending(p => p.NgayTao)
                    .Take(5)
                    .ToListAsync();

                // Recent users
                viewModel.RecentUsers = await _userManager.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToListAsync();                // Movies by genre statistics
                var moviesByGenre = await _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .GroupBy(p => p.TheLoaiPhim!.TenTL)
                    .Select(g => new { Genre = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Genre, x => x.Count);

                viewModel.MoviesByGenre = moviesByGenre;

                // Users by role statistics
                viewModel.UsersByRole = new Dictionary<string, int>
                {
                    { UserRoles.Admin, await GetUsersInRoleCount(UserRoles.Admin) },
                    { UserRoles.PremiumUser, await GetUsersInRoleCount(UserRoles.PremiumUser) },
                    { UserRoles.FreeUser, await GetUsersInRoleCount(UserRoles.FreeUser) }
                };

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Accessed Admin Dashboard", 
                    "Dashboard viewed"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "Có lỗi xảy ra khi tải dashboard.";
                return View(new AdminDashboardViewModel());
            }
        }

        private async Task<int> GetUsersInRoleCount(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.Count;
        }
    }
}
