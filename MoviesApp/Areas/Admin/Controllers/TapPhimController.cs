using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;

namespace MoviesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TapPhimController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly ICDNVideoService _cdnVideoService;

        public TapPhimController(WebMoviesDbContext context, ICDNVideoService cdnVideoService)
        {
            _context = context;
            _cdnVideoService = cdnVideoService;
        }// GET: Admin/TapPhim
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
        public async Task<IActionResult> Create([Bind("MaPhim,SoTapThuTu,TenTap,ChiTiet,VideoUrl,VideoId,ThoiLuongTap,NgayPhatHanh")] TapPhim tapPhim)
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
                }

                _context.Add(tapPhim);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm tập phim thành công!";
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
        }        private bool TapPhimExists(string id)
        {
            return _context.TapPhims.Any(e => e.MaTap == id);
        }        // API endpoint để upload video lên CDN
        [HttpPost]
        public async Task<IActionResult> UploadVideoToCDN(IFormFile videoFile, string episodeId, string maTap = null)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file video" });
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };
                var fileExtension = Path.GetExtension(videoFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Định dạng file không được hỗ trợ" });
                }

                // Kiểm tra kích thước file (tối đa 2GB)
                if (videoFile.Length > 2L * 1024 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Kích thước file quá lớn (tối đa 2GB)" });
                }

                // Upload lên CDN
                var result = await _cdnVideoService.UploadVideoAsync(videoFile, episodeId);

                if (result.Success)
                {
                    // Nếu có maTap, cập nhật database ngay
                    if (!string.IsNullOrEmpty(maTap))
                    {
                        var tapPhim = await _context.TapPhims.FindAsync(maTap);
                        if (tapPhim != null)
                        {
                            tapPhim.VideoId = result.VideoId;
                            tapPhim.VideoUrl = result.VideoUrl;
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Json(new { 
                        success = true, 
                        videoUrl = result.VideoUrl,
                        videoId = result.VideoId,
                        message = "Upload video thành công"
                    });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi upload: {ex.Message}" });
            }
        }        // API endpoint để xóa video từ CDN
        [HttpPost]
        public async Task<IActionResult> DeleteVideoFromCDN(string videoUrl)
        {
            try
            {
                // Extract videoId from URL or use URL as videoId
                string videoId = ExtractVideoIdFromUrl(videoUrl);
                var result = await _cdnVideoService.DeleteVideoAsync(videoId);
                
                if (result)
                {
                    return Json(new { success = true, message = "Xóa video thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa video" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi xóa video: {ex.Message}" });
            }
        }

        private string ExtractVideoIdFromUrl(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
                return string.Empty;

            try
            {
                // Extract from URL pattern like: http://localhost:5288/api/v1/videos/{videoId}/stream
                var uri = new Uri(videoUrl);
                var segments = uri.Segments;
                
                // Find "videos" segment and get the next one
                for (int i = 0; i < segments.Length - 1; i++)
                {
                    if (segments[i].TrimEnd('/').Equals("videos", StringComparison.OrdinalIgnoreCase))
                    {
                        return segments[i + 1].TrimEnd('/');
                    }
                }
                
                // Fallback: use filename without extension
                return Path.GetFileNameWithoutExtension(videoUrl);
            }
            catch
            {
                // Fallback: use the URL as is
                return videoUrl;
            }
        }// API endpoint để kiểm tra trạng thái video
        [HttpGet]
        public async Task<IActionResult> CheckVideoStatus(string videoId)
        {
            try
            {
                var result = await _cdnVideoService.CheckVideoStatusAsync(videoId);
                return Json(new { 
                    success = true, 
                    status = result.Status,
                    isReady = result.IsReady,
                    processingProgress = result.ProcessingProgress
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi kiểm tra trạng thái: {ex.Message}" });
            }
        }
    }
}
