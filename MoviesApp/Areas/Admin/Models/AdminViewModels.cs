using MoviesApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Areas.Admin.Models
{
    public class UserManagementViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public Dictionary<string, List<string>> UserRoles { get; set; } = new Dictionary<string, List<string>>();
        public int TotalUsers { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public string? FilterRole { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        
        public List<string> SelectedRoles { get; set; } = new List<string>();
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }

    public class MovieManagementViewModel
    {
        public List<Phim> Movies { get; set; } = new List<Phim>();
        public List<TheLoaiPhim> Genres { get; set; } = new List<TheLoaiPhim>();
        public List<QuocGia> Countries { get; set; } = new List<QuocGia>();
        public List<DanhMuc> Categories { get; set; } = new List<DanhMuc>();
        
        public int TotalMovies { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public int? FilterGenre { get; set; }
        public int? FilterCountry { get; set; }
    }
}
