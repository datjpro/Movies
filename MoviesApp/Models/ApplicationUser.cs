using Microsoft.AspNetCore.Identity;

namespace MoviesApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginTime { get; set; }
        
        // Navigation properties
        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public virtual ICollection<LichSuXem> LichSuXems { get; set; } = new List<LichSuXem>();
        public virtual ICollection<PhimYeuThich> PhimYeuThichs { get; set; } = new List<PhimYeuThich>();
    }
}
