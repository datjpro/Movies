using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class SimpleAdminController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SimpleAdminController(
            WebMoviesDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalMovies = await _context.Phims.CountAsync();
            ViewBag.TotalEpisodes = await _context.TapPhims.CountAsync();
            
            var recentUsers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .ToListAsync();
                
            return View(recentUsers);
        }

        public async Task<IActionResult> Users(int page = 1, int pageSize = 10)
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            return View(users);
        }

        public async Task<IActionResult> Movies(int page = 1, int pageSize = 10)
        {
            var totalMovies = await _context.Phims.CountAsync();
            var movies = await _context.Phims
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .OrderByDescending(p => p.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            return View(movies);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                return Json(new { success = true, isActive = user.IsActive });
            }
            return Json(new { success = false });
        }
    }
}
