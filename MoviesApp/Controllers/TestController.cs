using Microsoft.AspNetCore.Mvc;
using MoviesApp.Data;
using MoviesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MoviesApp.Controllers
{
    public class TestController : Controller
    {
        private readonly WebMoviesDbContext _context;

        public TestController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: Test/VideoData
        public async Task<IActionResult> VideoData()
        {
            try
            {
                var tapPhims = await _context.TapPhim
                    .Include(t => t.Phim)
                    .Select(t => new {
                        t.MaTap,
                        t.MaPhim,
                        PhimName = t.Phim.TenPhim,
                        t.SoTapThuTu,
                        t.TenTap,
                        t.VideoUrl,
                        HasVideo = !string.IsNullOrEmpty(t.VideoUrl),
                        VideoType = string.IsNullOrEmpty(t.VideoUrl) ? "None" :
                                   t.VideoUrl.Contains("youtube") ? "YouTube" :
                                   t.VideoUrl.Contains("vimeo") ? "Vimeo" :
                                   t.VideoUrl.Contains(".mp4") || t.VideoUrl.Contains(".webm") ? "Direct" : "Unknown"
                    })
                    .OrderBy(t => t.MaPhim)
                    .ThenBy(t => t.SoTapThuTu)
                    .ToListAsync();

                ViewBag.TotalEpisodes = tapPhims.Count;
                ViewBag.EpisodesWithVideo = tapPhims.Count(t => t.HasVideo);
                ViewBag.EpisodesWithoutVideo = tapPhims.Count(t => !t.HasVideo);

                return View(tapPhims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<object>());
            }
        }

        // POST: Test/AddTestVideo
        [HttpPost]
        public async Task<IActionResult> AddTestVideo(string maPhim, int soTapThuTu, string videoUrl)
        {
            try
            {
                // Tìm phim đầu tiên nếu không có maPhim
                if (string.IsNullOrEmpty(maPhim))
                {
                    var firstPhim = await _context.Phim.FirstOrDefaultAsync();
                    if (firstPhim != null)
                    {
                        maPhim = firstPhim.MaPhim;
                    }
                    else
                    {
                        TempData["Error"] = "Không tìm thấy phim nào trong database";
                        return RedirectToAction("VideoData");
                    }
                }

                // Kiểm tra xem tập phim đã tồn tại chưa
                var existingTap = await _context.TapPhim
                    .FirstOrDefaultAsync(t => t.MaPhim == maPhim && t.SoTapThuTu == soTapThuTu);

                if (existingTap != null)
                {
                    // Cập nhật video URL
                    existingTap.VideoUrl = videoUrl;
                    _context.Update(existingTap);
                }
                else
                {
                    // Tạo tập phim mới
                    var newTap = new TapPhim
                    {
                        MaTap = $"T{DateTime.Now:yyyyMMddHHmmss}",
                        MaPhim = maPhim,
                        SoTapThuTu = soTapThuTu,
                        TenTap = $"Tập {soTapThuTu} - Test Video",
                        VideoUrl = videoUrl,
                        ThoiLuongTap = 45,
                        NgayPhatHanh = DateTime.Now,
                        ChiTiet = "Tập phim test để kiểm tra video player"
                    };
                    _context.TapPhim.Add(newTap);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm/cập nhật tập {soTapThuTu} với video: {videoUrl}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("VideoData");
        }

        // GET: Test/WatchVideo/phimId/episodeNumber
        public async Task<IActionResult> WatchVideo(string phimId, int episodeNumber = 1)
        {
            try
            {
                // Nếu không có phimId, lấy phim đầu tiên có tập phim với video
                if (string.IsNullOrEmpty(phimId))
                {
                    var phimWithVideo = await _context.TapPhim
                        .Where(t => !string.IsNullOrEmpty(t.VideoUrl))
                        .Include(t => t.Phim)
                        .Select(t => t.Phim)
                        .FirstOrDefaultAsync();
                    
                    if (phimWithVideo != null)
                    {
                        return RedirectToAction("Details", "Phim", new { id = phimWithVideo.MaPhim, episode = episodeNumber });
                    }
                    else
                    {
                        TempData["Error"] = "Không tìm thấy phim nào có video";
                        return RedirectToAction("VideoData");
                    }
                }
                else
                {
                    return RedirectToAction("Details", "Phim", new { id = phimId, episode = episodeNumber });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("VideoData");
            }
        }
    }
}
