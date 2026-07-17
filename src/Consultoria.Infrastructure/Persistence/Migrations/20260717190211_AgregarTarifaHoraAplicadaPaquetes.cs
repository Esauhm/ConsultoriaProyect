using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consultoria.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTarifaHoraAplicadaPaquetes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TarifaHoraAplicada",
                table: "PaquetesServicio",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.Sql(
                """
        UPDATE [PaquetesServicio]
        SET [TarifaHoraAplicada] =
            ROUND(
                [Costo] / NULLIF([DuracionHoras], 0),
                2
            )
        WHERE [TarifaHoraAplicada] IS NULL;
        """);

            migrationBuilder.AlterColumn<decimal>(
                name: "TarifaHoraAplicada",
                table: "PaquetesServicio",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PaquetesServicio_TarifaHoraAplicada",
                table: "PaquetesServicio",
                sql: "[TarifaHoraAplicada] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PaquetesServicio_TarifaHoraAplicada",
                table: "PaquetesServicio");

            migrationBuilder.DropColumn(
                name: "TarifaHoraAplicada",
                table: "PaquetesServicio");
        }
    }
}
