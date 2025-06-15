using Microsoft.EntityFrameworkCore;
using MoviesApp.DataAccess.Data;
using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public class NguoiDungRepository : Repository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(WebMoviesDbContext context) : base(context)
        {
        }

        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(nd => nd.Email == email);
        }

        public async Task<IEnumerable<PhimYeuThich>> GetPhimYeuThichByUserIdAsync(string userId)
        {
            return await _context.PhimYeuThichs
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.TheLoaiPhim)
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.QuocGia)
                .Include(pyt => pyt.Phim)
                    .ThenInclude(p => p.DanhMuc)
                .Where(pyt => pyt.MaND == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuXem>> GetLichSuXemByUserIdAsync(string userId)
        {
            return await _context.LichSuXems
                .Include(ls => ls.TapPhim)
                    .ThenInclude(t => t.Phim)
                .Where(ls => ls.MaND == userId)
                .OrderByDescending(ls => ls.ThoiDiemXem)
                .ToListAsync();
        }
    }
}
