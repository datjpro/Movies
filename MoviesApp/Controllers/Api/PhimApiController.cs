using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;
using Microsoft.Extensions.Logging;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;
        private readonly ILogger<PhimApiController> _logger;

        public PhimApiController(WebMoviesDbContext context, ILogger<PhimApiController> logger)
        {
            _context = context;
            _logger = logger;
        }        // GET: api/PhimApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPhims(int pageSize = 20)
        {
            try
            {
                var phims = await _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .Take(pageSize)
                    .Select(p => new {
                        p.MaPhim,
                        p.TenPhim,
                        p.AnhPhim,
                        p.MoTaPhim,
                        p.NamPhatHanh,
                        p.ThoiLuongPhim,
                        p.DiemImdb,
                        p.DaoDien,
                        p.DienVien,
                        p.BienKich,
                        p.LoaiPhim,
                        p.NgonNgu,
                        p.QuocGiaSanXuat,
                        p.XepHang,
                        TheLoai = p.TheLoaiPhim != null ? p.TheLoaiPhim.TenTL : "",
                        QuocGia = p.QuocGia != null ? p.QuocGia.TenQG : "",
                        DanhMuc = p.DanhMuc != null ? p.DanhMuc.TenDM : ""
                    })
                    .ToListAsync();
                
                return Ok(phims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/PhimApi/P001
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<ActionResult<Phim>> GetPhim(string id)
        {
            try
            {
                var phim = await _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .FirstOrDefaultAsync(p => p.MaPhim == id);

                if (phim == null)
                {
                    return NotFound(new { message = "Movie not found" });
                }

                return Ok(phim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie {MovieId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/PhimApi
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Phim>> PostPhim(Phim phim)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Phims.Add(phim);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Movie {MovieId} created successfully", phim.MaPhim);
                return CreatedAtAction(nameof(GetPhim), new { id = phim.MaPhim }, phim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // PUT: api/PhimApi/P001
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> PutPhim(string id, Phim phim)
        {
            try
            {
                if (id != phim.MaPhim)
                {
                    return BadRequest(new { message = "Movie ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Entry(phim).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Movie {MovieId} updated successfully", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhimExists(id))
                    {
                        return NotFound(new { message = "Movie not found" });
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie {MovieId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // DELETE: api/PhimApi/P001
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeletePhim(string id)
        {
            try
            {
                var phim = await _context.Phims.FindAsync(id);
                if (phim == null)
                {
                    return NotFound(new { message = "Movie not found" });
                }

                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Movie {MovieId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie {MovieId}", id);                return StatusCode(500, new { message = "Internal server error" });
            }
        }        // GET: api/PhimApi/recommended
        [HttpGet("recommended")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecommendedMovies(int pageSize = 8)
        {
            try
            {
                var currentUserId = User.Identity?.Name;
                
                // Get user's viewing history through TapPhim -> Phim
                var userHistory = await _context.LichSuXems
                    .Where(l => l.NguoiDung.Email == currentUserId)
                    .Include(l => l.TapPhim)
                    .ThenInclude(t => t.Phim)
                    .ThenInclude(p => p.TheLoaiPhim)
                    .Include(l => l.TapPhim)
                    .ThenInclude(t => t.Phim)
                    .ThenInclude(p => p.QuocGia)
                    .OrderByDescending(l => l.ThoiDiemXem)
                    .Take(20) // Take more to get variety
                    .ToListAsync();

                // Get user's favorite movies
                var userFavorites = await _context.PhimYeuThichs
                    .Where(f => f.NguoiDung.Email == currentUserId)
                    .Include(f => f.Phim)
                    .ThenInclude(p => p.TheLoaiPhim)
                    .Include(f => f.Phim)
                    .ThenInclude(p => p.QuocGia)
                    .ToListAsync();

                // Collect preferred genres and countries from viewing history
                var preferredGenres = new List<string>();
                var preferredCountries = new List<string>();
                var watchedMovieIds = new List<string>();
                
                foreach (var history in userHistory)
                {
                    if (history.TapPhim?.Phim != null)
                    {
                        var phim = history.TapPhim.Phim;
                        watchedMovieIds.Add(phim.MaPhim);
                        
                        if (!string.IsNullOrEmpty(phim.MaTL))
                            preferredGenres.Add(phim.MaTL);
                        if (!string.IsNullOrEmpty(phim.MaQG))
                            preferredCountries.Add(phim.MaQG);
                    }
                }
                
                // Collect from favorites
                foreach (var favorite in userFavorites)
                {
                    if (favorite.Phim != null)
                    {
                        if (!string.IsNullOrEmpty(favorite.Phim.MaTL))
                            preferredGenres.Add(favorite.Phim.MaTL);
                        if (!string.IsNullOrEmpty(favorite.Phim.MaQG))
                            preferredCountries.Add(favorite.Phim.MaQG);
                    }
                }

                var favoriteMovieIds = userFavorites.Select(f => f.MaPhim).ToList();
                var excludedMovieIds = watchedMovieIds.Concat(favoriteMovieIds).Distinct().ToList();

                IQueryable<Phim> recommendedQuery = _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .Where(p => !excludedMovieIds.Contains(p.MaPhim));

                // If user has preferences, prioritize based on them
                if (preferredGenres.Any() || preferredCountries.Any())
                {
                    var topGenres = preferredGenres.GroupBy(g => g)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key)
                        .ToList();
                    
                    var topCountries = preferredCountries.GroupBy(c => c)
                        .OrderByDescending(c => c.Count())
                        .Take(2)
                        .Select(c => c.Key)
                        .ToList();

                    // Prioritize movies with preferred genres and countries
                    recommendedQuery = recommendedQuery
                        .OrderByDescending(p => topGenres.Contains(p.MaTL ?? ""))
                        .ThenByDescending(p => topCountries.Contains(p.MaQG ?? ""))
                        .ThenByDescending(p => p.DiemImdb ?? 0)
                        .ThenByDescending(p => p.NgayTao);
                }
                else
                {
                    // For new users or users without history, recommend popular movies
                    recommendedQuery = recommendedQuery
                        .OrderByDescending(p => p.DiemImdb ?? 0)
                        .ThenByDescending(p => p.TongSoMua ?? 0)
                        .ThenByDescending(p => p.NgayTao);
                }

                var recommendedMovies = await recommendedQuery
                    .Take(pageSize)
                    .Select(p => new {
                        p.MaPhim,
                        p.TenPhim,
                        p.AnhPhim,
                        p.MoTaPhim,
                        p.NamPhatHanh,
                        p.ThoiLuongPhim,
                        p.DiemImdb,
                        p.DaoDien,
                        p.DienVien,
                        p.BienKich,
                        p.LoaiPhim,
                        p.NgonNgu,
                        p.QuocGiaSanXuat,
                        p.XepHang,
                        TheLoai = p.TheLoaiPhim != null ? p.TheLoaiPhim.TenTL : "",
                        QuocGia = p.QuocGia != null ? p.QuocGia.TenQG : "",
                        DanhMuc = p.DanhMuc != null ? p.DanhMuc.TenDM : "",
                        IsRecommended = true,
                        RecommendationReason = GetRecommendationReason(p, preferredGenres, preferredCountries)
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} recommended movies for user {UserId}", 
                    recommendedMovies.Count, currentUserId ?? "anonymous");
                    
                return Ok(recommendedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recommended movies");
                
                // Fallback to popular movies if recommendation fails
                var fallbackMovies = await _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .OrderByDescending(p => p.DiemImdb ?? 0)
                    .Take(pageSize)
                    .Select(p => new {
                        p.MaPhim,
                        p.TenPhim,
                        p.AnhPhim,
                        p.MoTaPhim,
                        p.NamPhatHanh,
                        p.ThoiLuongPhim,
                        p.DiemImdb,
                        p.DaoDien,
                        p.DienVien,
                        p.BienKich,
                        p.LoaiPhim,
                        p.NgonNgu,
                        p.QuocGiaSanXuat,
                        p.XepHang,
                        TheLoai = p.TheLoaiPhim != null ? p.TheLoaiPhim.TenTL : "",
                        QuocGia = p.QuocGia != null ? p.QuocGia.TenQG : "",
                        DanhMuc = p.DanhMuc != null ? p.DanhMuc.TenDM : "",
                        IsRecommended = false,
                        RecommendationReason = "Popular Movies"
                    })
                    .ToListAsync();
                    
                return Ok(fallbackMovies);
            }
        }        private string GetRecommendationReason(Phim phim, List<string> preferredGenres, List<string> preferredCountries)
        {
            var reasons = new List<string>();
            
            if (preferredGenres.Contains(phim.MaTL ?? ""))
                reasons.Add($"Thể loại {phim.TheLoaiPhim?.TenTL} yêu thích");
            
            if (preferredCountries.Contains(phim.MaQG ?? ""))
                reasons.Add($"Phim {phim.QuocGia?.TenQG} yêu thích");
            
            if (phim.DiemImdb >= 8.0m)
                reasons.Add("Điểm IMDb cao");
            
            if (phim.NamPhatHanh >= DateTime.Now.Year - 2)
                reasons.Add("Phim mới");
                
            return reasons.Any() ? string.Join(", ", reasons) : "Đề xuất cho bạn";
        }

        private bool PhimExists(string id)
        {
            return _context.Phims.Any(e => e.MaPhim == id);
        }
    }
}
