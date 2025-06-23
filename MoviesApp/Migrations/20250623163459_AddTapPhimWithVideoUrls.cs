using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTapPhimWithVideoUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    MaDM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenDM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMuc", x => x.MaDM);
                });

            migrationBuilder.CreateTable(
                name: "LoaiNguoiDung",
                columns: table => new
                {
                    MaLoaiND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenLoaiND = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SoNgayHieuLuc = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiNguoiDung", x => x.MaLoaiND);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenND = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Pass = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaND);
                });

            migrationBuilder.CreateTable(
                name: "QuocGia",
                columns: table => new
                {
                    MaQG = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenQG = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnhQG = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuocGia", x => x.MaQG);
                });

            migrationBuilder.CreateTable(
                name: "TheLoaiPhim",
                columns: table => new
                {
                    MaTL = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenTL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheLoaiPhim", x => x.MaTL);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKy",
                columns: table => new
                {
                    MaDK = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayDK = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhuongThucTT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoTien = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLoaiND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKy", x => x.MaDK);
                    table.ForeignKey(
                        name: "FK_DangKy_LoaiNguoiDung_MaLoaiND",
                        column: x => x.MaLoaiND,
                        principalTable: "LoaiNguoiDung",
                        principalColumn: "MaLoaiND",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKy_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phim",
                columns: table => new
                {
                    MaPhim = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaQG = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaTL = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaDM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenPhim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTaPhim = table.Column<string>(type: "ntext", nullable: true),
                    AnhPhim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoTap = table.Column<int>(type: "int", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NamPhatHanh = table.Column<int>(type: "int", nullable: true),
                    ThoiLuongPhim = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BienKich = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DaoDien = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiemImdb = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    DiemMetascore = table.Column<int>(type: "int", nullable: true),
                    DienVien = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GiaiThuong = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImdbId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LoaiPhim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LuotVoteImdb = table.Column<int>(type: "int", nullable: true),
                    NgayKhoiChieu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgonNgu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuocGiaSanXuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TongSoMua = table.Column<int>(type: "int", nullable: true),
                    XepHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LinkPhim = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phim", x => x.MaPhim);
                    table.ForeignKey(
                        name: "FK_Phim_DanhMuc_MaDM",
                        column: x => x.MaDM,
                        principalTable: "DanhMuc",
                        principalColumn: "MaDM",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phim_QuocGia_MaQG",
                        column: x => x.MaQG,
                        principalTable: "QuocGia",
                        principalColumn: "MaQG",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phim_TheLoaiPhim_MaTL",
                        column: x => x.MaTL,
                        principalTable: "TheLoaiPhim",
                        principalColumn: "MaTL",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhimYeuThich",
                columns: table => new
                {
                    MaYT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayYT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaPhim = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhimYeuThich", x => x.MaYT);
                    table.ForeignKey(
                        name: "FK_PhimYeuThich_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhimYeuThich_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhimYeuThich_Phim_MaPhim",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "MaPhim",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TapPhim",
                columns: table => new
                {
                    MaTap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaPhim = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoTapThuTu = table.Column<int>(type: "int", nullable: false),
                    TenTap = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChiTiet = table.Column<string>(type: "ntext", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiLuongTap = table.Column<int>(type: "int", nullable: true),
                    NgayPhatHanh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TapPhim", x => x.MaTap);
                    table.ForeignKey(
                        name: "FK_TapPhim_Phim_MaPhim",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "MaPhim",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongKePhim",
                columns: table => new
                {
                    MaPhim = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TongLuotXem = table.Column<long>(type: "bigint", nullable: false),
                    DiemTrungBinh = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    SoLuotDanhGia = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKePhim", x => x.MaPhim);
                    table.ForeignKey(
                        name: "FK_ThongKePhim_Phim_MaPhim",
                        column: x => x.MaPhim,
                        principalTable: "Phim",
                        principalColumn: "MaPhim",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BinhLuan",
                columns: table => new
                {
                    MaBL = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NoiDung = table.Column<string>(type: "ntext", nullable: false),
                    NgayBL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgaySuaDoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaTap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuan", x => x.MaBL);
                    table.ForeignKey(
                        name: "FK_BinhLuan_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BinhLuan_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BinhLuan_TapPhim_MaTap",
                        column: x => x.MaTap,
                        principalTable: "TapPhim",
                        principalColumn: "MaTap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    MaDG = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThoiGianDG = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiemDG = table.Column<byte>(type: "tinyint", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaTap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.MaDG);
                    table.ForeignKey(
                        name: "FK_DanhGia_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DanhGia_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGia_TapPhim_MaTap",
                        column: x => x.MaTap,
                        principalTable: "TapPhim",
                        principalColumn: "MaTap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuXem",
                columns: table => new
                {
                    MaLS = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThoiDiemXem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianXem = table.Column<int>(type: "int", nullable: false),
                    MaND = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaTap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuXem", x => x.MaLS);
                    table.ForeignKey(
                        name: "FK_LichSuXem_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichSuXem_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuXem_TapPhim_MaTap",
                        column: x => x.MaTap,
                        principalTable: "TapPhim",
                        principalColumn: "MaTap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeTapPhim",
                columns: table => new
                {
                    MaTap = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LuotXem = table.Column<long>(type: "bigint", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeTapPhim", x => x.MaTap);
                    table.ForeignKey(
                        name: "FK_ThongKeTapPhim_TapPhim_MaTap",
                        column: x => x.MaTap,
                        principalTable: "TapPhim",
                        principalColumn: "MaTap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_ApplicationUserId",
                table: "BinhLuan",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaND",
                table: "BinhLuan",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaTap",
                table: "BinhLuan",
                column: "MaTap");

            migrationBuilder.CreateIndex(
                name: "IX_DangKy_MaLoaiND",
                table: "DangKy",
                column: "MaLoaiND");

            migrationBuilder.CreateIndex(
                name: "IX_DangKy_MaND",
                table: "DangKy",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_ApplicationUserId",
                table: "DanhGia",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaND",
                table: "DanhGia",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaTap",
                table: "DanhGia",
                column: "MaTap");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaTap_MaND",
                table: "DanhGia",
                columns: new[] { "MaTap", "MaND" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXem_ApplicationUserId",
                table: "LichSuXem",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXem_MaND",
                table: "LichSuXem",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXem_MaTap",
                table: "LichSuXem",
                column: "MaTap");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXem_ThoiDiemXem",
                table: "LichSuXem",
                column: "ThoiDiemXem");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phim_MaDM",
                table: "Phim",
                column: "MaDM");

            migrationBuilder.CreateIndex(
                name: "IX_Phim_MaQG",
                table: "Phim",
                column: "MaQG");

            migrationBuilder.CreateIndex(
                name: "IX_Phim_MaTL",
                table: "Phim",
                column: "MaTL");

            migrationBuilder.CreateIndex(
                name: "IX_Phim_NamPhatHanh",
                table: "Phim",
                column: "NamPhatHanh");

            migrationBuilder.CreateIndex(
                name: "IX_Phim_TenPhim",
                table: "Phim",
                column: "TenPhim");

            migrationBuilder.CreateIndex(
                name: "IX_PhimYeuThich_ApplicationUserId",
                table: "PhimYeuThich",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhimYeuThich_MaND_MaPhim",
                table: "PhimYeuThich",
                columns: new[] { "MaND", "MaPhim" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhimYeuThich_MaPhim",
                table: "PhimYeuThich",
                column: "MaPhim");

            migrationBuilder.CreateIndex(
                name: "IX_TapPhim_MaPhim",
                table: "TapPhim",
                column: "MaPhim");

            migrationBuilder.CreateIndex(
                name: "IX_TapPhim_MaPhim_SoTapThuTu",
                table: "TapPhim",
                columns: new[] { "MaPhim", "SoTapThuTu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BinhLuan");

            migrationBuilder.DropTable(
                name: "DangKy");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "LichSuXem");

            migrationBuilder.DropTable(
                name: "PhimYeuThich");

            migrationBuilder.DropTable(
                name: "ThongKePhim");

            migrationBuilder.DropTable(
                name: "ThongKeTapPhim");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "LoaiNguoiDung");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "TapPhim");

            migrationBuilder.DropTable(
                name: "Phim");

            migrationBuilder.DropTable(
                name: "DanhMuc");

            migrationBuilder.DropTable(
                name: "QuocGia");

            migrationBuilder.DropTable(
                name: "TheLoaiPhim");
        }
    }
}
