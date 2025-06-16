using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("QuocGia")]
    public class QuocGia
    {
        [Key]
        [StringLength(10)]
        public string MaQG { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TenQG { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }        [StringLength(255)]
        public string? AnhQG { get; set; }

        // Compatibility properties
        [NotMapped]
        public string TenQuocGia => TenQG;
        
        [NotMapped]
        public string QuocGiaId => MaQG;

        // Navigation properties
        public virtual ICollection<Phim> Phims { get; set; } = new List<Phim>();
    }
}
