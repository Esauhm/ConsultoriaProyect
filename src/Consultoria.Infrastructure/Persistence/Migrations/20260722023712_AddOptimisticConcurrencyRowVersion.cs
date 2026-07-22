using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consultoria.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOptimisticConcurrencyRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PaquetesServicio",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Consultores",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AreasEspecializacion",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PaquetesServicio");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Consultores");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AreasEspecializacion");
        }
    }
}
