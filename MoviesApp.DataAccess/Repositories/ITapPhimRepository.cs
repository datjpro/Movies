using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public interface ITapPhimRepository : IRepository<TapPhim>
    {
        Task<IEnumerable<TapPhim>> GetTapsByPhimIdAsync(string phimId);
        Task<TapPhim?> GetTapWithDetailsAsync(string tapId);
    }
}
