using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "faa2bf96-4b29-407e-85f0-610151ca8fa8", "AQAAAAIAAYagAAAAELaGER7WB2cCCKUkK1wElbp+OqnnKqZrSK7ci5SCUzKAYdaS6M7Qm3hkp4iirNsYhw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "58535c22-ffbb-4a0c-85f0-2396401f1dbe", "AQAAAAIAAYagAAAAEP4ndZubT6/EoXlEeX8E1b36WngYWZH21dC0qUUrgMpIe9ZdRlJa5Heyc5Pe8Rik3A==" });
        }
    }
}
