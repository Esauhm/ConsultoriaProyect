using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consultoria.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreasEspecializacion",
                columns: table => new
                {
                    AreaEspecializacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasEspecializacion", x => x.AreaEspecializacionId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.CheckConstraint("CK_Usuarios_Rol", "[Rol] IN ('Admin', 'User')");
                });

            migrationBuilder.CreateTable(
                name: "Consultores",
                columns: table => new
                {
                    ConsultorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AreaEspecializacionId = table.Column<int>(type: "int", nullable: false),
                    TarifaHora = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EmailCorporativo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultores", x => x.ConsultorId);
                    table.CheckConstraint("CK_Consultores_TarifaHora", "[TarifaHora] >= 30 AND [TarifaHora] <= 200");
                    table.ForeignKey(
                        name: "FK_Consultores_AreasEspecializacion",
                        column: x => x.AreaEspecializacionId,
                        principalTable: "AreasEspecializacion",
                        principalColumn: "AreaEspecializacionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaquetesServicio",
                columns: table => new
                {
                    PaqueteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AreaEspecializacionId = table.Column<int>(type: "int", nullable: false),
                    ConsultorId = table.Column<int>(type: "int", nullable: false),
                    DuracionHoras = table.Column<int>(type: "int", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaquetesServicio", x => x.PaqueteId);
                    table.CheckConstraint("CK_PaquetesServicio_Costo", "[Costo] > 0");
                    table.CheckConstraint("CK_PaquetesServicio_DuracionHoras", "[DuracionHoras] > 0");
                    table.ForeignKey(
                        name: "FK_PaquetesServicio_AreasEspecializacion",
                        column: x => x.AreaEspecializacionId,
                        principalTable: "AreasEspecializacion",
                        principalColumn: "AreaEspecializacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaquetesServicio_Consultores",
                        column: x => x.ConsultorId,
                        principalTable: "Consultores",
                        principalColumn: "ConsultorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_AreasEspecializacion_Nombre",
                table: "AreasEspecializacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultores_AreaEspecializacionId",
                table: "Consultores",
                column: "AreaEspecializacionId");

            migrationBuilder.CreateIndex(
                name: "UX_Consultores_EmailCorporativo",
                table: "Consultores",
                column: "EmailCorporativo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Consultores_Nombre_AreaEspecializacionId",
                table: "Consultores",
                columns: new[] { "Nombre", "AreaEspecializacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicio_AreaEspecializacionId",
                table: "PaquetesServicio",
                column: "AreaEspecializacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicio_AreaEspecializacionId_Activo",
                table: "PaquetesServicio",
                columns: new[] { "AreaEspecializacionId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicio_ConsultorId",
                table: "PaquetesServicio",
                column: "ConsultorId");

            migrationBuilder.CreateIndex(
                name: "IX_PaquetesServicio_ConsultorId_Activo",
                table: "PaquetesServicio",
                columns: new[] { "ConsultorId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Rol_Activo",
                table: "Usuarios",
                columns: new[] { "Rol", "Activo" });

            migrationBuilder.CreateIndex(
                name: "UX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaquetesServicio");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Consultores");

            migrationBuilder.DropTable(
                name: "AreasEspecializacion");
        }
    }
}
