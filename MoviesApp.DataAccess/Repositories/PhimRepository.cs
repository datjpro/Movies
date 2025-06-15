using Microsoft.EntityFrameworkCore;
using MoviesApp.DataAccess.Data;
using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public class PhimRepository : Repository<Phim>, IPhimRepository
    {
        public PhimRepository(WebMoviesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Phim>> GetPhimsByTheLoaiAsync(string theLoaiId)
        {
            return await _dbSet
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Where(p => p.MaTL == theLoaiId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Phim>> GetPhimsByQuocGiaAsync(string quocGiaId)
        {
            return await _dbSet
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Where(p => p.MaQG == quocGiaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Phim>> GetPhimsByDanhMucAsync(string danhMucId)
        {
            return await _dbSet
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Where(p => p.MaDM == danhMucId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Phim>> SearchPhimAsync(string searchTerm)
        {
            return await _dbSet
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Where(p => p.TenPhim.Contains(searchTerm) || 
                           (p.MoTaPhim != null && p.MoTaPhim.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Phim?> GetPhimWithDetailsAsync(string id)
        {
            return await _dbSet
                .Include(p => p.TheLoaiPhim)
                .Include(p => p.QuocGia)
                .Include(p => p.DanhMuc)
                .Include(p => p.TapPhims)
                .Include(p => p.ThongKePhim)
                .FirstOrDefaultAsync(p => p.MaPhim == id);
        }
    }
}
