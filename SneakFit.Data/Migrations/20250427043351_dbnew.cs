using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "SanPhamChiTiet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "SanPhamChiTiet",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "SanPhamChiTiet");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "SanPhamChiTiet");
        }
    }
}
