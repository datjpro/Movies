-- Xuất dữ liệu Database CCFilm Movies
-- Script tạo bởi Admin Panel
-- Ngày tạo: 23/06/2025

-- ================================================
-- 1. BẢNG DANH MỤC PHIM
-- ================================================
SELECT 
    'INSERT INTO DanhMuc (MaDM, TenDanhMuc, MoTa) VALUES (''' + 
    MaDM + ''', ''' + TenDanhMuc + ''', ''' + ISNULL(MoTa, '') + ''');'
FROM DanhMuc
ORDER BY MaDM;

-- ================================================
-- 2. BẢNG QUỐC GIA
-- ================================================
SELECT 
    'INSERT INTO QuocGia (MaQG, TenQuocGia, MoTa) VALUES (''' + 
    MaQG + ''', ''' + TenQuocGia + ''', ''' + ISNULL(MoTa, '') + ''');'
FROM QuocGia
ORDER BY MaQG;

-- ================================================
-- 3. BẢNG THỂ LOẠI PHIM
-- ================================================
SELECT 
    'INSERT INTO TheLoaiPhim (MaTL, TenTheLoai, MoTa) VALUES (''' + 
    MaTL + ''', ''' + TenTheLoai + ''', ''' + ISNULL(MoTa, '') + ''');'
FROM TheLoaiPhim
ORDER BY MaTL;

-- ================================================
-- 4. BẢNG PHIM
-- ================================================
SELECT 
    'INSERT INTO Phim (MaPhim, TenPhim, MoTa, AnhPhim, DaoDien, DienVien, NamSanXuat, ThoiLuong, DiemImdb, NgayTao, NgayCapNhat, MaDM, MaQG, MaTL, LinkPhim) VALUES (''' + 
    MaPhim + ''', ''' + TenPhim + ''', ''' + ISNULL(MoTa, '') + ''', ''' + ISNULL(AnhPhim, '') + ''', ''' + 
    ISNULL(DaoDien, '') + ''', ''' + ISNULL(DienVien, '') + ''', ' + CAST(NamSanXuat AS VARCHAR) + ', ' + 
    ISNULL(CAST(ThoiLuong AS VARCHAR), 'NULL') + ', ' + ISNULL(CAST(DiemImdb AS VARCHAR), 'NULL') + ', ''' + 
    CONVERT(VARCHAR, NgayTao, 120) + ''', ''' + CONVERT(VARCHAR, NgayCapNhat, 120) + ''', ''' + 
    ISNULL(MaDM, '') + ''', ''' + ISNULL(MaQG, '') + ''', ''' + ISNULL(MaTL, '') + ''', ''' + 
    ISNULL(LinkPhim, '') + ''');'
FROM Phim
ORDER BY NgayTao;

-- ================================================
-- 5. BẢNG TẬP PHIM
-- ================================================
SELECT 
    'INSERT INTO TapPhim (MaTap, TenTap, SoTap, LinkTap, ThoiLuong, NgayTao, MaPhim) VALUES (''' + 
    MaTap + ''', ''' + TenTap + ''', ' + CAST(SoTap AS VARCHAR) + ', ''' + ISNULL(LinkTap, '') + ''', ' + 
    ISNULL(CAST(ThoiLuong AS VARCHAR), 'NULL') + ', ''' + CONVERT(VARCHAR, NgayTao, 120) + ''', ''' + 
    MaPhim + ''');'
FROM TapPhim
ORDER BY MaPhim, SoTap;

-- ================================================
-- 6. BẢNG NGƯỜI DÙNG (Không bao gồm mật khẩu)
-- ================================================
SELECT 
    'INSERT INTO NguoiDung (MaND, TenDangNhap, Email, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, NgayTao, TrangThai, MaLoai) VALUES (''' + 
    MaND + ''', ''' + TenDangNhap + ''', ''' + Email + ''', ''' + ISNULL(HoTen, '') + ''', ''' + 
    ISNULL(CONVERT(VARCHAR, NgaySinh, 120), '') + ''', ''' + ISNULL(GioiTinh, '') + ''', ''' + 
    ISNULL(DiaChi, '') + ''', ''' + ISNULL(SoDienThoai, '') + ''', ''' + CONVERT(VARCHAR, NgayTao, 120) + ''', ' + 
    CAST(TrangThai AS VARCHAR) + ', ''' + ISNULL(MaLoai, '') + ''');'
FROM NguoiDung
ORDER BY NgayTao;

-- ================================================
-- 7. BẢNG ĐÁNH GIÁ
-- ================================================
SELECT 
    'INSERT INTO DanhGia (MaDG, ThoiGianDG, DiemDG, NhanXet, MaTap, MaND) VALUES (''' + 
    MaDG + ''', ''' + CONVERT(VARCHAR, ThoiGianDG, 120) + ''', ' + CAST(DiemDG AS VARCHAR) + ', ''' + 
    ISNULL(NhanXet, '') + ''', ''' + MaTap + ''', ''' + MaND + ''');'
FROM DanhGia
ORDER BY ThoiGianDG;

-- ================================================
-- 8. BẢNG BÌNH LUẬN
-- ================================================
SELECT 
    'INSERT INTO BinhLuan (MaBL, NoiDung, NgayBL, NgaySuaDoi, MaND, MaTap, TrangThai) VALUES (''' + 
    MaBL + ''', ''' + NoiDung + ''', ''' + CONVERT(VARCHAR, NgayBL, 120) + ''', ''' + 
    ISNULL(CONVERT(VARCHAR, NgaySuaDoi, 120), '') + ''', ''' + MaND + ''', ''' + MaTap + ''', ' + 
    CAST(TrangThai AS VARCHAR) + ');'
FROM BinhLuan
ORDER BY NgayBL;

-- ================================================
-- 9. BẢNG LỊCH SỬ XEM
-- ================================================
SELECT 
    'INSERT INTO LichSuXem (MaLS, ThoiGianXem, ThoiGianDungLai, MaND, MaTap) VALUES (''' + 
    MaLS + ''', ''' + CONVERT(VARCHAR, ThoiGianXem, 120) + ''', ' + 
    ISNULL(CAST(ThoiGianDungLai AS VARCHAR), 'NULL') + ', ''' + MaND + ''', ''' + MaTap + ''');'
FROM LichSuXem
ORDER BY ThoiGianXem;

-- ================================================
-- 10. BẢNG PHIM YÊU THÍCH
-- ================================================
SELECT 
    'INSERT INTO PhimYeuThich (MaPYT, NgayThem, MaND, MaPhim) VALUES (''' + 
    MaPYT + ''', ''' + CONVERT(VARCHAR, NgayThem, 120) + ''', ''' + MaND + ''', ''' + MaPhim + ''');'
FROM PhimYeuThich
ORDER BY NgayThem;

-- ================================================
-- THỐNG KÊ DATABASE
-- ================================================
SELECT 
    'Tổng số bảng: 10' AS ThongKe
UNION ALL
SELECT 
    'Tổng số phim: ' + CAST(COUNT(*) AS VARCHAR) 
FROM Phim
UNION ALL
SELECT 
    'Tổng số tập phim: ' + CAST(COUNT(*) AS VARCHAR) 
FROM TapPhim
UNION ALL
SELECT 
    'Tổng số người dùng: ' + CAST(COUNT(*) AS VARCHAR) 
FROM NguoiDung
UNION ALL
SELECT 
    'Tổng số đánh giá: ' + CAST(COUNT(*) AS VARCHAR) 
FROM DanhGia
UNION ALL
SELECT 
    'Tổng số bình luận: ' + CAST(COUNT(*) AS VARCHAR) 
FROM BinhLuan;

-- ================================================
-- HƯỚNG DẪN SỬ DỤNG
-- ================================================
/*
Cách sử dụng file SQL này:

1. MỞ SQL SERVER MANAGEMENT STUDIO hoặc AZURE DATA STUDIO
2. Kết nối đến server database của bạn
3. Tạo database mới (nếu cần):
   CREATE DATABASE CCFilm_Backup;
   
4. Chạy script tạo bảng trước (từ Entity Framework migrations)
5. Sau đó chạy các script INSERT ở trên để thêm dữ liệu

LƯU Ý:
- Script này chỉ xuất dữ liệu, không bao gồm cấu trúc bảng
- Mật khẩu người dùng không được xuất vì lý do bảo mật
- Kiểm tra foreign key constraints trước khi chạy
- Backup dữ liệu hiện tại trước khi import

Để tạo cấu trúc bảng, sử dụng Entity Framework:
dotnet ef migrations script > database_structure.sql
*/
