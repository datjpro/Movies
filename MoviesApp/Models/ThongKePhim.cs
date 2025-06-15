using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("ThongKePhim")]
    public class ThongKePhim
    {
        [Key]
        [StringLength(10)]
        public string MaPhim { get; set; } = string.Empty;

        public long TongLuotXem { get; set; } = 0;

        [Column(TypeName = "decimal(3,2)")]
        public decimal DiemTrungBinh { get; set; } = 0;

        public int SoLuotDanhGia { get; set; } = 0;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("MaPhim")]
        public virtual Phim Phim { get; set; } = null!;
    }
}
