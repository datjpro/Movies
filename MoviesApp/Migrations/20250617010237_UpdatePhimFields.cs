using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhimFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BienKich",
                table: "Phim",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaoDien",
                table: "Phim",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiemImdb",
                table: "Phim",
                type: "decimal(3,1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiemMetascore",
                table: "Phim",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DienVien",
                table: "Phim",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiaiThuong",
                table: "Phim",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Phim",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiPhim",
                table: "Phim",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LuotVoteImdb",
                table: "Phim",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKhoiChieu",
                table: "Phim",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NgonNgu",
                table: "Phim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuocGiaSanXuat",
                table: "Phim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TongSoMua",
                table: "Phim",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XepHang",
                table: "Phim",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BienKich",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "DaoDien",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "DiemImdb",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "DiemMetascore",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "DienVien",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "GiaiThuong",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "LoaiPhim",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "LuotVoteImdb",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "NgayKhoiChieu",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "NgonNgu",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "QuocGiaSanXuat",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "TongSoMua",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "XepHang",
                table: "Phim");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "AspNetUsers");
        }
    }
}
