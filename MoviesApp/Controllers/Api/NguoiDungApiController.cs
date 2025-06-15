using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungApiController : ControllerBase
    {
        private readonly WebMoviesDbContext _context;

        public NguoiDungApiController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: api/NguoiDungApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> GetNguoiDungs()
        {
            return await _context.NguoiDungs.ToListAsync();
        }

        // GET: api/NguoiDungApi/ND001
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung>> GetNguoiDung(string id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            return nguoiDung;
        }

        // GET: api/NguoiDungApi/email/user@example.com
        [HttpGet("email/{email}")]
        public async Task<ActionResult<NguoiDung>> GetNguoiDungByEmail(string email)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(nd => nd.Email == email);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            return nguoiDung;
        }

        // GET: api/NguoiDungApi/ND001/phimyeuthich
        [HttpGet("{id}/phimyeuthich")]
        public async Task<ActionResult<IEnumerable<PhimYeuThich>>> GetPhimYeuThich(string id)
        {
            return await _context.PhimYeuThichs
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.TheLoaiPhim)
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.QuocGia)
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.DanhMuc)
                .Where(pyt => pyt.MaND == id)
                .ToListAsync();
        }

        // GET: api/NguoiDungApi/ND001/lichsuxem
        [HttpGet("{id}/lichsuxem")]
        public async Task<ActionResult<IEnumerable<LichSuXem>>> GetLichSuXem(string id)
        {
            return await _context.LichSuXems
                .Include(ls => ls.TapPhim)
                    .ThenInclude(t => t.Phim)
                .Where(ls => ls.MaND == id)
                .OrderByDescending(ls => ls.ThoiDiemXem)
                .ToListAsync();
        }

        // POST: api/NguoiDungApi
        [HttpPost]
        public async Task<ActionResult<NguoiDung>> PostNguoiDung(NguoiDung nguoiDung)
        {
            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNguoiDung", new { id = nguoiDung.MaND }, nguoiDung);
        }

        // PUT: api/NguoiDungApi/ND001
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNguoiDung(string id, NguoiDung nguoiDung)
        {
            if (id != nguoiDung.MaND)
            {
                return BadRequest();
            }

            _context.Entry(nguoiDung).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NguoiDungExists(id))
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

        // DELETE: api/NguoiDungApi/ND001
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoiDung(string id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NguoiDungExists(string id)
        {
            return _context.NguoiDungs.Any(e => e.MaND == id);
        }
    }
}
