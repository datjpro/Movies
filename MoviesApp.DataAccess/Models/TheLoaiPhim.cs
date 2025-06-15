using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.DataAccess.Models
{
    [Table("TheLoaiPhim")]
    public class TheLoaiPhim
    {
        [Key]
        [StringLength(10)]
        public string MaTL { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TenTL { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        // Navigation properties
        public virtual ICollection<Phim> Phims { get; set; } = new List<Phim>();
    }
}
