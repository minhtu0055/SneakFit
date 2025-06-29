using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "b2638009-08a5-4afb-8d77-04354095227f", "AQAAAAIAAYagAAAAEOSfjoTO+Tv3kGdQ3uTzR79IKtVGYyP8nprvgyHTQXPyhbW+5bZpnKioc6jdl60o0g==" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "e2dcbf7c-a714-4583-b7de-0fb6d5bd48d2", "AQAAAAIAAYagAAAAEIJMlDDZ6/tpVvRobqjujUlFUJe+Y27qdrQ1J+PFbHsuFX7oBQCqlUr9i142ElAAeg==" });
        }
    }
}
