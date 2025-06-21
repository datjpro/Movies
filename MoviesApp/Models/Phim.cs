using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("Phim")]
    public class Phim
    {
        [Key]
        [StringLength(10)]
        public string MaPhim { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaQG { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaTL { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaDM { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TenPhim { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string? MoTaPhim { get; set; }

        [StringLength(255)]
        public string? AnhPhim { get; set; }

        public int SoTap { get; set; } = 1;

        [StringLength(50)]
        public string? TinhTrang { get; set; }

        public int? NamPhatHanh { get; set; }        public int? ThoiLuongPhim { get; set; } // Tính bằng phút

        public DateTime NgayTao { get; set; } = DateTime.Now;        // Các thuộc tính OMDb
        [StringLength(500)]
        public string? BienKich { get; set; }

        [StringLength(500)]
        public string? DaoDien { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        public decimal? DiemImdb { get; set; }

        public int? DiemMetascore { get; set; }

        [StringLength(1000)]
        public string? DienVien { get; set; }

        [StringLength(500)]
        public string? GiaiThuong { get; set; }

        [StringLength(20)]
        public string? ImdbId { get; set; }

        [StringLength(50)]
        public string? LoaiPhim { get; set; }

        public int? LuotVoteImdb { get; set; }

        public DateTime? NgayKhoiChieu { get; set; }

        [StringLength(100)]
        public string? NgonNgu { get; set; }

        [StringLength(100)]
        public string? QuocGiaSanXuat { get; set; }

        public int? TongSoMua { get; set; }

        [StringLength(20)]
        public string? XepHang { get; set; }

        [StringLength(500)]
        public string? LinkPhim { get; set; }

        // Compatibility properties
        [NotMapped]
        public string PhimId => MaPhim;
        
        [NotMapped]
        public string? PosterUrl => AnhPhim;
        
        [NotMapped]
        public string? MoTa 
        { 
            get => MoTaPhim; 
            set => MoTaPhim = value; 
        }
        
        [NotMapped]
        public int? NamSanXuat => NamPhatHanh;

        [NotMapped]
        public string TheLoaiPhimId => MaTL;
        
        [NotMapped]
        public string QuocGiaId => MaQG;        // Navigation properties
        [ForeignKey("MaQG")]
        public virtual QuocGia? QuocGia { get; set; }

        [ForeignKey("MaTL")]
        public virtual TheLoaiPhim? TheLoaiPhim { get; set; }

        [ForeignKey("MaDM")]
        public virtual DanhMuc? DanhMuc { get; set; }

        public virtual ICollection<TapPhim> TapPhims { get; set; } = new List<TapPhim>();
        public virtual ThongKePhim? ThongKePhim { get; set; }
        public virtual ICollection<PhimYeuThich> PhimYeuThichs { get; set; } = new List<PhimYeuThich>();
    }
}
