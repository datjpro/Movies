# Movies App - Tách Database Layer thành API

## Cấu trúc dự án

### 1. MoviesApp.DataAccess (Class Library)

- **Models/**: Chứa tất cả các entity models (Phim, TapPhim, NguoiDung, TheLoaiPhim, QuocGia, DanhMuc, v.v.)
- **Data/**: Chứa DbContext và SeedData
- **Repositories/**: Chứa Repository Pattern và Unit of Work
  - `IRepository<T>` và `Repository<T>`: Generic repository
  - `IPhimRepository`, `PhimRepository`: Repository chuyên biệt cho Phim
  - `ITapPhimRepository`, `TapPhimRepository`: Repository chuyên biệt cho TapPhim
  - `INguoiDungRepository`, `NguoiDungRepository`: Repository chuyên biệt cho NguoiDung
  - `IUnitOfWork`, `UnitOfWork`: Quản lý tất cả repositories

### 2. MoviesApp.Api (Web API)

- **Controllers/**: Chứa các API controllers
  - `PhimController`: API cho quản lý phim
  - `TapPhimController`: API cho quản lý tập phim
  - `NguoiDungController`: API cho quản lý người dùng
  - `TheLoaiPhimController`: API cho quản lý thể loại phim
  - `QuocGiaController`: API cho quản lý quốc gia
  - `DanhMucController`: API cho quản lý danh mục

### 3. MoviesApp (Web MVC - Original)

- Ứng dụng web gốc (có thể giữ lại hoặc chuyển sang sử dụng API)

## API Endpoints

### Phim Endpoints

- `GET /api/phim` - Lấy tất cả phim
- `GET /api/phim/{id}` - Lấy chi tiết phim theo ID
- `GET /api/phim/search?searchTerm={term}` - Tìm kiếm phim
- `GET /api/phim/theloai/{theLoaiId}` - Lấy phim theo thể loại
- `GET /api/phim/quocgia/{quocGiaId}` - Lấy phim theo quốc gia
- `GET /api/phim/danhmuc/{danhMucId}` - Lấy phim theo danh mục
- `POST /api/phim` - Tạo phim mới
- `PUT /api/phim/{id}` - Cập nhật phim
- `DELETE /api/phim/{id}` - Xóa phim

### TapPhim Endpoints

- `GET /api/tapphim` - Lấy tất cả tập phim
- `GET /api/tapphim/{id}` - Lấy chi tiết tập phim
- `GET /api/tapphim/phim/{phimId}` - Lấy tất cả tập của một phim
- `POST /api/tapphim` - Tạo tập phim mới
- `PUT /api/tapphim/{id}` - Cập nhật tập phim
- `DELETE /api/tapphim/{id}` - Xóa tập phim

### NguoiDung Endpoints

- `GET /api/nguoidung` - Lấy tất cả người dùng
- `GET /api/nguoidung/{id}` - Lấy thông tin người dùng
- `GET /api/nguoidung/email/{email}` - Lấy người dùng theo email
- `GET /api/nguoidung/{id}/phimyeuthich` - Lấy danh sách phim yêu thích
- `GET /api/nguoidung/{id}/lichsuxem` - Lấy lịch sử xem
- `POST /api/nguoidung` - Tạo người dùng mới
- `PUT /api/nguoidung/{id}` - Cập nhật thông tin người dùng
- `DELETE /api/nguoidung/{id}` - Xóa người dùng

### TheLoaiPhim, QuocGia, DanhMuc Endpoints

Tương tự với các CRUD operations cơ bản:

- `GET /api/{controller}` - Lấy tất cả
- `GET /api/{controller}/{id}` - Lấy theo ID
- `POST /api/{controller}` - Tạo mới
- `PUT /api/{controller}/{id}` - Cập nhật
- `DELETE /api/{controller}/{id}` - Xóa

## Chạy ứng dụng

### 1. Chạy API

```bash
cd MoviesApp.Api
dotnet run
```

API sẽ chạy trên: `http://localhost:5074`
Swagger UI: `http://localhost:5074/swagger`

### 2. Chạy Web MVC (nếu cần)

```bash
cd MoviesApp
dotnet run
```

## Kết nối Database

Cả hai project đều sử dụng cùng connection string từ `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MoviesAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Tính năng chính

1. **Repository Pattern**: Tách biệt logic truy cập dữ liệu
2. **Unit of Work**: Quản lý transactions và repositories
3. **Dependency Injection**: Sử dụng DI container của .NET
4. **CORS**: Cho phép truy cập từ các domain khác
5. **Swagger**: Tự động tạo API documentation
6. **Entity Framework Core**: ORM để làm việc với database

## Migration Database

Nếu cần tạo migration mới:

```bash
cd MoviesApp.DataAccess
dotnet ef migrations add MigrationName --startup-project ../MoviesApp.Api
dotnet ef database update --startup-project ../MoviesApp.Api
```

## Sử dụng API trong ứng dụng khác

Bạn có thể sử dụng API này từ:

- React/Angular/Vue.js frontend
- Mobile apps (Flutter, React Native, Xamarin)
- Desktop applications
- Microservices khác

## Bảo mật

Hiện tại API chưa có authentication/authorization. Để production, nên thêm:

- JWT Authentication
- Role-based Authorization
- API Rate Limiting
- Input Validation
- HTTPS Enforcement
