using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheLoaiPhimController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TheLoaiPhimController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheLoaiPhim>>> GetAllTheLoaiPhim()
        {
            var theLoaiPhims = await _unitOfWork.TheLoaiPhims.GetAllAsync();
            return Ok(theLoaiPhims);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<TheLoaiPhim>> GetTheLoaiPhim(string id)
        {
            var theLoaiPhim = await _unitOfWork.TheLoaiPhims.GetByIdAsync(id);
            if (theLoaiPhim == null)
            {
                return NotFound();
            }
            return Ok(theLoaiPhim);
        }

        [HttpPost]
        public async Task<ActionResult<TheLoaiPhim>> CreateTheLoaiPhim(TheLoaiPhim theLoaiPhim)
        {
            await _unitOfWork.TheLoaiPhims.AddAsync(theLoaiPhim);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetTheLoaiPhim), new { id = theLoaiPhim.MaTL }, theLoaiPhim);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheLoaiPhim(string id, TheLoaiPhim theLoaiPhim)
        {
            if (id != theLoaiPhim.MaTL)
            {
                return BadRequest();
            }

            var existingTheLoaiPhim = await _unitOfWork.TheLoaiPhims.GetByIdAsync(id);
            if (existingTheLoaiPhim == null)
            {
                return NotFound();
            }

            _unitOfWork.TheLoaiPhims.Update(theLoaiPhim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheLoaiPhim(string id)
        {
            var theLoaiPhim = await _unitOfWork.TheLoaiPhims.GetByIdAsync(id);
            if (theLoaiPhim == null)
            {
                return NotFound();
            }

            _unitOfWork.TheLoaiPhims.Remove(theLoaiPhim);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
