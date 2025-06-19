using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TapPhimController : Controller
    {
        private readonly WebMoviesDbContext _context;

        public TapPhimController(WebMoviesDbContext context)
        {
            _context = context;
        }        // GET: Admin/TapPhim
        public async Task<IActionResult> Index(string phimId, string maPhim, int page = 1, int pageSize = 10)
        {
            // Support both phimId and maPhim parameters
            string movieId = phimId ?? maPhim;
            
            if (string.IsNullOrEmpty(movieId))
            {
                return RedirectToAction("Index", "Movies");
            }

            var phim = await _context.Phims.FindAsync(movieId);
            if (phim == null)
            {
                return NotFound();
            }

            ViewBag.Phim = phim;
            ViewBag.PhimId = movieId;

            var query = _context.TapPhims
                .Where(t => t.MaPhim == movieId)
                .Include(t => t.Phim)
                .OrderBy(t => t.SoTapThuTu);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var tapPhims = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(tapPhims);
        }        // GET: Admin/TapPhim/Create
        public async Task<IActionResult> Create(string phimId, string maPhim)
        {
            // Support both phimId and maPhim parameters
            string movieId = phimId ?? maPhim;
            
            if (string.IsNullOrEmpty(movieId))
            {
                return RedirectToAction("Index", "Movies");
            }

            var phim = await _context.Phims.FindAsync(movieId);
            if (phim == null)
            {
                return NotFound();
            }

            // Get next episode number
            var lastEpisode = await _context.TapPhims
                .Where(t => t.MaPhim == movieId)
                .OrderByDescending(t => t.SoTapThuTu)
                .FirstOrDefaultAsync();

            var nextEpisodeNumber = (lastEpisode?.SoTapThuTu ?? 0) + 1;

            var tapPhim = new TapPhim
            {
                MaPhim = movieId,
                SoTapThuTu = nextEpisodeNumber,
                NgayPhatHanh = DateTime.Now
            };

            ViewBag.Phim = phim;
            return View(tapPhim);
        }        // POST: Admin/TapPhim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhim,SoTapThuTu,TenTap,ChiTiet,VideoUrl,ThoiLuongTap,NgayPhatHanh")] TapPhim tapPhim)
        {
            // Remove navigation properties from ModelState validation
            ModelState.Remove("Phim");
            ModelState.Remove("BinhLuans");
            ModelState.Remove("LichSuXems");
            ModelState.Remove("DanhGias");
            ModelState.Remove("ThongKeTapPhim");
            
            if (ModelState.IsValid)
            {
                // Generate MaTap
                var lastTap = await _context.TapPhims
                    .OrderByDescending(t => t.MaTap)
                    .FirstOrDefaultAsync();

                int nextId = 1;
                if (lastTap != null && lastTap.MaTap.StartsWith("T"))
                {
                    if (int.TryParse(lastTap.MaTap.Substring(1), out int currentId))
                    {
                        nextId = currentId + 1;
                    }
                }

                tapPhim.MaTap = "T" + nextId.ToString("D3");

                // Check if episode number already exists for this movie
                var existingEpisode = await _context.TapPhims
                    .FirstOrDefaultAsync(t => t.MaPhim == tapPhim.MaPhim && t.SoTapThuTu == tapPhim.SoTapThuTu);

                if (existingEpisode != null)
                {
                    ModelState.AddModelError("SoTapThuTu", "Số tập này đã tồn tại cho phim này.");
                    var phim = await _context.Phims.FindAsync(tapPhim.MaPhim);
                    ViewBag.Phim = phim;
                    return View(tapPhim);
                }                _context.Add(tapPhim);
                await _context.SaveChangesAsync();                TempData["Success"] = "Thêm tập phim thành công!";
                return RedirectToAction(nameof(Index), new { maPhim = tapPhim.MaPhim });
            }

            // Debug ModelState errors  
            foreach (var modelError in ModelState)
            {
                foreach (var error in modelError.Value.Errors)
                {
                    // Log hoặc debug lỗi ở đây
                    Console.WriteLine($"ModelState Error: {modelError.Key} - {error.ErrorMessage}");
                }
            }

            var phimModel = await _context.Phims.FindAsync(tapPhim.MaPhim);
            ViewBag.Phim = phimModel;
            return View(tapPhim);
        }

        // GET: Admin/TapPhim/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tapPhim = await _context.TapPhims
                .Include(t => t.Phim)
                .FirstOrDefaultAsync(t => t.MaTap == id);

            if (tapPhim == null)
            {
                return NotFound();
            }

            ViewBag.Phim = tapPhim.Phim;
            return View(tapPhim);
        }

        // POST: Admin/TapPhim/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTap,MaPhim,SoTapThuTu,TenTap,ChiTiet,VideoUrl,ThoiLuongTap,NgayPhatHanh")] TapPhim tapPhim)
        {
            if (id != tapPhim.MaTap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if episode number already exists for this movie (excluding current episode)
                    var existingEpisode = await _context.TapPhims
                        .FirstOrDefaultAsync(t => t.MaPhim == tapPhim.MaPhim && t.SoTapThuTu == tapPhim.SoTapThuTu && t.MaTap != tapPhim.MaTap);

                    if (existingEpisode != null)
                    {
                        ModelState.AddModelError("SoTapThuTu", "Số tập này đã tồn tại cho phim này.");
                        var phim = await _context.Phims.FindAsync(tapPhim.MaPhim);
                        ViewBag.Phim = phim;
                        return View(tapPhim);
                    }

                    _context.Update(tapPhim);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật tập phim thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TapPhimExists(tapPhim.MaTap))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { maPhim = tapPhim.MaPhim });
            }

            var phimModel = await _context.Phims.FindAsync(tapPhim.MaPhim);
            ViewBag.Phim = phimModel;
            return View(tapPhim);
        }

        // GET: Admin/TapPhim/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tapPhim = await _context.TapPhims
                .Include(t => t.Phim)
                .FirstOrDefaultAsync(t => t.MaTap == id);

            if (tapPhim == null)
            {
                return NotFound();
            }

            return View(tapPhim);
        }

        // POST: Admin/TapPhim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tapPhim = await _context.TapPhims.FindAsync(id);
            if (tapPhim != null)
            {
                var phimId = tapPhim.MaPhim;
                _context.TapPhims.Remove(tapPhim);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa tập phim thành công!";
                return RedirectToAction(nameof(Index), new { maPhim = phimId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TapPhimExists(string id)
        {
            return _context.TapPhims.Any(e => e.MaTap == id);
        }
    }
}
