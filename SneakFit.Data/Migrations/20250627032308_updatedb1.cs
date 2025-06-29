using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e2dcbf7c-a714-4583-b7de-0fb6d5bd48d2", "AQAAAAIAAYagAAAAEIJMlDDZ6/tpVvRobqjujUlFUJe+Y27qdrQ1J+PFbHsuFX7oBQCqlUr9i142ElAAeg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f03e12d8-5c74-4268-ad57-1e84631bfd33", "AQAAAAIAAYagAAAAEJwU40FMspaEXz56BUp1cdzf2eADBXnV0kL+1bndTRS/lU+oCB54asmBl91L2cYU5g==" });
        }
    }
}
