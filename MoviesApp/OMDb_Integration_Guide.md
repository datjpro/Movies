# OMDb API Integration - Hướng dẫn sử dụng

## Giới thiệu

Tính năng OMDb API Integration đã được tích hợp vào form **Tạo phim mới** để tự động điền thông tin phim từ cơ sở dữ liệu OMDb (Open Movie Database).

## API Key

- **API Key:** `5d4d8ff1`
- **Base URL:** `http://www.omdbapi.com/`

## Cách sử dụng

### 1. Truy cập trang tạo phim mới

- Vào menu **Phim** → **Tạo phim mới**
- Hoặc truy cập trực tiếp: `http://localhost:5032/Phim/Create`

### 2. Sử dụng tính năng OMDb Search

1. **Nhập tên phim:** Ở phần "OMDb API - Tìm kiếm thông tin phim", nhập tên phim tiếng Anh
2. **Tìm kiếm:** Nhấn nút "Tìm kiếm OMDb" hoặc nhấn Enter
3. **Xem kết quả:** Thông tin phim sẽ hiển thị bên dưới nếu tìm thấy
4. **Điền tự động:** Nhấn nút "Điền thông tin tự động" để tự động điền vào form

### 3. Thông tin được tự động điền

- **Tên phim:** Title từ OMDb
- **Mô tả phim:** Plot/Description
- **Ảnh phim:** Poster URL
- **Năm phát hành:** Year
- **Thời lượng:** Runtime (phút)
- **Các trường mở rộng** (nếu có trong database):
  - Đạo diễn (Director)
  - Biên kịch (Writer)
  - Diễn viên (Actors)
  - Điểm IMDb (imdbRating)
  - Điểm Metascore
  - Giải thưởng (Awards)
  - IMDb ID
  - Ngôn ngữ (Language)
  - Quốc gia sản xuất (Country)
  - Xếp hạng (Rated)

### 4. Hoàn tất tạo phim

- Kiểm tra và chỉnh sửa thông tin nếu cần
- Chọn **Quốc gia**, **Thể loại phim**, **Danh mục**
- Nhấn **Tạo** để lưu phim mới

## Lưu ý kỹ thuật

### File cấu hình

- **API Key** được lưu trong `appsettings.json`:

```json
"OMDbSettings": {
  "ApiKey": "5d4d8ff1",
  "BaseUrl": "http://www.omdbapi.com/"
}
```

### Services đã tạo

- **OMDbService:** `Services/OMDbService.cs` - Xử lý API calls
- **OMDbSettings:** `Models/OMDbSettings.cs` - Cấu hình settings
- **API Controller:** `Controllers/Api/OMDbController.cs` - REST API endpoints

### API Endpoints

- `GET /api/OMDb/search/{title}` - Tìm kiếm phim theo tên
- `POST /api/OMDb/import/{movieId}?title={title}` - Import dữ liệu vào phim có sẵn

### Database Schema Updates

Model `Phim` đã được mở rộng với các trường mới để lưu thông tin từ OMDb:

- BienKich (Writer) - nvarchar(255)
- DaoDien (Director) - nvarchar(255)
- DiemImdb (IMDb Rating) - decimal(3,1)
- DiemMetascore (Metascore) - int
- DienVien (Actors) - ntext
- GiaiThuong (Awards) - ntext
- ImdbId - nvarchar(20)
- LoaiPhim (Type) - nvarchar(50)
- LuotVoteImdb (IMDb Votes) - int
- NgayKhoiChieu (Release Date) - datetime
- NgonNgu (Language) - nvarchar(100)
- QuocGiaSanXuat (Country) - nvarchar(100)
- TongSoMua (Box Office) - int
- XepHang (Rated) - nvarchar(20)

## Ví dụ sử dụng

1. Nhập "The Matrix" → Tìm kiếm
2. Xem thông tin phim hiển thị
3. Nhấn "Điền thông tin tự động"
4. Form sẽ được điền với:
   - Tên: "The Matrix"
   - Năm: 1999
   - Thời lượng: 136 phút
   - Mô tả: "A computer hacker learns..."
   - Ảnh: URL poster từ OMDb
   - Và các thông tin khác

## Troubleshooting

- **Không tìm thấy phim:** Thử tên tiếng Anh chính xác
- **API không phản hồi:** Kiểm tra kết nối internet
- **Lỗi 401:** Kiểm tra API key trong `appsettings.json`
- **Lỗi CORS:** Đảm bảo CORS đã được cấu hình trong `Program.cs`
