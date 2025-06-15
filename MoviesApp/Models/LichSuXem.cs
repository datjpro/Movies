using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("LichSuXem")]
    public class LichSuXem
    {
        [Key]
        [StringLength(10)]
        public string MaLS { get; set; } = string.Empty;

        public DateTime ThoiDiemXem { get; set; } = DateTime.Now;

        public int ThoiGianXem { get; set; } = 0; // Thời gian xem tính bằng giây

        [Required]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string MaTap { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("MaND")]
        public virtual NguoiDung NguoiDung { get; set; } = null!;

        [ForeignKey("MaTap")]
        public virtual TapPhim TapPhim { get; set; } = null!;
    }
}
