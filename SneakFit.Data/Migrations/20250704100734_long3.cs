using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class long3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaTriToiThieu",
                table: "Voucher",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1f4a59e6-09fc-4fb1-9635-988dd68e5bd0", "AQAAAAIAAYagAAAAEJJYjOmQk4MG2nfIkQ1PhDDeptn3hB5k3RDGgGEXEJJjkrlznoQLzwA4YMDVUD0C2A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaTriToiThieu",
                table: "Voucher");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b4e0e428-2834-4c90-bd89-83501aa2daee", "AQAAAAIAAYagAAAAEIkLQkhzZddCDyiubTnrHUVLeE9XVZsZ/joiSyLPeeN1iPxdlGzQhrVxUhMGzh6iHA==" });
        }
    }
}
