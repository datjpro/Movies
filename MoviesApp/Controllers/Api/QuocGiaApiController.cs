using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuocGiaApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public QuocGiaApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/QuocGiaApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuocGia>>> GetQuocGias()
        {
            return await _context.QuocGias.ToListAsync();
        }

        // GET: api/QuocGiaApi/QG001
        [HttpGet("{id}")]
        public async Task<ActionResult<QuocGia>> GetQuocGia(string id)
        {
            var quocGia = await _context.QuocGias.FindAsync(id);

            if (quocGia == null)
            {
                return NotFound();
            }

            return quocGia;
        }

        // POST: api/QuocGiaApi
        [HttpPost]
        public async Task<ActionResult<QuocGia>> PostQuocGia(QuocGia quocGia)
        {
            _context.QuocGias.Add(quocGia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuocGia", new { id = quocGia.MaQG }, quocGia);
        }

        // PUT: api/QuocGiaApi/QG001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuocGia(string id, QuocGia quocGia)
        {
            if (id != quocGia.MaQG)
            {
                return BadRequest();
            }

            _context.Entry(quocGia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuocGiaExists(id))
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

        // DELETE: api/QuocGiaApi/QG001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuocGia(string id)
        {
            var quocGia = await _context.QuocGias.FindAsync(id);
            if (quocGia == null)
            {
                return NotFound();
            }

            _context.QuocGias.Remove(quocGia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuocGiaExists(string id)
        {
            return _context.QuocGias.Any(e => e.MaQG == id);
        }
    }
}
