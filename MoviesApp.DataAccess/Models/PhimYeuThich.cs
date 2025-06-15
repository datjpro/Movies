using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
{
    [Table("PhimYeuThich")]
    public class PhimYeuThich
    {
        [Key]
        [StringLength(10)]
        public string MaYT { get; set; } = string.Empty;

        public DateTime NgayYT { get; set; } = DateTime.Now;

        [Required]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaPhim { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("MaND")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("MaPhim")]
        public virtual Phim Phim { get; set; } = null!;
    }
}
