using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionOdontologica.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "odontologos",
                columns: table => new
                {
                    id_odontologo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    matricula = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odontologos", x => x.id_odontologo);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id_paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dni = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    fecha_nacimiento = table.Column<DateTime>(type: "date", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.id_paciente);
                });

            migrationBuilder.CreateTable(
                name: "tratamientos",
                columns: table => new
                {
                    id_tratamiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tratamientos", x => x.id_tratamiento);
                    table.CheckConstraint("CK_tratamientos_precio", "[precio] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_odontologo = table.Column<int>(type: "int", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    ultimo_acceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.CheckConstraint("CK_usuarios_rol", "[rol] IN ('admin', 'odontologo', 'recepcionista')");
                    table.ForeignKey(
                        name: "FK_usuarios_odontologos_id_odontologo",
                        column: x => x.id_odontologo,
                        principalTable: "odontologos",
                        principalColumn: "id_odontologo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "historial_clinico",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_odontologo = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    motivo_consulta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    diagnostico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_clinico", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK_historial_clinico_odontologos_id_odontologo",
                        column: x => x.id_odontologo,
                        principalTable: "odontologos",
                        principalColumn: "id_odontologo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historial_clinico_pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    monto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    metodo_pago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id_pago);
                    table.CheckConstraint("CK_pagos_metodo", "[metodo_pago] IN ('efectivo', 'tarjeta', 'transferencia')");
                    table.CheckConstraint("CK_pagos_monto", "[monto] > 0");
                    table.ForeignKey(
                        name: "FK_pagos_pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "turnos",
                columns: table => new
                {
                    id_turno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_odontologo = table.Column<int>(type: "int", nullable: false),
                    fecha_hora_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_hora_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pendiente"),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turnos", x => x.id_turno);
                    table.CheckConstraint("CK_turnos_estado", "[estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado')");
                    table.CheckConstraint("CK_turnos_fechas", "[fecha_hora_fin] > [fecha_hora_inicio]");
                    table.ForeignKey(
                        name: "FK_turnos_odontologos_id_odontologo",
                        column: x => x.id_odontologo,
                        principalTable: "odontologos",
                        principalColumn: "id_odontologo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_turnos_pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bitacora",
                columns: table => new
                {
                    id_bitacora = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    detalles = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bitacora", x => x.id_bitacora);
                    table.ForeignKey(
                        name: "FK_bitacora_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "archivos_clinicos",
                columns: table => new
                {
                    id_archivo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_historial = table.Column<int>(type: "int", nullable: false),
                    nombre_original = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nombre_archivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ruta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    tipo_mime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tamano_bytes = table.Column<long>(type: "bigint", nullable: false),
                    fecha_subida = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_archivos_clinicos", x => x.id_archivo);
                    table.ForeignKey(
                        name: "FK_archivos_clinicos_historial_clinico_id_historial",
                        column: x => x.id_historial,
                        principalTable: "historial_clinico",
                        principalColumn: "id_historial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historial_tratamientos",
                columns: table => new
                {
                    id_historial_tratamiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_historial = table.Column<int>(type: "int", nullable: false),
                    id_tratamiento = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_tratamientos", x => x.id_historial_tratamiento);
                    table.CheckConstraint("CK_historial_tratamientos_cantidad", "[cantidad] > 0");
                    table.CheckConstraint("CK_historial_tratamientos_precio", "[precio_unitario] >= 0");
                    table.ForeignKey(
                        name: "FK_historial_tratamientos_historial_clinico_id_historial",
                        column: x => x.id_historial,
                        principalTable: "historial_clinico",
                        principalColumn: "id_historial",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historial_tratamientos_tratamientos_id_tratamiento",
                        column: x => x.id_tratamiento,
                        principalTable: "tratamientos",
                        principalColumn: "id_tratamiento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recordatorios_turno",
                columns: table => new
                {
                    id_recordatorio = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_turno = table.Column<int>(type: "int", nullable: false),
                    fecha_programada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    enviado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fecha_envio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recordatorios_turno", x => x.id_recordatorio);
                    table.CheckConstraint("CK_recordatorios_tipo", "[tipo] IN ('email','whatsapp', 'sms')");
                    table.ForeignKey(
                        name: "FK_recordatorios_turno_turnos_id_turno",
                        column: x => x.id_turno,
                        principalTable: "turnos",
                        principalColumn: "id_turno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_archivos_clinicos_id_historial",
                table: "archivos_clinicos",
                column: "id_historial");

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_id_usuario_fecha",
                table: "bitacora",
                columns: new[] { "id_usuario", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_historial_clinico_id_odontologo",
                table: "historial_clinico",
                column: "id_odontologo");

            migrationBuilder.CreateIndex(
                name: "IX_historial_clinico_id_paciente",
                table: "historial_clinico",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_historial_tratamientos_id_historial",
                table: "historial_tratamientos",
                column: "id_historial");

            migrationBuilder.CreateIndex(
                name: "IX_historial_tratamientos_id_tratamiento",
                table: "historial_tratamientos",
                column: "id_tratamiento");

            migrationBuilder.CreateIndex(
                name: "IX_odontologos_matricula",
                table: "odontologos",
                column: "matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_dni",
                table: "pacientes",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_id_paciente",
                table: "pagos",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_recordatorios_turno_enviado_fecha_programada",
                table: "recordatorios_turno",
                columns: new[] { "enviado", "fecha_programada" });

            migrationBuilder.CreateIndex(
                name: "IX_recordatorios_turno_id_turno",
                table: "recordatorios_turno",
                column: "id_turno");

            migrationBuilder.CreateIndex(
                name: "IX_turnos_id_odontologo_fecha_hora_inicio",
                table: "turnos",
                columns: new[] { "id_odontologo", "fecha_hora_inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_turnos_id_paciente",
                table: "turnos",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_odontologo",
                table: "usuarios",
                column: "id_odontologo");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_username",
                table: "usuarios",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "archivos_clinicos");

            migrationBuilder.DropTable(
                name: "bitacora");

            migrationBuilder.DropTable(
                name: "historial_tratamientos");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "recordatorios_turno");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "historial_clinico");

            migrationBuilder.DropTable(
                name: "tratamientos");

            migrationBuilder.DropTable(
                name: "turnos");

            migrationBuilder.DropTable(
                name: "odontologos");

            migrationBuilder.DropTable(
                name: "pacientes");
        }
    }
}
