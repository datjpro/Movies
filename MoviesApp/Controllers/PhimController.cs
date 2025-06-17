using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoviesApp.Controllers
{
    public class PhimController : Controller
    {
        private readonly WebMoviesDbContext _context;
        private readonly OMDbService _omdbService;

        public PhimController(WebMoviesDbContext context, OMDbService omdbService)
        {
            _context = context;
            _omdbService = omdbService;
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
        }

        // GET: Phim/Details/5
        public async Task<IActionResult> Details(string id)
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
                .Include(p => p.TapPhims)
                .FirstOrDefaultAsync(m => m.MaPhim == id);

            if (phim == null)
            {
                return NotFound();
            }

            return View(phim);
        }        // GET: Phim/Create - Chỉ cho phép Admin
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phim);
                    await _context.SaveChangesAsync();
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

            return RedirectToAction(nameof(Index));
        }
    }
}
