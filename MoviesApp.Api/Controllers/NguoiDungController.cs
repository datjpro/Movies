using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NguoiDungController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NguoiDungController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> GetAllNguoiDung()
        {
            var nguoiDungs = await _unitOfWork.NguoiDungs.GetAllAsync();
            return Ok(nguoiDungs);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDung>> GetNguoiDung(string id)
        {
            var nguoiDung = await _unitOfWork.NguoiDungs.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return Ok(nguoiDung);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<NguoiDung>> GetNguoiDungByEmail(string email)
        {
            var nguoiDung = await _unitOfWork.NguoiDungs.GetByEmailAsync(email);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return Ok(nguoiDung);
        }        [HttpGet("{id}/phimyeuthich")]
        public async Task<ActionResult<IEnumerable<PhimYeuThich>>> GetPhimYeuThich(string id)
        {
            var phimYeuThichs = await _unitOfWork.NguoiDungs.GetPhimYeuThichByUserIdAsync(id);
            return Ok(phimYeuThichs);
        }

        [HttpGet("{id}/lichsuxem")]
        public async Task<ActionResult<IEnumerable<LichSuXem>>> GetLichSuXem(string id)
        {
            var lichSuXems = await _unitOfWork.NguoiDungs.GetLichSuXemByUserIdAsync(id);
            return Ok(lichSuXems);
        }

        [HttpPost]
        public async Task<ActionResult<NguoiDung>> CreateNguoiDung(NguoiDung nguoiDung)
        {
            await _unitOfWork.NguoiDungs.AddAsync(nguoiDung);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetNguoiDung), new { id = nguoiDung.MaND }, nguoiDung);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNguoiDung(string id, NguoiDung nguoiDung)
        {
            if (id != nguoiDung.MaND)
            {
                return BadRequest();
            }

            var existingNguoiDung = await _unitOfWork.NguoiDungs.GetByIdAsync(id);
            if (existingNguoiDung == null)
            {
                return NotFound();
            }

            _unitOfWork.NguoiDungs.Update(nguoiDung);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoiDung(string id)
        {
            var nguoiDung = await _unitOfWork.NguoiDungs.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            _unitOfWork.NguoiDungs.Remove(nguoiDung);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
