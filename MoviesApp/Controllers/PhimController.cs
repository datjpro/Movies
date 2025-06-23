using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoviesApp.Controllers
{    public class PhimController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly OMDbService _omdbService;
        private readonly ICDNVideoService _cdnVideoService;

        public PhimController(WebMoviesDbContext context, OMDbService omdbService, ICDNVideoService cdnVideoService)
        {
            _context = context;
            _omdbService = omdbService;
            _cdnVideoService = cdnVideoService;
        }

        // GET: Phim - Công khai cho tất cả users
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string search = "", string quocGia = "", string theLoai = "", string danhMuc = "")
        {
            var query = _context.Phims
                .Include(p => p.QuocGia)
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.DanhMuc)
                .Include(p => p.ThongKePhim)
                .AsQueryable();

            // Tìm kiếm theo tên phim
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenPhim.Contains(search));
            }

            // Lọc theo quốc gia
            if (!string.IsNullOrEmpty(quocGia))
            {
                query = query.Where(p => p.MaQG == quocGia);
            }

            // Lọc theo thể loại
            if (!string.IsNullOrEmpty(theLoai))
            {
                query = query.Where(p => p.MaTL == theLoai);
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(danhMuc))
            {
                query = query.Where(p => p.MaDM == danhMuc);
            }

            // Phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var phims = await query
                .OrderByDescending(p => p.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();            // Dữ liệu cho dropdown filters
            var quocGias = await _context.QuocGias.ToListAsync();
            var theLoaiPhims = await _context.TheLoaiPhims.ToListAsync();
            var danhMucs = await _context.DanhMucs.ToListAsync();            ViewBag.QuocGiaList = quocGias.Select(qg => new SelectListItem
            {
                Value = qg.MaQG,
                Text = qg.TenQG,
                Selected = qg.MaQG == quocGia
            }).ToList();

            ViewBag.TheLoaiList = theLoaiPhims.Select(tl => new SelectListItem
            {
                Value = tl.MaTL,
                Text = tl.TenTL,
                Selected = tl.MaTL == theLoai
            }).ToList();

            ViewBag.DanhMucList = danhMucs.Select(dm => new SelectListItem
            {
                Value = dm.MaDM,
                Text = dm.TenDM,
                Selected = dm.MaDM == danhMuc
            }).ToList();

            // Thông tin phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.QuocGia = quocGia;
            ViewBag.TheLoai = theLoai;
            ViewBag.DanhMuc = danhMuc;

            return View(phims);
        }        // GET: Phim/Details/5 hoặc Phim/Details/5/episode/1
        public async Task<IActionResult> Details(string id, int? episode)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims
                .Include(p => p.QuocGia)
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.DanhMuc)
                .Include(p => p.ThongKePhim)
                .Include(p => p.TapPhims.OrderBy(t => t.SoTapThuTu))
                .FirstOrDefaultAsync(m => m.MaPhim == id);

            if (phim == null)
            {
                return NotFound();
            }            // Tìm tập phim được chọn
            TapPhim? selectedEpisode = null;
            if (episode.HasValue)
            {
                // Cho phép xem cả tập 0 (trailer) và các tập khác
                selectedEpisode = phim.TapPhims.FirstOrDefault(t => t.SoTapThuTu == episode.Value);
                if (selectedEpisode == null)
                {
                    // Log để debug
                    Console.WriteLine($"Không tìm thấy tập {episode.Value} cho phim {id}");
                    Console.WriteLine($"Các tập có sẵn: {string.Join(", ", phim.TapPhims.Select(t => t.SoTapThuTu))}");
                    
                    // Nếu không tìm thấy tập được yêu cầu, redirect về trang chính
                    return RedirectToAction("Details", new { id });
                }
                else
                {
                    Console.WriteLine($"Tìm thấy tập {selectedEpisode.SoTapThuTu} với VideoUrl: {selectedEpisode.VideoUrl}");
                }
            }            else if (phim.TapPhims.Any())
            {
                // Nếu không chỉ định tập nào, ưu tiên hiển thị tập 0 (trailer), nếu không có thì tập 1, cuối cùng mới là tập đầu tiên
                selectedEpisode = phim.TapPhims.FirstOrDefault(t => t.SoTapThuTu == 0) // Ưu tiên trailer
                                ?? phim.TapPhims.FirstOrDefault(t => t.SoTapThuTu == 1) // Sau đó đến tập 1
                                ?? phim.TapPhims.OrderBy(t => t.SoTapThuTu).FirstOrDefault(); // Cuối cùng tập đầu tiên
                if (selectedEpisode != null)
                {
                    episode = selectedEpisode.SoTapThuTu;
                    Console.WriteLine($"Tự động chọn tập {selectedEpisode.SoTapThuTu} ({(selectedEpisode.SoTapThuTu == 0 ? "Trailer" : "Tập " + selectedEpisode.SoTapThuTu)})");
                }
            }ViewBag.SelectedEpisode = selectedEpisode;
            ViewBag.EpisodeNumber = episode;
            
            // Xử lý video URL - Detect YouTube hoặc MP4 từ CDN
            if (selectedEpisode != null)
            {
                // Kiểm tra nếu có VideoId từ CDN
                if (!string.IsNullOrEmpty(selectedEpisode.VideoId))
                {
                    ViewBag.MP4Url = $"http://localhost:5288/api/v1/videos/{selectedEpisode.VideoId}/mp4";
                    ViewBag.HasVideo = true;
                    ViewBag.VideoType = "MP4";
                    
                    Console.WriteLine($"DEBUG - CDN Video - Episode: {selectedEpisode.TenTap}");
                    Console.WriteLine($"DEBUG - VideoId: {selectedEpisode.VideoId}");
                    Console.WriteLine($"DEBUG - MP4 URL: {ViewBag.MP4Url}");
                }
                // Kiểm tra nếu có VideoUrl từ YouTube
                else if (!string.IsNullOrEmpty(selectedEpisode.VideoUrl))
                {
                    if (selectedEpisode.VideoUrl.Contains("youtube.com") || selectedEpisode.VideoUrl.Contains("youtu.be"))
                    {
                        // Convert YouTube URL to embed format
                        var youtubeUrl = selectedEpisode.VideoUrl;
                        string videoId = "";
                        
                        if (youtubeUrl.Contains("watch?v="))
                        {
                            videoId = youtubeUrl.Split("watch?v=")[1].Split("&")[0];
                        }
                        else if (youtubeUrl.Contains("youtu.be/"))
                        {
                            videoId = youtubeUrl.Split("youtu.be/")[1].Split("?")[0];
                        }
                        
                        if (!string.IsNullOrEmpty(videoId))
                        {
                            ViewBag.YouTubeEmbedUrl = $"https://www.youtube.com/embed/{videoId}";
                            ViewBag.HasVideo = true;
                            ViewBag.VideoType = "YOUTUBE";
                            
                            Console.WriteLine($"DEBUG - YouTube Video - Episode: {selectedEpisode.TenTap}");
                            Console.WriteLine($"DEBUG - Original URL: {selectedEpisode.VideoUrl}");
                            Console.WriteLine($"DEBUG - Embed URL: {ViewBag.YouTubeEmbedUrl}");
                        }
                        else
                        {
                            ViewBag.HasVideo = false;
                            ViewBag.VideoType = "ERROR";
                        }
                    }
                    else
                    {
                        // URL khác (có thể là MP4 direct link)
                        ViewBag.MP4Url = selectedEpisode.VideoUrl;
                        ViewBag.HasVideo = true;
                        ViewBag.VideoType = "DIRECT";
                        
                        Console.WriteLine($"DEBUG - Direct Video - Episode: {selectedEpisode.TenTap}");
                        Console.WriteLine($"DEBUG - Direct URL: {selectedEpisode.VideoUrl}");
                    }
                }
                else
                {
                    ViewBag.HasVideo = false;
                    ViewBag.VideoType = "NONE";
                    Console.WriteLine($"DEBUG - No video available. Episode: {selectedEpisode?.TenTap}");
                }
            }
            else
            {
                ViewBag.HasVideo = false;
                ViewBag.VideoType = "NONE";
            }
            
            return View(phim);
        }// GET: Phim/Create - Chỉ cho phép Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View();
        }        // POST: Phim/Create - Chỉ cho phép Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("MaPhim,MaQG,MaTL,MaDM,TenPhim,MoTaPhim,AnhPhim,SoTap,TinhTrang,NamPhatHanh,ThoiLuongPhim,BienKich,DaoDien,DiemImdb,DiemMetascore,DienVien,GiaiThuong,ImdbId,LoaiPhim,LuotVoteImdb,NgayKhoiChieu,NgonNgu,QuocGiaSanXuat,TongSoMua,XepHang")] Phim phim)
        {
            // Remove navigation properties from ModelState validation
            ModelState.Remove("QuocGia");
            ModelState.Remove("TheLoaiPhim");
            ModelState.Remove("DanhMuc");
            
            if (ModelState.IsValid)
            {
                phim.NgayTao = DateTime.Now;
                _context.Add(phim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Debug ModelState errors
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
              ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(phim);
        }        // GET: Phim/SearchOMDb - Chỉ cho phép Admin
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchOMDb(string query, string imdbId)
        {
            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(imdbId))
            {
                return Json(new { success = false, message = "Query hoặc IMDb ID không được để trống" });
            }

            try
            {
                OMDbMovieResponse movieData;
                
                // Tìm kiếm theo IMDb ID nếu có, ngược lại tìm theo tên
                if (!string.IsNullOrWhiteSpace(imdbId))
                {
                    movieData = await _omdbService.GetMovieByImdbIdAsync(imdbId);
                }
                else
                {
                    movieData = await _omdbService.GetMovieByTitleAsync(query);
                }
                
                if (movieData != null && movieData.Response == "True")
                {
                    var result = new
                    {
                        success = true,
                        movie = new
                        {
                            title = movieData.Title,
                            year = movieData.Year,
                            director = movieData.Director,
                            actors = movieData.Actors,
                            genre = movieData.Genre,
                            runtime = movieData.Runtime,
                            imdbRating = movieData.imdbRating,
                            plot = movieData.Plot,
                            poster = movieData.Poster,
                            language = movieData.Language,
                            country = movieData.Country,
                            awards = movieData.Awards,
                            imdbID = movieData.imdbID,
                            type = movieData.Type,
                            metascore = movieData.Metascore,
                            imdbVotes = movieData.imdbVotes
                        }
                    };
                    
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy phim" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }        // GET: Phim/Edit/5 - Chỉ cho phép Admin và User
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims.FindAsync(id);
            if (phim == null)
            {
                return NotFound();
            }
            
            ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(phim);
        }        // POST: Phim/Edit/5 - Chỉ cho phép Admin và User
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(string id, [Bind("MaPhim,MaQG,MaTL,MaDM,TenPhim,MoTaPhim,AnhPhim,SoTap,TinhTrang,NamPhatHanh,ThoiLuongPhim,BienKich,DaoDien,DiemImdb,DiemMetascore,DienVien,GiaiThuong,ImdbId,LoaiPhim,LuotVoteImdb,NgayKhoiChieu,NgonNgu,QuocGiaSanXuat,TongSoMua,XepHang,NgayTao")] Phim phim)
        {
            if (id != phim.MaPhim)
            {
                return NotFound();
            }

            // Remove navigation properties from ModelState validation
            ModelState.Remove("QuocGia");
            ModelState.Remove("TheLoaiPhim");
            ModelState.Remove("DanhMuc");
            ModelState.Remove("ThongKePhim");
            ModelState.Remove("TapPhims");
            ModelState.Remove("BinhLuans");
            ModelState.Remove("LichSuXems");
            ModelState.Remove("PhimYeuThichs");
            ModelState.Remove("DanhGias");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy phim gốc từ database để giữ lại các thông tin không thay đổi
                    var existingPhim = await _context.Phims.AsNoTracking().FirstOrDefaultAsync(p => p.MaPhim == id);
                    if (existingPhim == null)
                    {
                        return NotFound();
                    }

                    // Giữ lại NgayTao gốc nếu không được truyền vào
                    if (phim.NgayTao == DateTime.MinValue || phim.NgayTao == default(DateTime))
                    {
                        phim.NgayTao = existingPhim.NgayTao;
                    }

                    _context.Update(phim);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Cập nhật phim thành công!";
                    return RedirectToAction(nameof(Details), new { id = phim.MaPhim });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật phim: " + ex.Message);
                }
            }
            
            // Debug ModelState errors
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            
            ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(phim);
        }        // GET: Phim/Delete/5 - Chỉ cho phép Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims
                .Include(p => p.QuocGia)
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.DanhMuc)
                .Include(p => p.ThongKePhim)
                .FirstOrDefaultAsync(m => m.MaPhim == id);
                
            if (phim == null)
            {
                return NotFound();
            }

            return View(phim);
        }        // POST: Phim/Delete/5 - Chỉ cho phép Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));        }

        // GET: Phim/Watch/movieId/episodeNumber - Alternative clean URL
        public async Task<IActionResult> Watch(string id, int episode = 1)
        {
            return await Details(id, episode);
        }
        
        // Test action để thêm tập 0 (trailer) - chỉ dùng để test
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTrailer(string phimId, string trailerUrl, string trailerTitle = "Trailer")
        {
            try
            {
                var phim = await _context.Phims.FirstOrDefaultAsync(p => p.MaPhim == phimId);
                if (phim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim" });
                }

                // Kiểm tra xem đã có trailer (tập 0) chưa
                var existingTrailer = await _context.TapPhims.FirstOrDefaultAsync(t => t.MaPhim == phimId && t.SoTapThuTu == 0);
                if (existingTrailer != null)
                {
                    // Cập nhật trailer hiện có
                    existingTrailer.VideoUrl = trailerUrl;
                    existingTrailer.TenTap = trailerTitle;
                    _context.Update(existingTrailer);
                }
                else
                {
                    // Tạo trailer mới
                    var trailer = new TapPhim
                    {
                        MaTap = $"T{phimId}_00", // Tạo mã tập duy nhất cho trailer
                        MaPhim = phimId,
                        SoTapThuTu = 0,
                        TenTap = trailerTitle,
                        VideoUrl = trailerUrl,
                        NgayPhatHanh = DateTime.Now
                    };
                    _context.TapPhims.Add(trailer);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm/cập nhật trailer thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Test page for adding trailer
        [Authorize(Roles = "Admin")]
        public IActionResult TestTrailer()
        {
            return View();
        }

        // API để lấy danh sách phim cho test
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMoviesForTest()
        {
            var phims = await _context.Phims
                .Include(p => p.TapPhims)
                .Select(p => new
                {
                    p.MaPhim,
                    p.TenPhim,
                    SoTapPhim = p.TapPhims.Count,
                    CoTrailer = p.TapPhims.Any(t => t.SoTapThuTu == 0)
                })
                .Take(10)
                .ToListAsync();

            return Json(phims);
        }        // GET: Phim/VideoPlayer/5 - Test video player
        public async Task<IActionResult> VideoPlayer(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Return demo video if no ID provided
                return View(new Phim 
                { 
                    TenPhim = "Demo Video Player",
                    MoTa = "Trải nghiệm video player hiện đại với đầy đủ tính năng điều khiển",
                    LinkPhim = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4"
                });
            }

            var phim = await _context.Phims
                .Include(p => p.QuocGia)
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.DanhMuc)
                .FirstOrDefaultAsync(m => m.MaPhim == id);

            if (phim == null)
            {
                return NotFound();
            }

            return View(phim);
        }

        // GET: Phim/XemTapPhim/T001 - Xem tập phim cụ thể
        public async Task<IActionResult> XemTapPhim(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không tìm thấy tập phim");
            }

            var tapPhim = await _context.TapPhims
                .Include(t => t.Phim)
                    .ThenInclude(p => p.QuocGia)
                .Include(t => t.Phim)
                    .ThenInclude(p => p.TheLoaiPhim)
                .Include(t => t.Phim)
                    .ThenInclude(p => p.DanhMuc)
                .FirstOrDefaultAsync(t => t.MaTap == id);

            if (tapPhim == null)
            {
                return NotFound("Không tìm thấy tập phim");
            }

            // Lấy danh sách tất cả tập phim của phim này
            var danhSachTapPhim = await _context.TapPhims
                .Where(t => t.MaPhim == tapPhim.MaPhim)
                .OrderBy(t => t.SoTapThuTu)
                .ToListAsync();

            ViewBag.DanhSachTapPhim = danhSachTapPhim;
            ViewBag.TapPhimHienTai = tapPhim;

            return View(tapPhim);
        }        // API: Lấy URL video từ CDN cho tập phim
        [HttpGet]
        public async Task<IActionResult> GetVideoUrl(string tapPhimId)
        {
            try
            {
                var tapPhim = await _context.TapPhims
                    .FirstOrDefaultAsync(t => t.MaTap == tapPhimId);

                if (tapPhim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tập phim" });
                }

                if (string.IsNullOrEmpty(tapPhim.VideoId))
                {
                    return Json(new { success = false, message = "Tập phim chưa có video" });
                }

                // Gọi CDN API để lấy streaming URL realtime
                var cdnVideoService = HttpContext.RequestServices.GetRequiredService<ICDNVideoService>();
                var streamingUrl = await cdnVideoService.GetStreamingUrlAsync(tapPhim.VideoId);
                
                if (string.IsNullOrEmpty(streamingUrl))
                {
                    return Json(new { 
                        success = false, 
                        message = "Không thể lấy URL video từ CDN",
                        videoId = tapPhim.VideoId
                    });
                }

                return Json(new { 
                    success = true, 
                    videoUrl = streamingUrl,
                    videoId = tapPhim.VideoId,
                    tapPhimInfo = new {
                        maTap = tapPhim.MaTap,
                        tenTap = tapPhim.TenTap,
                        soTapThuTu = tapPhim.SoTapThuTu
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lấy video: " + ex.Message });
            }
        }

        // Test action để upload video và tạo tập phim mới
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadVideoForEpisode(string phimId, int soTap, string tenTap, IFormFile videoFile)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file video" });
                }

                var phim = await _context.Phims.FirstOrDefaultAsync(p => p.MaPhim == phimId);
                if (phim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim" });
                }                // Upload video lên CDN
                var cdnVideoService = HttpContext.RequestServices.GetRequiredService<ICDNVideoService>();
                var episodeId = $"episode_{phimId}_{soTap}_{DateTime.Now.Ticks}";
                
                var uploadResult = await cdnVideoService.UploadVideoAsync(videoFile, episodeId);
                
                if (!uploadResult.Success)
                {
                    return Json(new { success = false, message = "Lỗi upload video: " + uploadResult.Message });
                }

                // Tạo hoặc cập nhật tập phim
                var maTap = $"{phimId}_T{soTap:D2}";
                var existingTap = await _context.TapPhims.FirstOrDefaultAsync(t => t.MaTap == maTap);                if (existingTap != null)
                {
                    // Cập nhật tập phim hiện có - chỉ lưu VideoId
                    existingTap.VideoId = uploadResult.VideoId; 
                    existingTap.VideoStatus = "processing"; 
                    existingTap.TenTap = tenTap;
                    _context.Update(existingTap);
                }
                else
                {
                    // Tạo tập phim mới - chỉ lưu VideoId
                    var tapPhim = new TapPhim
                    {
                        MaTap = maTap,
                        MaPhim = phimId,
                        SoTapThuTu = soTap,
                        TenTap = tenTap,
                        VideoId = uploadResult.VideoId, // Chỉ lưu VideoId
                        VideoStatus = "processing",
                        NgayPhatHanh = DateTime.Now
                    };
                    _context.TapPhims.Add(tapPhim);
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Upload video thành công!", 
                    videoUrl = uploadResult.VideoUrl,
                    tapPhimId = maTap
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Test page for uploading videos
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TestUploadVideo()
        {
            var phims = await _context.Phims.Take(10).ToListAsync();
            ViewBag.Phims = phims;
            return View();
        }        // API: Tạo tập phim test với VideoId giả lập
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTestEpisode(string phimId, int soTap = 1, string tenTap = "Tập test")
        {
            try
            {
                var phim = await _context.Phims.FirstOrDefaultAsync(p => p.MaPhim == phimId);
                if (phim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim" });
                }

                // Tạo VideoId giả lập cho test
                var testVideoId = $"test_episode_{phimId}_{soTap}_{DateTime.Now.Ticks}";

                // Tạo hoặc cập nhật tập phim
                var maTap = $"{phimId}_T{soTap:D2}";
                var existingTap = await _context.TapPhims.FirstOrDefaultAsync(t => t.MaTap == maTap);
                
                if (existingTap != null)
                {
                    // Cập nhật tập phim hiện có
                    existingTap.VideoId = testVideoId;
                    existingTap.VideoStatus = "ready";
                    existingTap.TenTap = tenTap;
                    _context.Update(existingTap);
                }
                else
                {
                    // Tạo tập phim mới
                    var tapPhim = new TapPhim
                    {
                        MaTap = maTap,
                        MaPhim = phimId,
                        SoTapThuTu = soTap,
                        TenTap = tenTap,
                        VideoId = testVideoId, // Chỉ lưu VideoId
                        VideoStatus = "ready",
                        ThoiLuongTap = 45,
                        NgayPhatHanh = DateTime.Now
                    };
                    _context.TapPhims.Add(tapPhim);
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Tạo tập phim test thành công!", 
                    videoId = testVideoId,
                    tapPhimId = maTap,
                    note = "Khi xem sẽ gọi CDN API để lấy streaming URL"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
