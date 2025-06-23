# 🎬 HƯỚNG DẪN TEST TÍNH NĂNG UPLOAD VIDEO CDN

## 🚀 Trạng Thái Hệ Thống

### ✅ Servers Đang Chạy

- **MoviesApp**: `http://localhost:5032` - Main web application
- **CDN Server**: `http://localhost:5288` - Video streaming CDN service

### ✅ Tính Năng Đã Hoàn Thiện

- Upload video file lên CDN từ admin panel
- Support nhiều format video (MP4, AVI, MKV, MOV, WMV, FLV)
- Progress tracking trong quá trình upload
- Preview video sau upload
- Tab interface: Upload File vs URL Video
- Delete video từ CDN
- Validation file size (max 2GB) và format

## 📋 Quy Trình Test Upload Video

### 1. Đăng Nhập Admin

1. Truy cập: `http://localhost:5032`
2. Đăng nhập với tài khoản admin:
   - **Email**: `admin@moviesapp.com`
   - **Password**: `Admin123!`

### 2. Truy Cập Admin Dashboard

1. Sau khi đăng nhập, click **Admin Dashboard**
2. Hoặc truy cập trực tiếp: `http://localhost:5032/Admin/Dashboard`

### 3. Quản Lý Phim

1. Từ Dashboard, click **"Quản lý phim"**
2. Hoặc truy cập: `http://localhost:5032/Admin/SimpleAdmin/Movies`
3. Chọn một phim để quản lý tập phim

### 4. Thêm Tập Phim Mới

1. Click **"Thêm tập mới"**
2. Hoặc truy cập: `http://localhost:5032/Admin/TapPhim/Create?phimId={PHIM_ID}`

### 5. Test Upload Video

#### Option A: Upload File Video

1. Chọn tab **"Upload File"**
2. **Drag & Drop** file video vào vùng upload
3. Hoặc click **"Chọn File"** để browse file
4. Observe:
   - ✅ File validation (format + size)
   - ✅ Progress bar hiển thị
   - ✅ Upload status messages
   - ✅ Success notification
   - ✅ Video URL được set tự động

#### Option B: Nhập URL Video

1. Chọn tab **"URL Video"**
2. Nhập URL video (YouTube/Vimeo/Direct):
   ```
   YouTube: https://www.youtube.com/watch?v=dQw4w9WgXcQ
   Vimeo: https://vimeo.com/123456789
   Direct: https://sample-videos.com/zip/10/mp4/SampleVideo_720x480_1mb.mp4
   ```
3. Observe:
   - ✅ URL validation
   - ✅ Video preview
   - ✅ Format detection

### 6. Test Edit Tập Phim

1. Từ danh sách tập phim, click **"Edit"**
2. Test chức năng:
   - ✅ Chỉnh sửa URL video hiện tại
   - ✅ Upload video mới (thay thế)
   - ✅ Xóa video từ CDN
   - ✅ Preview video

## 🧪 Test Cases Cần Kiểm Tra

### File Upload Tests

- [ ] **Valid formats**: Upload file MP4, AVI, MKV, MOV, WMV, FLV
- [ ] **Invalid formats**: Upload file JPG, TXT, PDF (should be rejected)
- [ ] **File size**: Upload file > 2GB (should be rejected)
- [ ] **Network error**: Disconnect internet during upload
- [ ] **CDN offline**: Stop CDN server và test upload

### URL Video Tests

- [ ] **YouTube URLs**:
  - `https://www.youtube.com/watch?v=VIDEO_ID`
  - `https://youtu.be/VIDEO_ID`
- [ ] **Vimeo URLs**: `https://vimeo.com/VIDEO_ID`
- [ ] **Direct video URLs**: `https://domain.com/video.mp4`
- [ ] **Invalid URLs**: `https://google.com`, `invalid-url`

### User Experience Tests

- [ ] **Drag & Drop**: Kéo file từ File Explorer vào upload area
- [ ] **Progress tracking**: Observe progress bar với file lớn
- [ ] **Error handling**: Test với invalid files/URLs
- [ ] **Success notifications**: Verify success messages
- [ ] **Tab switching**: Chuyển đổi giữa Upload File vs URL Video

### Integration Tests

- [ ] **Save episode**: Tạo tập phim mới với video uploaded
- [ ] **Edit episode**: Sửa tập phim và thay đổi video
- [ ] **Delete video**: Xóa video từ CDN
- [ ] **Video playback**: Xem video đã upload trên trang user

## 🔧 Debug & Troubleshooting

### Kiểm Tra CDN Server

```bash
# Test CDN health endpoint
curl http://localhost:5288/api/v1/videos/health

# Check CDN logs
# Xem terminal đang chạy CDN server
```

### Kiểm Tra MoviesApp Logs

```bash
# Check application logs trong terminal
# Xem terminal đang chạy MoviesApp
```

### Browser Developer Tools

1. Mở **F12 Developer Tools**
2. Tab **Network**: Xem HTTP requests đến CDN
3. Tab **Console**: Xem JavaScript errors
4. Tab **Application**: Xem Local Storage/Session

### Common Issues

- **Upload fails**: Check CDN server status
- **Progress not showing**: Check JavaScript console
- **Video not saving**: Check CDNService configuration
- **Preview not working**: Check video URL format

## 📊 Expected Results

### Successful Upload Flow:

```
1. User selects video file
2. File validation passes
3. Upload progress shows 0-100%
4. CDN returns video URL
5. URL saved to database
6. Success notification appears
7. Video preview available
```

### Successful URL Input Flow:

```
1. User enters video URL
2. URL format validation
3. Video preview loads
4. URL saved to form
5. Form submission successful
```

## 🎯 Success Metrics

- ✅ Upload success rate > 95%
- ✅ File validation accuracy 100%
- ✅ Progress tracking works smoothly
- ✅ Error messages are clear and helpful
- ✅ Video preview works for all supported formats
- ✅ CDN integration stable

## 📝 Test Results Template

```
Date: ___________
Tester: ___________

Upload File Tests:
[ ] MP4 upload - Pass/Fail
[ ] Large file (>1GB) - Pass/Fail
[ ] Invalid format rejection - Pass/Fail
[ ] Progress tracking - Pass/Fail
[ ] Drag & drop - Pass/Fail

URL Video Tests:
[ ] YouTube URL - Pass/Fail
[ ] Vimeo URL - Pass/Fail
[ ] Direct video URL - Pass/Fail
[ ] Invalid URL rejection - Pass/Fail
[ ] Video preview - Pass/Fail

Integration Tests:
[ ] Save new episode - Pass/Fail
[ ] Edit existing episode - Pass/Fail
[ ] Delete video from CDN - Pass/Fail
[ ] Video playback on user page - Pass/Fail

Issues Found:
_________________________
_________________________

Overall Status: Pass/Fail
```

---

🎉 **Ready for Testing!** Cả hai servers đã sẵn sàng để test tính năng upload video CDN.
