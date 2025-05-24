using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class mmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.InsertData(
                table: "DeGiay",
                columns: new[] { "Id", "TenDeGiay" },
                values: new object[,]
                {
                    { new Guid("8f8d4a1e-2bfa-4e8c-9d2c-3f6a7e9b81cb"), "Nhựa" },
                    { new Guid("9f4d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"), "Cao Su" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"), null, "Admin", "ADMIN" },
                    { new Guid("8d04dce2-969a-435d-bba4-df3f325984dc"), null, "Nhân Viên", "NHÂN VIÊN" },
                    { new Guid("8d04dce2-979a-435d-bba4-df3f325983dc"), null, "Khách Hàng", "KHÁCH HÀNG" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "TrangThai" },
                values: new object[] { "6073709c-14ce-4c9a-99b6-0e6ad6e49404", "AQAAAAIAAYagAAAAEFlNK8hcziWGdQzUmzTDKz5Jse7Tj699Q9kPe/3gZOCKpGCVhP10bAG4B/MQl3Fx2Q==", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeGiay",
                keyColumn: "Id",
                keyValue: new Guid("8f8d4a1e-2bfa-4e8c-9d2c-3f6a7e9b81cb"));

            migrationBuilder.DeleteData(
                table: "DeGiay",
                keyColumn: "Id",
                keyValue: new Guid("9f4d4a6e-2bfa-4e8c-9d2c-3f6a7e9b87cb"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325984dc"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-979a-435d-bba4-df3f325983dc"));

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8d04dce2-969a-435d-bba4-df3f325983dc", null, "Admin", "Admin" },
                    { "8d04dce2-969a-435d-bba4-df3f325984dc", null, "Nhân Viên", "Nhân Viên" },
                    { "8d04dce2-979a-435d-bba4-df3f325983dc", null, "Khách Hàng", "Khách Hàng" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "TrangThai" },
                values: new object[] { "50f4a692-4c79-4154-b877-6939187e984f", "AQAAAAIAAYagAAAAEP272MDrgPGxakJxhoi/umNDRP+IV0feT/Q/IOHBx4qON+HFIKRJEbTaY3JYUrcMgw==", false });
        }
    }
}
