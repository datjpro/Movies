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
