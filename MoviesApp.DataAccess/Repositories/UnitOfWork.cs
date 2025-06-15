using MoviesApp.DataAccess.Data;
using MoviesApp.DataAccess.Models;

namespace MoviesApp.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebMoviesDbContext _context;
        private IPhimRepository? _phims;
        private ITapPhimRepository? _tapPhims;
        private INguoiDungRepository? _nguoiDungs;
        private IRepository<TheLoaiPhim>? _theLoaiPhims;
        private IRepository<QuocGia>? _quocGias;
        private IRepository<DanhMuc>? _danhMucs;
        private IRepository<BinhLuan>? _binhLuans;
        private IRepository<DanhGia>? _danhGias;
        private IRepository<PhimYeuThich>? _phimYeuThichs;
        private IRepository<LichSuXem>? _lichSuXems;
        private IRepository<ThongKePhim>? _thongKePhims;
        private IRepository<ThongKeTapPhim>? _thongKeTapPhims;

        public UnitOfWork(WebMoviesDbContext context)
        {
            _context = context;
        }

        public IPhimRepository Phims => _phims ??= new PhimRepository(_context);
        public ITapPhimRepository TapPhims => _tapPhims ??= new TapPhimRepository(_context);
        public INguoiDungRepository NguoiDungs => _nguoiDungs ??= new NguoiDungRepository(_context);
        public IRepository<TheLoaiPhim> TheLoaiPhims => _theLoaiPhims ??= new Repository<TheLoaiPhim>(_context);
        public IRepository<QuocGia> QuocGias => _quocGias ??= new Repository<QuocGia>(_context);
        public IRepository<DanhMuc> DanhMucs => _danhMucs ??= new Repository<DanhMuc>(_context);
        public IRepository<BinhLuan> BinhLuans => _binhLuans ??= new Repository<BinhLuan>(_context);
        public IRepository<DanhGia> DanhGias => _danhGias ??= new Repository<DanhGia>(_context);
        public IRepository<PhimYeuThich> PhimYeuThichs => _phimYeuThichs ??= new Repository<PhimYeuThich>(_context);
        public IRepository<LichSuXem> LichSuXems => _lichSuXems ??= new Repository<LichSuXem>(_context);
        public IRepository<ThongKePhim> ThongKePhims => _thongKePhims ??= new Repository<ThongKePhim>(_context);
        public IRepository<ThongKeTapPhim> ThongKeTapPhims => _thongKeTapPhims ??= new Repository<ThongKeTapPhim>(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
