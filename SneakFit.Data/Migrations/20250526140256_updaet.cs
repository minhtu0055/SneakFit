using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updaet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "DiaChi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenNguoiNhan",
                table: "DiaChi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ffd40b54-5523-4dda-8ff5-baa91570e802", "AQAAAAIAAYagAAAAEFvsQrK8suWIEBLSZyc3ZeoVjkhKtAn65sn+7PJOdiJxGxBMOpLMWRmHEGRcGEHbkg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "DiaChi");

            migrationBuilder.DropColumn(
                name: "TenNguoiNhan",
                table: "DiaChi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "df53d45a-ee5d-4d03-9fe3-8298dc9c3914", "AQAAAAIAAYagAAAAEPSIpCSr4n0opVgLVZz7PU+JS5up7kFnRzUxqNa7vU2JC9MF05avy1trM3n2mw4/dg==" });
        }
    }
}
