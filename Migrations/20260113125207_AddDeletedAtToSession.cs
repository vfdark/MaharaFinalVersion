using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharaFinalVersion.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeleteReason",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sessions");
        }
    }
}
