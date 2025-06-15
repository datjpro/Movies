using Microsoft.AspNetCore.Mvc;
using MoviesApp.DataAccess.Models;
using MoviesApp.DataAccess.Repositories;

namespace MoviesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhMucController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DanhMucController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhMuc>>> GetAllDanhMuc()
        {
            var danhMucs = await _unitOfWork.DanhMucs.GetAllAsync();
            return Ok(danhMucs);
        }        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMuc>> GetDanhMuc(string id)
        {
            var danhMuc = await _unitOfWork.DanhMucs.GetByIdAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return Ok(danhMuc);
        }

        [HttpPost]
        public async Task<ActionResult<DanhMuc>> CreateDanhMuc(DanhMuc danhMuc)
        {
            await _unitOfWork.DanhMucs.AddAsync(danhMuc);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetDanhMuc), new { id = danhMuc.MaDM }, danhMuc);
        }        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDanhMuc(string id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDM)
            {
                return BadRequest();
            }

            var existingDanhMuc = await _unitOfWork.DanhMucs.GetByIdAsync(id);
            if (existingDanhMuc == null)
            {
                return NotFound();
            }

            _unitOfWork.DanhMucs.Update(danhMuc);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhMuc(string id)
        {
            var danhMuc = await _unitOfWork.DanhMucs.GetByIdAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            _unitOfWork.DanhMucs.Remove(danhMuc);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
