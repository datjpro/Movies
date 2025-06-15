using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        [StringLength(10)]
        public string MaDG { get; set; } = string.Empty;

        public DateTime ThoiGianDG { get; set; } = DateTime.Now;

        [Range(1, 10)]
        public byte DiemDG { get; set; }

        [StringLength(1000)]
        public string? NhanXet { get; set; }

        [Required]
        [StringLength(10)]
        public string MaTap { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("MaTap")]
        public virtual TapPhim TapPhim { get; set; } = null!;

        [ForeignKey("MaND")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;
    }
}
