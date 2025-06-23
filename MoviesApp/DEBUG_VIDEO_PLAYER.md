# 🐛 DEBUG HƯỚNG DẪN FIX LỖI VIDEO PLAYER

## 🔍 Vấn Đề Phát Hiện

Video player không thể phát video - có thể do:

1. **Không có dữ liệu VideoUrl** trong database TapPhim
2. **URL video không hợp lệ**
3. **CORS issues** với video từ CDN
4. **JavaScript errors** trong video player

## 🛠️ STEPS FIX LỖI

### 1. Kiểm Tra Database - Có TapPhim nào có VideoUrl không?

Mở SQL Server Management Studio hoặc chạy query:

```sql
-- Kiểm tra dữ liệu TapPhim
SELECT TOP 10
    MaTap,
    MaPhim,
    SoTapThuTu,
    TenTap,
    VideoUrl,
    CASE
        WHEN VideoUrl IS NULL THEN 'NULL'
        WHEN VideoUrl = '' THEN 'EMPTY'
        ELSE 'HAS_VALUE'
    END as VideoStatus
FROM TapPhim
ORDER BY SoTapThuTu;

-- Tìm TapPhim có VideoUrl
SELECT * FROM TapPhim WHERE VideoUrl IS NOT NULL AND VideoUrl != '';
```

### 2. Nếu Không Có Dữ liệu - Thêm TapPhim Test

```sql
-- Thêm tập phim test với YouTube video
INSERT INTO TapPhim (MaTap, MaPhim, SoTapThuTu, TenTap, VideoUrl, ThoiLuongTap, NgayPhatHanh, ChiTiet)
VALUES (
    'T999',
    'P001',  -- Thay bằng MaPhim có sẵn
    1,
    'Tập 1 - Test Video',
    'https://www.youtube.com/watch?v=dQw4w9WgXcQ',  -- Video test
    45,
    GETDATE(),
    'Tập phim test để kiểm tra video player'
);

-- Hoặc thêm với direct video URL
INSERT INTO TapPhim (MaTap, MaPhim, SoTapThuTu, TenTap, VideoUrl, ThoiLuongTap, NgayPhatHanh, ChiTiet)
VALUES (
    'T998',
    'P001',  -- Thay bằng MaPhim có sẵn
    2,
    'Tập 2 - Test Direct Video',
    'https://sample-videos.com/zip/10/mp4/SampleVideo_720x480_1mb.mp4',  -- Direct video
    5,
    GETDATE(),
    'Tập phim test với direct video link'
);
```

### 3. Kiểm Tra Phim Có TapPhim Không

```sql
-- Tìm phim có tập phim
SELECT
    p.MaPhim,
    p.TenPhim,
    COUNT(t.MaTap) as SoTapPhimCoSan,
    MAX(CASE WHEN t.VideoUrl IS NOT NULL AND t.VideoUrl != '' THEN 1 ELSE 0 END) as CoVideoUrl
FROM Phim p
LEFT JOIN TapPhim t ON p.MaPhim = t.MaPhim
GROUP BY p.MaPhim, p.TenPhim
HAVING COUNT(t.MaTap) > 0
ORDER BY SoTapPhimCoSan DESC;
```

### 4. Test Video Player Directly

Mở Developer Tools (F12) và test:

```javascript
// Test trong Console của browser
console.log("Testing video player...");

// Kiểm tra có element video player không
const videoElement = document.getElementById("mainVideoPlayer");
console.log("Video element:", videoElement);

// Kiểm tra VideoUrl từ server
console.log("Current page video URL:", window.location.href);

// Test play video function
function testPlayVideo() {
  const testUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
  playEpisode(testUrl, "Test Episode");
}

// Chạy test
testPlayVideo();
```

### 5. Fix CORS Issue (Nếu Cần)

Nếu video từ CDN localhost bị CORS, thêm headers trong CDN server:

```csharp
// Trong VideoStreamingCDN Program.cs hoặc Controller
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }

    await next();
});
```

### 6. Thêm Debug Info Trong View

Thêm vào Details.cshtml để debug:

```html
<!-- DEBUG INFO - Thêm vào đầu video player section -->
@if (ViewBag.SelectedEpisode != null) { var selectedEpisode =
ViewBag.SelectedEpisode as MoviesApp.Models.TapPhim;
<!-- DEBUG: Show video info -->
<div class="alert alert-info">
  <h6>🐛 DEBUG INFO:</h6>
  <p><strong>Episode:</strong> @selectedEpisode.SoTapThuTu</p>
  <p><strong>VideoUrl:</strong> @(selectedEpisode.VideoUrl ?? "NULL")</p>
  <p><strong>URL Length:</strong> @(selectedEpisode.VideoUrl?.Length ?? 0)</p>
</div>
}
```

### 7. Test URLs Khác Nhau

Test với các loại URL:

```
YouTube: https://www.youtube.com/watch?v=dQw4w9WgXcQ
Direct MP4: https://sample-videos.com/zip/10/mp4/SampleVideo_720x480_1mb.mp4
Local CDN: http://localhost:5288/videos/episode_123.mp4
```

### 8. Kiểm Tra Network Tab

1. Mở **F12 > Network**
2. Reload trang có video
3. Kiểm tra:
   - Request đến video URL có success không?
   - Status code là gì? (200, 404, 403, CORS error?)
   - Response headers có đúng không?

## 🎯 Quick Fix Steps

### Option 1: Thêm Video Test Nhanh

```sql
-- Copy MaPhim từ database
SELECT TOP 5 MaPhim, TenPhim FROM Phim;

-- Thêm tập test (thay P001 bằng MaPhim thật)
INSERT INTO TapPhim VALUES ('TTEST1', 'P001', 1, 'Test Episode', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 45, GETDATE(), 'Test video');
```

### Option 2: Upload Video Qua Admin Panel

1. Truy cập: `http://localhost:5032/Admin/TapPhim/Create?phimId=P001`
2. Upload file video hoặc nhập YouTube URL
3. Save và test

### Option 3: Fix Direct Video CORS

Nếu dùng CDN localhost, start CDN server và kiểm tra CORS headers.

## 📋 Expected Results After Fix

✅ Database có ít nhất 1 TapPhim với VideoUrl  
✅ Browser không có CORS errors  
✅ Video player hiển thị video  
✅ Play/pause controls hoạt động  
✅ Video loading successfully

## 🆘 Nếu Vẫn Lỗi

1. **Check browser console** - có JavaScript errors không?
2. **Check network tab** - video request có thành công không?
3. **Test simple HTML5 video** - browser có support video format không?
4. **Check database connection** - EF có query đúng TapPhim không?

---

🎬 **Chạy các bước trên để fix video player issue!**
