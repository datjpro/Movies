using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
{
    [Table("LoaiNguoiDung")]
    public class LoaiNguoiDung
    {
        [Key]
        [StringLength(10)]
        public string MaLoaiND { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenLoaiND { get; set; } = string.Empty;

        [StringLength(100)]
        public string? VaiTro { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Gia { get; set; }

        public int? SoNgayHieuLuc { get; set; }

        // Navigation properties
        public virtual ICollection<DangKy> DangKys { get; set; } = new List<DangKy>();
    }
}
