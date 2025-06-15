using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TapPhimController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TapPhimController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TapPhim>>> GetAllTapPhim()
        {
            var tapPhims = await _unitOfWork.TapPhims.GetAllAsync();
            return Ok(tapPhims);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<TapPhim>> GetTapPhim(string id)
        {
            var tapPhim = await _unitOfWork.TapPhims.GetTapWithDetailsAsync(id);
            if (tapPhim == null)
            {
                return NotFound();
            }
            return Ok(tapPhim);
        }

        [HttpGet("phim/{phimId}")]
        public async Task<ActionResult<IEnumerable<TapPhim>>> GetTapsByPhimId(string phimId)
        {
            var tapPhims = await _unitOfWork.TapPhims.GetTapsByPhimIdAsync(phimId);
            return Ok(tapPhims);
        }

        [HttpPost]
        public async Task<ActionResult<TapPhim>> CreateTapPhim(TapPhim tapPhim)
        {
            await _unitOfWork.TapPhims.AddAsync(tapPhim);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetTapPhim), new { id = tapPhim.MaTap }, tapPhim);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTapPhim(string id, TapPhim tapPhim)
        {
            if (id != tapPhim.MaTap)
            {
                return BadRequest();
            }

            var existingTapPhim = await _unitOfWork.TapPhims.GetByIdAsync(id);
            if (existingTapPhim == null)
            {
                return NotFound();
            }

            _unitOfWork.TapPhims.Update(tapPhim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTapPhim(string id)
        {
            var tapPhim = await _unitOfWork.TapPhims.GetByIdAsync(id);
            if (tapPhim == null)
            {
                return NotFound();
            }

            _unitOfWork.TapPhims.Remove(tapPhim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
