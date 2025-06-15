using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    [Table("ThongKeTapPhim")]
    public class ThongKeTapPhim
    {
        [Key]
        [StringLength(10)]
        public string MaTap { get; set; } = string.Empty;

        public long LuotXem { get; set; } = 0;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("MaTap")]
        public virtual TapPhim TapPhim { get; set; } = null!;
    }
}
