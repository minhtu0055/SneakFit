using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class upte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangChiTiet_SanPhamChiTiet_SanPhamChiTietId",
                table: "GioHangChiTiet");

            migrationBuilder.AlterColumn<Guid>(
                name: "SanPhamChiTietId",
                table: "GioHangChiTiet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "GioHang",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "eb20ebc6-8bbd-4ef9-a5f1-e446134f2110", "AQAAAAIAAYagAAAAEFg5UlMETjVR0HHV2ctqAtZaOTcX7JwTvDGwkucYcoQIUAtSIDZ/QWl6O8dimLBCMQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangChiTiet_SanPhamChiTiet_SanPhamChiTietId",
                table: "GioHangChiTiet",
                column: "SanPhamChiTietId",
                principalTable: "SanPhamChiTiet",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangChiTiet_SanPhamChiTiet_SanPhamChiTietId",
                table: "GioHangChiTiet");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "GioHang");

            migrationBuilder.AlterColumn<Guid>(
                name: "SanPhamChiTietId",
                table: "GioHangChiTiet",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6073709c-14ce-4c9a-99b6-0e6ad6e49404", "AQAAAAIAAYagAAAAEFlNK8hcziWGdQzUmzTDKz5Jse7Tj699Q9kPe/3gZOCKpGCVhP10bAG4B/MQl3Fx2Q==" });

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangChiTiet_SanPhamChiTiet_SanPhamChiTietId",
                table: "GioHangChiTiet",
                column: "SanPhamChiTietId",
                principalTable: "SanPhamChiTiet",
                principalColumn: "ID");
        }
    }
}
