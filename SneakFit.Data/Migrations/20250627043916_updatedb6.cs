using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedb6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "fb1aba80-b655-436b-ab72-be79053103f9", "AQAAAAIAAYagAAAAEMvPMPR0py/4r/WJIptqNt4BA1vH4iThXN0UypBQ2jU5YgPZDpzWPGbuaVZ/OO87/Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "912f8e26-24c3-4134-b866-21ff27ffec0a", "AQAAAAIAAYagAAAAEOzZIPwkPfVXfzQWNKr5DjD6jfxSE3xvXnFf4Nlkrk9xO1/ine7tZOl+fP0tVDsqmQ==" });
        }
    }
}
