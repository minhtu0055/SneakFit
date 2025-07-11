using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class hehe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "loaiVoucher",
                table: "Voucher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SPCTId",
                table: "KhuyenMaiChiTiet",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySuaDoi",
                table: "KhuyenMai",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VoucherUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherUser_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "Id",
                keyValue: new Guid("8f4d4a5e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                column: "TenDanhMuc",
                value: "Sneaker");

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "Id",
                keyValue: new Guid("8f8d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                column: "TenDanhMuc",
                value: "SneakFit");

            migrationBuilder.UpdateData(
                table: "MauSac",
                keyColumn: "Id",
                keyValue: new Guid("8f4d4a5e-2bfa-2e8c-9d2c-3f6a7e9b87cb"),
                column: "TenMauSac",
                value: "Đỏ");

            migrationBuilder.UpdateData(
                table: "MauSac",
                keyColumn: "Id",
                keyValue: new Guid("8f8d4a5e-2bfa-4e9c-9d2c-3f6a7e9b87cb"),
                column: "TenMauSac",
                value: "Trắng");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b4e0e428-2834-4c90-bd89-83501aa2daee", "AQAAAAIAAYagAAAAEIkLQkhzZddCDyiubTnrHUVLeE9XVZsZ/joiSyLPeeN1iPxdlGzQhrVxUhMGzh6iHA==" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUser_UserId",
                table: "VoucherUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUser_VoucherId",
                table: "VoucherUser",
                column: "VoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherUser");

            migrationBuilder.DropColumn(
                name: "loaiVoucher",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "SPCTId",
                table: "KhuyenMaiChiTiet");

            migrationBuilder.DropColumn(
                name: "NgaySuaDoi",
                table: "KhuyenMai");

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "Id",
                keyValue: new Guid("8f4d4a5e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                column: "TenDanhMuc",
                value: "Giày Chạy Bộ");

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "Id",
                keyValue: new Guid("8f8d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"),
                column: "TenDanhMuc",
                value: "Giày Đá Bóng");

            migrationBuilder.UpdateData(
                table: "MauSac",
                keyColumn: "Id",
                keyValue: new Guid("8f4d4a5e-2bfa-2e8c-9d2c-3f6a7e9b87cb"),
                column: "TenMauSac",
                value: "Đen");

            migrationBuilder.UpdateData(
                table: "MauSac",
                keyColumn: "Id",
                keyValue: new Guid("8f8d4a5e-2bfa-4e9c-9d2c-3f6a7e9b87cb"),
                column: "TenMauSac",
                value: "Đỏ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "48040758-b14e-4baf-a3b0-2423d387c371", "AQAAAAIAAYagAAAAEJD/sFjx7n3q1dr9QJN2cv2yFOPgrPst1GRacmKX853aNMnZ9VN0huZaNS1E+vmyxA==" });
        }
    }
}
