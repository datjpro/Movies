using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCDNVideoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VideoFileSize",
                table: "TapPhim",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoFormat",
                table: "TapPhim",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoId",
                table: "TapPhim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoStatus",
                table: "TapPhim",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoFileSize",
                table: "TapPhim");

            migrationBuilder.DropColumn(
                name: "VideoFormat",
                table: "TapPhim");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "TapPhim");

            migrationBuilder.DropColumn(
                name: "VideoStatus",
                table: "TapPhim");
        }
    }
}
