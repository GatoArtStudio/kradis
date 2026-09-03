using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kradis.Migrations
{
    /// <inheritdoc />
    public partial class FixGuidDataFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "guilds",
                type: "binary(16)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "binary(36)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "guilds",
                type: "binary(36)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");
        }
    }
}
