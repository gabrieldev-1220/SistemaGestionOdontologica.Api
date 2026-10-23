using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionOdontologica.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarTablaPacienteAPacientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_facturas_paciente_id_paciente",
                table: "facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_historial_clinico_paciente_id_paciente",
                table: "historial_clinico");

            migrationBuilder.DropForeignKey(
                name: "FK_odontogramas_paciente_id_paciente",
                table: "odontogramas");

            migrationBuilder.DropForeignKey(
                name: "FK_pagos_paciente_id_paciente",
                table: "pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_turnos_paciente_id_paciente",
                table: "turnos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_paciente",
                table: "paciente");

            migrationBuilder.RenameTable(
                name: "paciente",
                newName: "pacientes");

            migrationBuilder.RenameIndex(
                name: "IX_paciente_dni",
                table: "pacientes",
                newName: "IX_pacientes_dni");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pacientes",
                table: "pacientes",
                column: "id_paciente");

            migrationBuilder.AddForeignKey(
                name: "FK_facturas_pacientes_id_paciente",
                table: "facturas",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_historial_clinico_pacientes_id_paciente",
                table: "historial_clinico",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_odontogramas_pacientes_id_paciente",
                table: "odontogramas",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pagos_pacientes_id_paciente",
                table: "pagos",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_turnos_pacientes_id_paciente",
                table: "turnos",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_facturas_pacientes_id_paciente",
                table: "facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_historial_clinico_pacientes_id_paciente",
                table: "historial_clinico");

            migrationBuilder.DropForeignKey(
                name: "FK_odontogramas_pacientes_id_paciente",
                table: "odontogramas");

            migrationBuilder.DropForeignKey(
                name: "FK_pagos_pacientes_id_paciente",
                table: "pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_turnos_pacientes_id_paciente",
                table: "turnos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pacientes",
                table: "pacientes");

            migrationBuilder.RenameTable(
                name: "pacientes",
                newName: "paciente");

            migrationBuilder.RenameIndex(
                name: "IX_pacientes_dni",
                table: "paciente",
                newName: "IX_paciente_dni");

            migrationBuilder.AddPrimaryKey(
                name: "PK_paciente",
                table: "paciente",
                column: "id_paciente");

            migrationBuilder.AddForeignKey(
                name: "FK_facturas_paciente_id_paciente",
                table: "facturas",
                column: "id_paciente",
                principalTable: "paciente",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_historial_clinico_paciente_id_paciente",
                table: "historial_clinico",
                column: "id_paciente",
                principalTable: "paciente",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_odontogramas_paciente_id_paciente",
                table: "odontogramas",
                column: "id_paciente",
                principalTable: "paciente",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pagos_paciente_id_paciente",
                table: "pagos",
                column: "id_paciente",
                principalTable: "paciente",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_turnos_paciente_id_paciente",
                table: "turnos",
                column: "id_paciente",
                principalTable: "paciente",
                principalColumn: "id_paciente",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
