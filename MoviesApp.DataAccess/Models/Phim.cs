using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
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

        public int? NamPhatHanh { get; set; }

        public int? ThoiLuongPhim { get; set; } // Tính bằng phút

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaQG")]
        public virtual QuocGia QuocGia { get; set; } = null!;

        [ForeignKey("MaTL")]
        public virtual TheLoaiPhim TheLoaiPhim { get; set; } = null!;

        [ForeignKey("MaDM")]
        public virtual DanhMuc DanhMuc { get; set; } = null!;

        public virtual ICollection<TapPhim> TapPhims { get; set; } = new List<TapPhim>();
        public virtual ThongKePhim? ThongKePhim { get; set; }
        public virtual ICollection<PhimYeuThich> PhimYeuThichs { get; set; } = new List<PhimYeuThich>();
    }
}
