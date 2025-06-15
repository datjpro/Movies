using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhimController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhimController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phim>>> GetAllPhim()
        {
            var phims = await _unitOfWork.Phims.GetAllAsync();
            return Ok(phims);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<Phim>> GetPhim(string id)
        {
            var phim = await _unitOfWork.Phims.GetPhimWithDetailsAsync(id);
            if (phim == null)
            {
                return NotFound();
            }
            return Ok(phim);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Phim>>> SearchPhim([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term cannot be empty");
            }

            var phims = await _unitOfWork.Phims.SearchPhimAsync(searchTerm);
            return Ok(phims);
        }        [HttpGet("theloai/{theLoaiId}")]
        public async Task<ActionResult<IEnumerable<Phim>>> GetPhimsByTheLoai(string theLoaiId)
        {
            var phims = await _unitOfWork.Phims.GetPhimsByTheLoaiAsync(theLoaiId);
            return Ok(phims);
        }

        [HttpGet("quocgia/{quocGiaId}")]
        public async Task<ActionResult<IEnumerable<Phim>>> GetPhimsByQuocGia(string quocGiaId)
        {
            var phims = await _unitOfWork.Phims.GetPhimsByQuocGiaAsync(quocGiaId);
            return Ok(phims);
        }

        [HttpGet("danhmuc/{danhMucId}")]
        public async Task<ActionResult<IEnumerable<Phim>>> GetPhimsByDanhMuc(string danhMucId)
        {
            var phims = await _unitOfWork.Phims.GetPhimsByDanhMucAsync(danhMucId);
            return Ok(phims);
        }

        [HttpPost]
        public async Task<ActionResult<Phim>> CreatePhim(Phim phim)
        {
            await _unitOfWork.Phims.AddAsync(phim);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetPhim), new { id = phim.MaPhim }, phim);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhim(string id, Phim phim)
        {
            if (id != phim.MaPhim)
            {
                return BadRequest();
            }

            var existingPhim = await _unitOfWork.Phims.GetByIdAsync(id);
            if (existingPhim == null)
            {
                return NotFound();
            }

            _unitOfWork.Phims.Update(phim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhim(string id)
        {
            var phim = await _unitOfWork.Phims.GetByIdAsync(id);
            if (phim == null)
            {
                return NotFound();
            }

            _unitOfWork.Phims.Remove(phim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
