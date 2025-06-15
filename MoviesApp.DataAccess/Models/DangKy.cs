using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
{
    [Table("DangKy")]
    public class DangKy
    {
        [Key]
        [StringLength(10)]
        public string MaDK { get; set; } = string.Empty;

        public DateTime NgayDK { get; set; } = DateTime.Now;

        [Required]
        public DateTime NgayKT { get; set; }

        [StringLength(50)]
        public string? PhuongThucTT { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SoTien { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Hoạt động";

        [Required]
        [StringLength(10)]
        public string MaLoaiND { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("MaLoaiND")]
        public virtual LoaiNguoiDung LoaiNguoiDung { get; set; } = null!;

        [ForeignKey("MaND")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
