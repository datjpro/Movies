using Microsoft.AspNetCore.Mvc;
using MoviesApp.Services;
using MoviesApp.Data;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Models;

namespace MoviesApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OMDbController : ControllerBase
    {
        private readonly OMDbService _omdbService;
        private readonly WebMoviesDbContext _context;

        public OMDbController(OMDbService omdbService, WebMoviesDbContext context)
        {
            _omdbService = omdbService;
            _context = context;
        }

        [HttpGet("search/{title}")]
        public async Task<IActionResult> SearchMovie(string title)
        {
            try
            {
                var movieInfo = await _omdbService.GetMovieByTitleAsync(title);
                if (movieInfo == null || movieInfo.Response == "False")
                {
                    return NotFound(new { message = "Không tìm thấy phim trên OMDb" });
                }

                return Ok(movieInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tìm kiếm phim", error = ex.Message });
            }
        }

        [HttpPost("import/{movieId}")]
        public async Task<IActionResult> ImportMovieData(string movieId, string title)
        {
            try
            {
                // Tìm phim trong database
                var existingMovie = await _context.Phim.FirstOrDefaultAsync(p => p.MaPhim == movieId);
                if (existingMovie == null)
                {
                    return NotFound(new { message = "Không tìm thấy phim trong database" });
                }

                // Lấy thông tin từ OMDb
                var omdbInfo = await _omdbService.GetMovieByTitleAsync(title);
                if (omdbInfo == null || omdbInfo.Response == "False")
                {
                    return BadRequest(new { message = "Không tìm thấy thông tin trên OMDb" });
                }

                // Cập nhật thông tin phim
                UpdateMovieFromOMDb(existingMovie, omdbInfo);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Đã cập nhật thông tin phim từ OMDb",
                    movieId = movieId,
                    title = existingMovie.TenPhim
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi import dữ liệu", error = ex.Message });
            }
        }

        private void UpdateMovieFromOMDb(Phim movie, OMDbMovieResponse omdbInfo)
        {
            // Cập nhật các trường có sẵn
            if (!string.IsNullOrEmpty(omdbInfo.Plot))
                movie.MoTaPhim = omdbInfo.Plot;

            if (!string.IsNullOrEmpty(omdbInfo.Poster) && omdbInfo.Poster != "N/A")
                movie.AnhPhim = omdbInfo.Poster;

            if (int.TryParse(omdbInfo.Year, out int year))
                movie.NamPhatHanh = year;

            if (!string.IsNullOrEmpty(omdbInfo.Runtime) && omdbInfo.Runtime != "N/A")
            {
                var runtimeStr = omdbInfo.Runtime.Replace(" min", "");
                if (int.TryParse(runtimeStr, out int runtime))
                    movie.ThoiLuongPhim = runtime;
            }

            // Cập nhật các trường mới nếu có trong model
            try
            {
                var movieType = movie.GetType();
                
                var bienKichProp = movieType.GetProperty("BienKich");
                if (bienKichProp != null && !string.IsNullOrEmpty(omdbInfo.Writer))
                    bienKichProp.SetValue(movie, omdbInfo.Writer);

                var daoDienProp = movieType.GetProperty("DaoDien");
                if (daoDienProp != null && !string.IsNullOrEmpty(omdbInfo.Director))
                    daoDienProp.SetValue(movie, omdbInfo.Director);

                var diemImdbProp = movieType.GetProperty("DiemImdb");
                if (diemImdbProp != null && decimal.TryParse(omdbInfo.imdbRating, out decimal imdbRating))
                    diemImdbProp.SetValue(movie, imdbRating);

                var diemMetascoreProp = movieType.GetProperty("DiemMetascore");
                if (diemMetascoreProp != null && int.TryParse(omdbInfo.Metascore, out int metascore))
                    diemMetascoreProp.SetValue(movie, metascore);

                var dienVienProp = movieType.GetProperty("DienVien");
                if (dienVienProp != null && !string.IsNullOrEmpty(omdbInfo.Actors))
                    dienVienProp.SetValue(movie, omdbInfo.Actors);

                var giaiThuongProp = movieType.GetProperty("GiaiThuong");
                if (giaiThuongProp != null && !string.IsNullOrEmpty(omdbInfo.Awards))
                    giaiThuongProp.SetValue(movie, omdbInfo.Awards);

                var imdbIdProp = movieType.GetProperty("ImdbId");
                if (imdbIdProp != null && !string.IsNullOrEmpty(omdbInfo.imdbID))
                    imdbIdProp.SetValue(movie, omdbInfo.imdbID);

                var loaiPhimProp = movieType.GetProperty("LoaiPhim");
                if (loaiPhimProp != null && !string.IsNullOrEmpty(omdbInfo.Type))
                    loaiPhimProp.SetValue(movie, omdbInfo.Type);

                var luotVoteProp = movieType.GetProperty("LuotVoteImdb");
                if (luotVoteProp != null && !string.IsNullOrEmpty(omdbInfo.imdbVotes))
                {
                    var votesStr = omdbInfo.imdbVotes.Replace(",", "");
                    if (int.TryParse(votesStr, out int votes))
                        luotVoteProp.SetValue(movie, votes);
                }

                var ngonNguProp = movieType.GetProperty("NgonNgu");
                if (ngonNguProp != null && !string.IsNullOrEmpty(omdbInfo.Language))
                    ngonNguProp.SetValue(movie, omdbInfo.Language);

                var quocGiaSanXuatProp = movieType.GetProperty("QuocGiaSanXuat");
                if (quocGiaSanXuatProp != null && !string.IsNullOrEmpty(omdbInfo.Country))
                    quocGiaSanXuatProp.SetValue(movie, omdbInfo.Country);

                var xepHangProp = movieType.GetProperty("XepHang");
                if (xepHangProp != null && !string.IsNullOrEmpty(omdbInfo.Rated))
                    xepHangProp.SetValue(movie, omdbInfo.Rated);

                var ngayKhoiChieuProp = movieType.GetProperty("NgayKhoiChieu");
                if (ngayKhoiChieuProp != null && !string.IsNullOrEmpty(omdbInfo.Released) && omdbInfo.Released != "N/A")
                {
                    if (DateTime.TryParse(omdbInfo.Released, out DateTime releaseDate))
                        ngayKhoiChieuProp.SetValue(movie, releaseDate);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi reflection nếu property không tồn tại
            }
        }
    }
}
