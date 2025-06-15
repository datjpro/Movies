using Microsoft.EntityFrameworkCore;
using MoviesApp.DataAccess.Data;
using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public class TapPhimRepository : Repository<TapPhim>, ITapPhimRepository
    {
        public TapPhimRepository(WebMoviesDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TapPhim>> GetTapsByPhimIdAsync(string phimId)
        {
            return await _dbSet
                .Include(t => t.Phim)
                .Include(t => t.ThongKeTapPhim)
                .Where(t => t.MaPhim == phimId)
                .OrderBy(t => t.SoTapThuTu)
                .ToListAsync();
        }

        public async Task<TapPhim?> GetTapWithDetailsAsync(string tapId)
        {
            return await _dbSet
                .Include(t => t.Phim)
                .Include(t => t.ThongKeTapPhim)
                .Include(t => t.BinhLuans)
                    .ThenInclude(bl => bl.NguoiDung)
                .Include(t => t.DanhGias)
                    .ThenInclude(dg => dg.NguoiDung)
                .FirstOrDefaultAsync(t => t.MaTap == tapId);
        }
    }
}
