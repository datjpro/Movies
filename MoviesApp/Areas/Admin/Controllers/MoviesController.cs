using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Areas.Admin.Models;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;
using System.Text.Json;

namespace MoviesApp.Areas.Admin.Controllers
{    [Area("Admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class MoviesController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly ILogger<MoviesController> _logger;
        private readonly HttpClient _httpClient;

        public MoviesController(
            WebMoviesDbContext context,
            IUserActivityService userActivityService,
            ILogger<MoviesController> logger,
            HttpClient httpClient)
        {
            _context = context;
            _userActivityService = userActivityService;
            _logger = logger;
            _httpClient = httpClient;
        }        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? filterGenre = null, int? filterCountry = null)
        {
            try
            {
                // Call API to get movies data
                var apiUrl = $"http://localhost:5032/api/phim?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(search))
                {
                    apiUrl += $"&search={Uri.EscapeDataString(search)}";
                }
                if (filterGenre.HasValue)
                {
                    apiUrl += $"&genre={filterGenre}";
                }
                if (filterCountry.HasValue)
                {
                    apiUrl += $"&country={filterCountry}";
                }

                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch movies from API. Status: {StatusCode}", response.StatusCode);
                    TempData["Error"] = "Không thể tải danh sách phim từ API.";
                    return View(new MovieManagementViewModel());
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<ApiMovieResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Get lookup data for filters (still from local DB for performance)
                var genres = await _context.TheLoaiPhim.OrderBy(g => g.TenTheLoai).ToListAsync();
                var countries = await _context.QuocGia.OrderBy(c => c.TenQuocGia).ToListAsync();
                var categories = await _context.DanhMuc.OrderBy(c => c.TenDanhMuc).ToListAsync();

                var viewModel = new MovieManagementViewModel
                {
                    Movies = apiResult?.Movies ?? new List<Phim>(),
                    Genres = genres,
                    Countries = countries,
                    Categories = categories,
                    TotalMovies = apiResult?.TotalCount ?? 0,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = apiResult?.TotalPages ?? 0,
                    SearchTerm = search,
                    FilterGenre = filterGenre,
                    FilterCountry = filterCountry
                };

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Viewed Movies Management via API", 
                    $"Page {page}, Search: {search ?? "None"}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movies management from API");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách phim từ API.";
                return View(new MovieManagementViewModel());
            }
        }        public async Task<IActionResult> Details(string id)
        {
            try
            {
                // Call API to get movie details
                var response = await _httpClient.GetAsync($"http://localhost:5032/api/phim/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Phim>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (movie == null)
                {
                    return NotFound();
                }

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Viewed Movie Details via API", 
                    $"Movie: {movie.TenPhim}"
                );

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movie details from API for {MovieId}", id);
                return NotFound();
            }
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Call API to delete movie
                var response = await _httpClient.DeleteAsync($"http://localhost:5032/api/phim/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    await _userActivityService.LogActivityAsync(
                        User.Identity!.Name!, 
                        "Deleted Movie via API", 
                        $"Deleted movie ID: {id}"
                    );

                    return Json(new { success = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"API Error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie via API {MovieId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa phim qua API." });
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
        }        [HttpGet]
        public async Task<IActionResult> TestData()
        {
            try
            {
                // Test API connection
                var response = await _httpClient.GetAsync("http://localhost:5032/api/phim?page=1&pageSize=5");
                var content = await response.Content.ReadAsStringAsync();
                
                return Json(new
                {
                    ApiStatus = response.IsSuccessStatusCode ? "OK" : "Error",
                    StatusCode = response.StatusCode,
                    Content = content
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }    // DTO for API response
    public class ApiMovieResponse
    {
        public List<Phim> Movies { get; set; } = new List<Phim>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
