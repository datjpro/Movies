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
        }        public async Task<IActionResult> Movies(int page = 1, int pageSize = 10, string? search = null, string? category = null, int? year = null)
        {
            var query = _context.Phims
                .Include(p => p.DanhMuc)
                .Include(p => p.QuocGia)
                .AsQueryable();            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenPhim.Contains(search) || 
                                       (!string.IsNullOrEmpty(p.DaoDien) && p.DaoDien.Contains(search)) ||
                                       (!string.IsNullOrEmpty(p.MoTaPhim) && p.MoTaPhim.Contains(search)));
                ViewData["CurrentFilter"] = search;
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.DanhMuc != null && p.DanhMuc.TenDanhMuc == category);
                ViewData["CurrentCategory"] = category;
            }            // Apply year filter
            if (year.HasValue)
            {
                query = query.Where(p => p.NamPhatHanh == year.Value);
                ViewData["CurrentYear"] = year;
            }

            var totalMovies = await query.CountAsync();
            var movies = await query
                .OrderByDescending(p => p.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            return View(movies);
        }        [HttpPost]
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
        }        [HttpPost]
        public async Task<IActionResult> DeleteMovie(string id)
        {
            try
            {
                var movie = await _context.Phims.FindAsync(id);
                if (movie == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim" });
                }

                // Check if movie has related data (episodes, comments through episodes, ratings through episodes)
                var hasEpisodes = await _context.TapPhims.AnyAsync(t => t.MaPhim == id);
                
                if (hasEpisodes)
                {
                    // Check if there are comments or ratings for any episode of this movie
                    var episodeIds = await _context.TapPhims
                        .Where(t => t.MaPhim == id)
                        .Select(t => t.MaTap)
                        .ToListAsync();
                    
                    var hasComments = await _context.BinhLuans.AnyAsync(b => episodeIds.Contains(b.MaTap));
                    var hasRatings = await _context.DanhGias.AnyAsync(d => episodeIds.Contains(d.MaTap));
                    
                    if (hasComments || hasRatings)
                    {
                        return Json(new { success = false, message = "Không thể xóa phim này vì đã có dữ liệu liên quan (tập phim, bình luận, đánh giá)" });
                    }
                    
                    // Remove all episodes first
                    var episodes = await _context.TapPhims.Where(t => t.MaPhim == id).ToListAsync();
                    _context.TapPhims.RemoveRange(episodes);
                }

                _context.Phims.Remove(movie);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa phim thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa phim: " + ex.Message });
            }
        }
    }
}
