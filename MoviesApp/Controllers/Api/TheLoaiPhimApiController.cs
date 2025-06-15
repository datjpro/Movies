using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheLoaiPhimApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public TheLoaiPhimApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/TheLoaiPhimApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheLoaiPhim>>> GetTheLoaiPhims()
        {
            return await _context.TheLoaiPhims.ToListAsync();
        }

        // GET: api/TheLoaiPhimApi/TL001
        [HttpGet("{id}")]
        public async Task<ActionResult<TheLoaiPhim>> GetTheLoaiPhim(string id)
        {
            var theLoaiPhim = await _context.TheLoaiPhims.FindAsync(id);

            if (theLoaiPhim == null)
            {
                return NotFound();
            }

            return theLoaiPhim;
        }

        // POST: api/TheLoaiPhimApi
        [HttpPost]
        public async Task<ActionResult<TheLoaiPhim>> PostTheLoaiPhim(TheLoaiPhim theLoaiPhim)
        {
            _context.TheLoaiPhims.Add(theLoaiPhim);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTheLoaiPhim", new { id = theLoaiPhim.MaTL }, theLoaiPhim);
        }

        // PUT: api/TheLoaiPhimApi/TL001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTheLoaiPhim(string id, TheLoaiPhim theLoaiPhim)
        {
            if (id != theLoaiPhim.MaTL)
            {
                return BadRequest();
            }

            _context.Entry(theLoaiPhim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheLoaiPhimExists(id))
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

        // DELETE: api/TheLoaiPhimApi/TL001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheLoaiPhim(string id)
        {
            var theLoaiPhim = await _context.TheLoaiPhims.FindAsync(id);
            if (theLoaiPhim == null)
            {
                return NotFound();
            }

            _context.TheLoaiPhims.Remove(theLoaiPhim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TheLoaiPhimExists(string id)
        {
            return _context.TheLoaiPhims.Any(e => e.MaTL == id);
        }
    }
}
