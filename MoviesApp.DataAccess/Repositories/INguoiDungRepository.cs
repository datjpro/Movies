using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public interface INguoiDungRepository : IRepository<NguoiDung>
    {
        Task<NguoiDung?> GetByEmailAsync(string email);
        Task<IEnumerable<PhimYeuThich>> GetPhimYeuThichByUserIdAsync(string userId);
        Task<IEnumerable<LichSuXem>> GetLichSuXemByUserIdAsync(string userId);
    }
}
