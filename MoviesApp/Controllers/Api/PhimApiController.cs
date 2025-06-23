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
        public async Task<ActionResult<object>> GetPhims(
            int page = 1, 
            int pageSize = 20, 
            string? search = null, 
            int? genre = null, 
            int? country = null)
        {
            try
            {
                var query = _context.Phims
                    .Include(p => p.TheLoaiPhim)
                    .Include(p => p.QuocGia)
                    .Include(p => p.DanhMuc)
                    .Include(p => p.TapPhims)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.TenPhim.Contains(search) || 
                                           (p.MoTaPhim != null && p.MoTaPhim.Contains(search)));
                }

                // Apply genre filter
                if (genre.HasValue && genre.Value > 0)
                {
                    query = query.Where(p => p.MaTL == genre.Value.ToString());
                }

                // Apply country filter
                if (country.HasValue && country.Value > 0)
                {
                    query = query.Where(p => p.MaQG == country.Value.ToString());
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var phims = await query
                    .OrderByDescending(p => p.NgayTao)
                    .Skip((page - 1) * pageSize)
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
                        p.NgayTao,
                        // Compatibility properties for Admin view
                        PhimId = p.MaPhim,
                        PosterUrl = p.AnhPhim,
                        MoTa = p.MoTaPhim,
                        NamSanXuat = p.NamPhatHanh,                        TheLoaiPhim = p.TheLoaiPhim != null ? new { TenTheLoai = p.TheLoaiPhim.TenTL } : null,
                        QuocGia = p.QuocGia != null ? new { TenQuocGia = p.QuocGia.TenQG } : null,
                        DanhMuc = p.DanhMuc != null ? new { TenDanhMuc = p.DanhMuc.TenDM } : null,
                        TapPhimsCount = p.TapPhims != null ? p.TapPhims.Count() : 0
                    })
                    .ToListAsync();

                var result = new {
                    Movies = phims,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies with pagination");
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
                _logger.LogError(ex, "Error deleting movie {MovieId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private bool PhimExists(string id)
        {
            return _context.Phims.Any(e => e.MaPhim == id);
        }
    }
}
