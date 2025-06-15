using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapPhimApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public TapPhimApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/TapPhimApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TapPhim>>> GetTapPhims()
        {
            return await _context.TapPhims
                .Include(t => t.Phim)
                .ToListAsync();
        }

        // GET: api/TapPhimApi/T001
        [HttpGet("{id}")]
        public async Task<ActionResult<TapPhim>> GetTapPhim(string id)
        {
            var tapPhim = await _context.TapPhims
                .Include(t => t.Phim)
                .Include(t => t.BinhLuans)
                .Include(t => t.DanhGias)
                .FirstOrDefaultAsync(t => t.MaTap == id);

            if (tapPhim == null)
            {
                return NotFound();
            }

            return tapPhim;
        }

        // GET: api/TapPhimApi/phim/P001
        [HttpGet("phim/{phimId}")]
        public async Task<ActionResult<IEnumerable<TapPhim>>> GetTapPhimsByPhimId(string phimId)
        {
            return await _context.TapPhims
                .Include(t => t.Phim)
                .Where(t => t.MaPhim == phimId)
                .OrderBy(t => t.SoTapThuTu)
                .ToListAsync();
        }

        // POST: api/TapPhimApi
        [HttpPost]
        public async Task<ActionResult<TapPhim>> PostTapPhim(TapPhim tapPhim)
        {
            _context.TapPhims.Add(tapPhim);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTapPhim", new { id = tapPhim.MaTap }, tapPhim);
        }

        // PUT: api/TapPhimApi/T001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTapPhim(string id, TapPhim tapPhim)
        {
            if (id != tapPhim.MaTap)
            {
                return BadRequest();
            }

            _context.Entry(tapPhim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TapPhimExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TapPhimApi/T001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTapPhim(string id)
        {
            var tapPhim = await _context.TapPhims.FindAsync(id);
            if (tapPhim == null)
            {
                return NotFound();
            }

            _context.TapPhims.Remove(tapPhim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TapPhimExists(string id)
        {
            return _context.TapPhims.Any(e => e.MaTap == id);
        }
    }
}
