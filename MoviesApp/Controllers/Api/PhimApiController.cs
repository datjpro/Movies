using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public PhimApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/PhimApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phim>>> GetPhims()
        {
            return await _context.Phims
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .ToListAsync();
        }

        // GET: api/PhimApi/P001
        [HttpGet("{id}")]
        public async Task<ActionResult<Phim>> GetPhim(string id)
        {
            var phim = await _context.Phims
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Include(p => p.TapPhims)
                .FirstOrDefaultAsync(p => p.MaPhim == id);

            if (phim == null)
            {
                return NotFound();
            }

            return phim;
        }

        // POST: api/PhimApi
        [HttpPost]
        public async Task<ActionResult<Phim>> PostPhim(Phim phim)
        {
            _context.Phims.Add(phim);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhim", new { id = phim.MaPhim }, phim);
        }

        // PUT: api/PhimApi/P001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhim(string id, Phim phim)
        {
            if (id != phim.MaPhim)
            {
                return BadRequest();
            }

            _context.Entry(phim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhimExists(id))
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

        // DELETE: api/PhimApi/P001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhim(string id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim == null)
            {
                return NotFound();
            }

            _context.Phims.Remove(phim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhimExists(string id)
        {
            return _context.Phims.Any(e => e.MaPhim == id);
        }
    }
}
