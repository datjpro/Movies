# CCFilm Video Player

Video player tùy chỉnh hiện đại với đầy đủ các tính năng điều khiển cho ứng dụng phim CCFilm.

## 🎯 Tính Năng

### Điều Khiển Cơ Bản

- ✅ **Phát/Dừng**: Click video hoặc nút play/pause
- ✅ **Tua nhanh**: Tua lại/tới 10 giây
- ✅ **Thanh tiến độ**: Kéo thả để tua đến vị trí bất kỳ
- ✅ **Hiển thị thời gian**: Thời gian hiện tại / tổng thời gian

### Điều Khiển Âm Thanh

- ✅ **Điều chỉnh âm lượng**: Slider âm lượng từ 0-100%
- ✅ **Tắt/Bật âm**: Nút mute với icon thay đổi theo trạng thái
- ✅ **Phím tắt âm lượng**: Mũi tên lên/xuống để điều chỉnh

### Chất Lượng Video

- ✅ **Đa độ phân giải**: 1080p, 720p, 480p, 360p
- ✅ **Chuyển đổi mượt mà**: Giữ nguyên vị trí phát khi đổi chất lượng
- ✅ **Auto quality**: Tự động chọn chất lượng phù hợp

### Tốc Độ Phát

- ✅ **Đa tốc độ**: 0.25x, 0.5x, 0.75x, 1x, 1.25x, 1.5x, 2x
- ✅ **Thay đổi tức thì**: Không làm gián đoạn video
- ✅ **UI trực quan**: Hiển thị tốc độ hiện tại

### Chế Độ Xem

- ✅ **Toàn màn hình**: Fullscreen với ESC để thoát
- ✅ **Picture-in-Picture**: Xem video trong cửa sổ nhỏ
- ✅ **Responsive**: Tự động thích ứng màn hình

### Phụ Đề

- ✅ **Đa ngôn ngữ**: Hỗ trợ nhiều track phụ đề
- ✅ **Bật/Tắt**: Dễ dàng chuyển đổi
- ✅ **Định vị đẹp**: Hiển thị ở vị trí tối ưu

### Trải Nghiệm UX

- ✅ **Loading states**: Spinner khi đang tải
- ✅ **Hover effects**: Hiện controls khi hover
- ✅ **Auto-hide**: Tự động ẩn controls khi phát
- ✅ **Keyboard shortcuts**: Đầy đủ phím tắt

## 🎮 Phím Tắt

| Phím    | Chức Năng            |
| ------- | -------------------- |
| `Space` | Phát/Dừng video      |
| `←`     | Tua lại 10 giây      |
| `→`     | Tua tới 10 giây      |
| `↑`     | Tăng âm lượng 10%    |
| `↓`     | Giảm âm lượng 10%    |
| `M`     | Tắt/Bật âm thanh     |
| `F`     | Chế độ toàn màn hình |

## 🛠️ Cách Sử Dụng

### 1. Cơ Bản

```html
<!-- Trong view -->
@{ var videoModel = new VideoPlayerViewModel { Title = "Tên Video", VideoUrl =
"path/to/video.mp4", PosterUrl = "path/to/poster.jpg" }; } @await
Html.PartialAsync("_VideoPlayer", videoModel)
```

### 2. Với Đa Chất Lượng

```html
@{ var videoModel = new VideoPlayerViewModel { Title = "Video Đa Chất Lượng",
VideoUrl = "path/to/1080p.mp4", // 1080p VideoUrl720 = "path/to/720p.mp4", //
720p VideoUrl480 = "path/to/480p.mp4", // 480p VideoUrl360 = "path/to/360p.mp4"
// 360p }; }
```

### 3. Với Phụ Đề

```html
@{ var videoModel = new VideoPlayerViewModel { Title = "Video Có Phụ Đề",
VideoUrl = "path/to/video.mp4", Subtitles = new List<SubtitleTrack>
  { new SubtitleTrack { Label = "Tiếng Việt", Url = "/subtitles/vi.vtt",
  Language = "vi" }, new SubtitleTrack { Label = "English", Url =
  "/subtitles/en.vtt", Language = "en" } } }; }</SubtitleTrack
>
```

### 4. Với Tùy Chọn Tải Xuống

```html
@{ var videoModel = new VideoPlayerViewModel { Title = "Video Có Link Tải",
VideoUrl = "path/to/video.mp4", ShowDownloadOptions = true, DownloadLinks = new
List<DownloadLink>
  { new DownloadLink { Quality = "1080p", Url = "download/1080p.mp4", Size =
  "2.5GB" }, new DownloadLink { Quality = "720p", Url = "download/720p.mp4",
  Size = "1.2GB" } } }; }</DownloadLink
>
```

## 📱 Responsive Design

Player tự động thích ứng với:

- **Desktop**: Đầy đủ controls và tính năng
- **Tablet**: Tối ưu hóa touch controls
- **Mobile**: Controls lớn hơn, dễ chạm

## 🎨 Tùy Chỉnh CSS

### Thay Đổi Màu Chủ Đạo

```css
:root {
  --player-primary: #your-color;
  --player-secondary: #your-secondary-color;
}
```

### Tùy Chỉnh Controls

```css
.ccfilm-video-player .control-btn {
  /* Custom button styles */
}

.ccfilm-video-player .progress-bar {
  /* Custom progress bar */
}
```

## 🔧 JavaScript API

### Khởi Tạo Player

```javascript
const player = new CCFilmVideoPlayer("#myPlayer", {
  src: "video.mp4",
  autoplay: false,
  muted: false,
  qualities: [
    { label: "1080p", src: "video_1080p.mp4" },
    { label: "720p", src: "video_720p.mp4" },
  ],
});
```

### Điều Khiển Qua JavaScript

```javascript
// Phát video
player.play();

// Dừng video
player.pause();

// Tua đến thời điểm cụ thể
player.setCurrentTime(30); // 30 giây

// Thay đổi âm lượng
player.setVolume(0.5); // 50%

// Lấy thông tin
const currentTime = player.getCurrentTime();
const duration = player.getDuration();
const isPaused = player.isPaused();
```

## 📂 Cấu Trúc Files

```
wwwroot/
├── css/
│   └── video-player.css         # CSS cho player
├── js/
│   └── video-player.js          # JavaScript player logic
└── ...

Views/
├── Shared/
│   └── _VideoPlayer.cshtml      # Partial view
└── ...

Models/
└── VideoPlayerViewModel.cs      # View model
```

## 🚀 Demo

Để xem demo đầy đủ:

1. Chạy ứng dụng
2. Truy cập `/Phim/VideoPlayer` hoặc `/Home/VideoPlayerDemo`
3. Test các tính năng

## 🐛 Xử Lý Lỗi

### Video Không Phát

- Kiểm tra đường dẫn video
- Đảm bảo format được hỗ trợ (MP4, WebM)
- Kiểm tra CORS headers

### Controls Không Hiện

- Đảm bảo CSS được load
- Kiểm tra JavaScript không có lỗi
- Verify DOM structure

### Fullscreen Không Hoạt Động

- Kiểm tra browser support
- User action required (click button)

## 🔄 Updates

### Version 1.0

- ✅ Basic playback controls
- ✅ Quality switching
- ✅ Speed control
- ✅ Fullscreen support
- ✅ Keyboard shortcuts
- ✅ Mobile responsive

### Roadmap

- 🔄 Advanced subtitle styling
- 🔄 Playlist support
- 🔄 Video analytics
- 🔄 Adaptive bitrate streaming
