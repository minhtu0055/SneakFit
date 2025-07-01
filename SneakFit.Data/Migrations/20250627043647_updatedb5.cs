using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedb5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhuyenMaiChiTiet_SanPhamChiTiet_SanPhamChiTietID",
                table: "KhuyenMaiChiTiet");

            migrationBuilder.DropIndex(
                name: "IX_KhuyenMaiChiTiet_SanPhamChiTietID",
                table: "KhuyenMaiChiTiet");

            migrationBuilder.DropColumn(
                name: "SanPhamChiTietID",
                table: "KhuyenMaiChiTiet");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "912f8e26-24c3-4134-b866-21ff27ffec0a", "AQAAAAIAAYagAAAAEOzZIPwkPfVXfzQWNKr5DjD6jfxSE3xvXnFf4Nlkrk9xO1/ine7tZOl+fP0tVDsqmQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamChiTietID",
                table: "KhuyenMaiChiTiet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "25703f40-4856-436e-b1b4-4d5cf6f5d154", "AQAAAAIAAYagAAAAEErnacy59rwdjmOhCB8Suy/ZiW65y+T8giIlHIf4D+NE6sov6GfyAPl+XFAHXyxcCg==" });

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMaiChiTiet_SanPhamChiTietID",
                table: "KhuyenMaiChiTiet",
                column: "SanPhamChiTietID");

            migrationBuilder.AddForeignKey(
                name: "FK_KhuyenMaiChiTiet_SanPhamChiTiet_SanPhamChiTietID",
                table: "KhuyenMaiChiTiet",
                column: "SanPhamChiTietID",
                principalTable: "SanPhamChiTiet",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
