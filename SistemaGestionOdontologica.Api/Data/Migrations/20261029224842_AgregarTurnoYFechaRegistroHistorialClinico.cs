using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionOdontologica.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTurnoYFechaRegistroHistorialClinico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_registro_utc",
                table: "historial_clinico",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "id_turno",
                table: "historial_clinico",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_historial_clinico_id_turno",
                table: "historial_clinico",
                column: "id_turno");

            migrationBuilder.AddForeignKey(
                name: "FK_historial_clinico_turnos_id_turno",
                table: "historial_clinico",
                column: "id_turno",
                principalTable: "turnos",
                principalColumn: "id_turno",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) // Este método sirve para revertir cambios si fuera necesario.
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historial_clinico_turnos_id_turno",
                table: "historial_clinico");

            migrationBuilder.DropIndex(
                name: "IX_historial_clinico_id_turno",
                table: "historial_clinico");

            migrationBuilder.DropColumn(
                name: "fecha_registro_utc",
                table: "historial_clinico");

            migrationBuilder.DropColumn(
                name: "id_turno",
                table: "historial_clinico");
        }
    }
}
