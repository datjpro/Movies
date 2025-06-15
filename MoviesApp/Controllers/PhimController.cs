using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;

namespace MoviesApp.Controllers
{
    public class PhimController : Controller
    {
        private readonly WebMoviesDbContext _context;

        public PhimController(WebMoviesDbContext context)
        {
            _context = context;
        }

        // GET: Phim
        public async Task<IActionResult> Index()
        {
            var phims = await _context.Phims
                .Include(p => p.QuocGia)
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.DanhMuc)
                .Include(p => p.ThongKePhim)
                .ToListAsync();
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
        }

        // GET: Phim/Create
        public IActionResult Create()
        {
            ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View();
        }

        // POST: Phim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhim,MaQG,MaTL,MaDM,TenPhim,MoTaPhim,AnhPhim,SoTap,TinhTrang,NamPhatHanh,ThoiLuongPhim")] Phim phim)
        {
            if (ModelState.IsValid)
            {
                phim.NgayTao = DateTime.Now;
                _context.Add(phim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.QuocGias = _context.QuocGias.ToList();
            ViewBag.TheLoaiPhims = _context.TheLoaiPhims.ToList();
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(phim);
        }
    }
}
