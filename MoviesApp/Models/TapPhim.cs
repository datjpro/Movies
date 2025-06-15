using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("TapPhim")]
    public class TapPhim
    {
        [Key]
        [StringLength(10)]
        public string MaTap { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaPhim { get; set; } = string.Empty;

        [Required]
        public int SoTapThuTu { get; set; }

        [StringLength(255)]
        public string? TenTap { get; set; }

        [Column(TypeName = "ntext")]
        public string? ChiTiet { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        public int? ThoiLuongTap { get; set; } // Tính bằng phút

        public DateTime? NgayPhatHanh { get; set; }

        // Navigation properties
        [ForeignKey("MaPhim")]
        public virtual Phim Phim { get; set; } = null!;

        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
        public virtual ICollection<LichSuXem> LichSuXems { get; set; } = new List<LichSuXem>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public virtual ThongKeTapPhim? ThongKeTapPhim { get; set; }
    }
}
