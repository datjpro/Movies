using Microsoft.AspNetCore.Authorization;
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
    public class MoviesController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(
            WebMoviesDbContext context,
            IUserActivityService userActivityService,
            ILogger<MoviesController> logger)
        {
            _context = context;
            _userActivityService = userActivityService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? filterGenre = null, int? filterCountry = null)
        {
            try
            {
                var query = _context.Phim
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .AsQueryable();

                // Search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.TenPhim.Contains(search) || 
                                           p.MoTa.Contains(search));
                }                // Genre filter
                if (filterGenre.HasValue && filterGenre.Value > 0)
                {
                    query = query.Where(p => p.MaTL == filterGenre.Value.ToString());
                }

                // Country filter
                if (filterCountry.HasValue && filterCountry.Value > 0)
                {
                    query = query.Where(p => p.MaQG == filterCountry.Value.ToString());
                }

                var totalMovies = await query.CountAsync();
                var movies = await query
                    .OrderByDescending(p => p.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var genres = await _context.TheLoaiPhim.OrderBy(g => g.TenTheLoai).ToListAsync();
                var countries = await _context.QuocGia.OrderBy(c => c.TenQuocGia).ToListAsync();
                var categories = await _context.DanhMuc.OrderBy(c => c.TenDanhMuc).ToListAsync();

                var viewModel = new MovieManagementViewModel
                {
                    Movies = movies,
                    Genres = genres,
                    Countries = countries,
                    Categories = categories,
                    TotalMovies = totalMovies,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize),
                    SearchTerm = search,
                    FilterGenre = filterGenre,
                    FilterCountry = filterCountry
                };

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Viewed Movies Management", 
                    $"Page {page}, Search: {search ?? "None"}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movies management");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách phim.";
                return View(new MovieManagementViewModel());
            }
        }        public async Task<IActionResult> Details(string id)
        {            var movie = await _context.Phim
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Include(p => p.TapPhims)
                .FirstOrDefaultAsync(p => p.MaPhim == id);

            if (movie == null)
            {
                return NotFound();
            }

            await _userActivityService.LogActivityAsync(
                User.Identity!.Name!, 
                "Viewed Movie Details", 
                $"Movie: {movie.TenPhim}"
            );

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {                var movie = await _context.Phim
                    .Include(p => p.TapPhims)
                    .FirstOrDefaultAsync(p => p.MaPhim == id);

                if (movie == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim." });
                }

                // Remove related episodes first
                if (movie.TapPhims?.Any() == true)
                {
                    _context.TapPhim.RemoveRange(movie.TapPhims);
                }

                _context.Phim.Remove(movie);
                await _context.SaveChangesAsync();

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Deleted Movie", 
                    $"Deleted movie: {movie.TenPhim}"
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie {MovieId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa phim." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var movie = await _context.Phim.FindAsync(id);
                if (movie == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim." });
                }

                // Assuming there's a status field, if not you can add one
                // movie.IsActive = !movie.IsActive;
                
                await _context.SaveChangesAsync();

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Toggled Movie Status", 
                    $"Movie: {movie.TenPhim}"
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling movie status for {MovieId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}
