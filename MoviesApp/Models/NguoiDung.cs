using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [StringLength(10)]
        public string MaND { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TenND { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(15)]
        public string? SDT { get; set; }

        [Required]
        [StringLength(255)]
        public string Pass { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
        public virtual ICollection<DangKy> DangKys { get; set; } = new List<DangKy>();
        public virtual ICollection<LichSuXem> LichSuXems { get; set; } = new List<LichSuXem>();
        public virtual ICollection<PhimYeuThich> PhimYeuThichs { get; set; } = new List<PhimYeuThich>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
