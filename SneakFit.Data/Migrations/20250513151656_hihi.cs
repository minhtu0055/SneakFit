using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class hihi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HinhAnhSanPham_SanPham_SanPhamId",
                table: "HinhAnhSanPham");

            migrationBuilder.DropIndex(
                name: "IX_HinhAnhSanPham_SanPhamId",
                table: "HinhAnhSanPham");

            migrationBuilder.DropColumn(
                name: "Gia",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "ThoiGianCapNhat",
                table: "KhuyenMai");

            migrationBuilder.DropColumn(
                name: "SanPhamId",
                table: "HinhAnhSanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Gia",
                table: "SanPham",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianCapNhat",
                table: "KhuyenMai",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamId",
                table: "HinhAnhSanPham",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhSanPham_SanPhamId",
                table: "HinhAnhSanPham",
                column: "SanPhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_HinhAnhSanPham_SanPham_SanPhamId",
                table: "HinhAnhSanPham",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "Id");
        }
    }
}
