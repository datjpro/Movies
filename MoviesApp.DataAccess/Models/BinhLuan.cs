using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
{
    [Table("BinhLuan")]
    public class BinhLuan
    {
        [Key]
        [StringLength(10)]
        public string MaBL { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "ntext")]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayBL { get; set; } = DateTime.Now;

        public DateTime? NgaySuaDoi { get; set; }

        [Required]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaTap { get; set; } = string.Empty;

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        [ForeignKey("MaND")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("MaTap")]
        public virtual TapPhim TapPhim { get; set; } = null!;
    }
}
