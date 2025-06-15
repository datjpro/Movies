using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public interface IPhimRepository : IRepository<Phim>
    {
        Task<IEnumerable<Phim>> GetPhimsByTheLoaiAsync(string theLoaiId);
        Task<IEnumerable<Phim>> GetPhimsByQuocGiaAsync(string quocGiaId);
        Task<IEnumerable<Phim>> GetPhimsByDanhMucAsync(string danhMucId);
        Task<IEnumerable<Phim>> SearchPhimAsync(string searchTerm);
        Task<Phim?> GetPhimWithDetailsAsync(string id);
    }
}
