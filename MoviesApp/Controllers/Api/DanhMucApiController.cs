using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public DanhMucApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/DanhMucApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhMuc>>> GetDanhMucs()
        {
            return await _context.DanhMucs.ToListAsync();
        }

        // GET: api/DanhMucApi/DM001
        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMuc>> GetDanhMuc(string id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            return danhMuc;
        }

        // POST: api/DanhMucApi
        [HttpPost]
        public async Task<ActionResult<DanhMuc>> PostDanhMuc(DanhMuc danhMuc)
        {
            _context.DanhMucs.Add(danhMuc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDanhMuc", new { id = danhMuc.MaDM }, danhMuc);
        }

        // PUT: api/DanhMucApi/DM001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDanhMuc(string id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDM)
            {
                return BadRequest();
            }

            _context.Entry(danhMuc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhMucExists(id))
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

        // DELETE: api/DanhMucApi/DM001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhMuc(string id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            _context.DanhMucs.Remove(danhMuc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DanhMucExists(string id)
        {
            return _context.DanhMucs.Any(e => e.MaDM == id);
        }
    }
}
