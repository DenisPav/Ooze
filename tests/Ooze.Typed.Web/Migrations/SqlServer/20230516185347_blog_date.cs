using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ooze.Typed.Web.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class blogdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Blog",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Blog");
        }
    }
}
