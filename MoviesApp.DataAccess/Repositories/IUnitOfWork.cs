using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPhimRepository Phims { get; }
        ITapPhimRepository TapPhims { get; }
        INguoiDungRepository NguoiDungs { get; }
        IRepository<TheLoaiPhim> TheLoaiPhims { get; }
        IRepository<QuocGia> QuocGias { get; }
        IRepository<DanhMuc> DanhMucs { get; }
        IRepository<BinhLuan> BinhLuans { get; }
        IRepository<DanhGia> DanhGias { get; }
        IRepository<PhimYeuThich> PhimYeuThichs { get; }
        IRepository<LichSuXem> LichSuXems { get; }
        IRepository<ThongKePhim> ThongKePhims { get; }
        IRepository<ThongKeTapPhim> ThongKeTapPhims { get; }

        Task<int> SaveAsync();
    }
}
