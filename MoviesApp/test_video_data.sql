-- Script để thêm dữ liệu test cho video player
-- Tạo tập phim test với video URL từ CDN

-- Kiểm tra có phim nào chưa
SELECT TOP 5 MaPhim, TenPhim FROM Phim;

-- Thêm tập phim test với video URL mẫu từ CDN
-- URL mẫu từ CDN service
INSERT INTO TapPhim (MaTap, MaPhim, SoTapThuTu, TenTap, ChiTiet, VideoUrl, ThoiLuongTap, NgayPhatHanh)
VALUES 
    ('T001_01', (SELECT TOP 1 MaPhim FROM Phim), 1, 'Tập 1 - Khởi đầu', 'Tập phim đầu tiên của series', 'http://localhost:5288/videos/episode_test_01/master.m3u8', 45, GETDATE()),
    ('T001_02', (SELECT TOP 1 MaPhim FROM Phim), 2, 'Tập 2 - Phát triển', 'Câu chuyện tiếp tục phát triển', 'http://localhost:5288/videos/episode_test_02/master.m3u8', 48, GETDATE()),
    ('T001_00', (SELECT TOP 1 MaPhim FROM Phim), 0, 'Trailer chính thức', 'Trailer giới thiệu bộ phim', 'http://localhost:5288/videos/trailer_test/master.m3u8', 3, GETDATE());

-- Kiểm tra kết quả
SELECT t.MaTap, t.SoTapThuTu, t.TenTap, t.VideoUrl, p.TenPhim 
FROM TapPhim t 
INNER JOIN Phim p ON t.MaPhim = p.MaPhim 
ORDER BY t.SoTapThuTu;
