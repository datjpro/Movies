// Ví dụ: PhimController trong MoviesApp có thể như thế này
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MoviesApp.Controllers
{
    public class PhimController : Controller
    {
        private readonly HttpClient _httpClient;

        public PhimController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // CONTROLLER - Xử lý request và điều hướng
        public async Task<IActionResult> Index()
        {
            // Gọi API (MODEL) để lấy dữ liệu
            var response = await _httpClient.GetAsync("http://localhost:5074/api/phim");
            var json = await response.Content.ReadAsStringAsync();
            var phims = JsonSerializer.Deserialize<List<Phim>>(json);

            // Trả về VIEW với dữ liệu
            return View(phims);
        }
    }
}

// Thì vẫn có đủ:
// MODEL = API (MoviesApp.Api)
// VIEW = Razor Views  
// CONTROLLER = MVC Controller
