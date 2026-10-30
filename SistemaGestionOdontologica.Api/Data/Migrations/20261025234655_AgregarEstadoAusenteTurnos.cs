using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionOdontologica.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoAusenteTurnos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_turnos_estado",
                table: "turnos");

            migrationBuilder.AddCheckConstraint(
                name: "CK_turnos_estado",
                table: "turnos",
                sql: "[estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado', 'ausente')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) // Este método sirve para revertir cambios si fuera necesario.
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_turnos_estado",
                table: "turnos");

            migrationBuilder.AddCheckConstraint(
                name: "CK_turnos_estado",
                table: "turnos",
                sql: "[estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado')");
        }
    }
}
