using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuocGiaController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuocGiaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuocGia>>> GetAllQuocGia()
        {
            var quocGias = await _unitOfWork.QuocGias.GetAllAsync();
            return Ok(quocGias);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<QuocGia>> GetQuocGia(string id)
        {
            var quocGia = await _unitOfWork.QuocGias.GetByIdAsync(id);
            if (quocGia == null)
            {
                return NotFound();
            }
            return Ok(quocGia);
        }

        [HttpPost]
        public async Task<ActionResult<QuocGia>> CreateQuocGia(QuocGia quocGia)
        {
            await _unitOfWork.QuocGias.AddAsync(quocGia);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetQuocGia), new { id = quocGia.MaQG }, quocGia);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuocGia(string id, QuocGia quocGia)
        {
            if (id != quocGia.MaQG)
            {
                return BadRequest();
            }

            var existingQuocGia = await _unitOfWork.QuocGias.GetByIdAsync(id);
            if (existingQuocGia == null)
            {
                return NotFound();
            }

            _unitOfWork.QuocGias.Update(quocGia);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuocGia(string id)
        {
            var quocGia = await _unitOfWork.QuocGias.GetByIdAsync(id);
            if (quocGia == null)
            {
                return NotFound();
            }

            _unitOfWork.QuocGias.Remove(quocGia);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
