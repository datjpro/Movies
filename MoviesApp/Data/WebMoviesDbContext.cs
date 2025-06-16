using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Models;

namespace MoviesApp.Data
{    public class WebMoviesDbContext : IdentityDbContext<ApplicationUser>
    {
        public WebMoviesDbContext(DbContextOptions<WebMoviesDbContext> options) : base(options)
        {
        }        // DbSets
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<QuocGia> QuocGias { get; set; }
        public DbSet<TheLoaiPhim> TheLoaiPhims { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<LoaiNguoiDung> LoaiNguoiDungs { get; set; }
        public DbSet<Phim> Phims { get; set; }
        public DbSet<TapPhim> TapPhims { get; set; }
        public DbSet<ThongKePhim> ThongKePhims { get; set; }
        public DbSet<ThongKeTapPhim> ThongKeTapPhims { get; set; }
        public DbSet<BinhLuan> BinhLuans { get; set; }
        public DbSet<DangKy> DangKys { get; set; }
        public DbSet<LichSuXem> LichSuXems { get; set; }
        public DbSet<PhimYeuThich> PhimYeuThichs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }

        // Compatibility properties (singular names)
        public DbSet<Phim> Phim => Set<Phim>();
        public DbSet<QuocGia> QuocGia => Set<QuocGia>();
        public DbSet<TheLoaiPhim> TheLoaiPhim => Set<TheLoaiPhim>();
        public DbSet<DanhMuc> DanhMuc => Set<DanhMuc>();
        public DbSet<TapPhim> TapPhim => Set<TapPhim>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<TapPhim>()
                .HasIndex(e => new { e.MaPhim, e.SoTapThuTu })
                .IsUnique();

            modelBuilder.Entity<PhimYeuThich>()
                .HasIndex(e => new { e.MaND, e.MaPhim })
                .IsUnique();

            modelBuilder.Entity<DanhGia>()
                .HasIndex(e => new { e.MaTap, e.MaND })
                .IsUnique();

            // Indexes for performance optimization
            modelBuilder.Entity<Phim>()
                .HasIndex(e => e.TenPhim)
                .HasDatabaseName("IX_Phim_TenPhim");

            modelBuilder.Entity<Phim>()
                .HasIndex(e => e.NamPhatHanh)
                .HasDatabaseName("IX_Phim_NamPhatHanh");

            modelBuilder.Entity<TapPhim>()
                .HasIndex(e => e.MaPhim)
                .HasDatabaseName("IX_TapPhim_MaPhim");

            modelBuilder.Entity<BinhLuan>()
                .HasIndex(e => e.MaTap)
                .HasDatabaseName("IX_BinhLuan_MaTap");

            modelBuilder.Entity<BinhLuan>()
                .HasIndex(e => e.MaND)
                .HasDatabaseName("IX_BinhLuan_MaND");

            modelBuilder.Entity<LichSuXem>()
                .HasIndex(e => e.MaND)
                .HasDatabaseName("IX_LichSuXem_MaND");

            modelBuilder.Entity<LichSuXem>()
                .HasIndex(e => e.ThoiDiemXem)
                .HasDatabaseName("IX_LichSuXem_ThoiDiemXem");

            modelBuilder.Entity<DanhGia>()
                .HasIndex(e => e.MaTap)
                .HasDatabaseName("IX_DanhGia_MaTap");

            // Relationships configuration
            modelBuilder.Entity<Phim>()
                .HasOne(p => p.QuocGia)
                .WithMany(q => q.Phims)
                .HasForeignKey(p => p.MaQG)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Phim>()
                .HasOne(p => p.TheLoaiPhim)
                .WithMany(tl => tl.Phims)
                .HasForeignKey(p => p.MaTL)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Phim>()
                .HasOne(p => p.DanhMuc)
                .WithMany(dm => dm.Phims)
                .HasForeignKey(p => p.MaDM)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TapPhim>()
                .HasOne(t => t.Phim)
                .WithMany(p => p.TapPhims)
                .HasForeignKey(t => t.MaPhim)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThongKePhim>()
                .HasOne(tk => tk.Phim)
                .WithOne(p => p.ThongKePhim)
                .HasForeignKey<ThongKePhim>(tk => tk.MaPhim)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThongKeTapPhim>()
                .HasOne(tk => tk.TapPhim)
                .WithOne(t => t.ThongKeTapPhim)
                .HasForeignKey<ThongKeTapPhim>(tk => tk.MaTap)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BinhLuan>()
                .HasOne(bl => bl.NguoiDung)
                .WithMany(nd => nd.BinhLuans)
                .HasForeignKey(bl => bl.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BinhLuan>()
                .HasOne(bl => bl.TapPhim)
                .WithMany(t => t.BinhLuans)
                .HasForeignKey(bl => bl.MaTap)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DangKy>()
                .HasOne(dk => dk.LoaiNguoiDung)
                .WithMany(lnd => lnd.DangKys)
                .HasForeignKey(dk => dk.MaLoaiND)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DangKy>()
                .HasOne(dk => dk.NguoiDung)
                .WithMany(nd => nd.DangKys)
                .HasForeignKey(dk => dk.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichSuXem>()
                .HasOne(ls => ls.NguoiDung)
                .WithMany(nd => nd.LichSuXems)
                .HasForeignKey(ls => ls.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichSuXem>()
                .HasOne(ls => ls.TapPhim)
                .WithMany(t => t.LichSuXems)
                .HasForeignKey(ls => ls.MaTap)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhimYeuThich>()
                .HasOne(pyt => pyt.NguoiDung)
                .WithMany(nd => nd.PhimYeuThichs)
                .HasForeignKey(pyt => pyt.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhimYeuThich>()
                .HasOne(pyt => pyt.Phim)
                .WithMany(p => p.PhimYeuThichs)
                .HasForeignKey(pyt => pyt.MaPhim)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhGia>()
                .HasOne(dg => dg.TapPhim)
                .WithMany(t => t.DanhGias)
                .HasForeignKey(dg => dg.MaTap)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhGia>()
                .HasOne(dg => dg.NguoiDung)
                .WithMany(nd => nd.DanhGias)
                .HasForeignKey(dg => dg.MaND)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
