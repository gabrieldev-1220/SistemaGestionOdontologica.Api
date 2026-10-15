using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionOdontologica.Api.Data.Migrations
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
                name: "paciente",
                columns: table => new
                {
                    id_paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dni = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paciente", x => x.id_paciente);
                });

            migrationBuilder.CreateTable(
                name: "tratamientos",
                columns: table => new
                {
                    id_tratamiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.CheckConstraint("CK_usuarios_odontologo", "([rol] = 'odontologo' AND[id_odontologo] IS NOT NULL) OR([rol] <> 'odontologo' AND[id_odontologo] IS NULL)");
                    table.CheckConstraint("CK_usuarios_rol", "[rol] IN ('admin', 'odontologo', 'asistente')");
                    table.ForeignKey(
                        name: "FK_usuarios_odontologos_id_odontologo",
                        column: x => x.id_odontologo,
                        principalTable: "odontologos",
                        principalColumn: "id_odontologo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    id_factura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "emitida"),
                    total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.id_factura);
                    table.CheckConstraint("CK_facturas_estado", "[estado] IN ('emitida', 'pagada', 'anulada')");
                    table.CheckConstraint("CK_facturas_total", "[total] >= 0");
                    table.ForeignKey(
                        name: "FK_facturas_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "paciente",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_historial_clinico_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "paciente",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "odontogramas",
                columns: table => new
                {
                    id_odontograma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_odontologo = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odontogramas", x => x.id_odontograma);
                    table.ForeignKey(
                        name: "FK_odontogramas_odontologos_id_odontologo",
                        column: x => x.id_odontologo,
                        principalTable: "odontologos",
                        principalColumn: "id_odontologo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_odontogramas_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "paciente",
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
                    monto = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    metodo_pago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.id_pago);
                    table.CheckConstraint("CK_pagos_metodo", "[metodo_pago] IN ('efectivo', 'tarjeta', 'Transferencia')");
                    table.CheckConstraint("CK_pagos_monto", "[monto] > 0");
                    table.ForeignKey(
                        name: "FK_pagos_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "paciente",
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
                        name: "FK_turnos_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "paciente",
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
                name: "refresh_tokens",
                columns: table => new
                {
                    id_refresh_token = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_revocacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id_refresh_token);
                    table.CheckConstraint("CK_refresh_tokens_fechas", "[fecha_expiracion] > [fecha_creacion]");
                    table.CheckConstraint("CK_refresh_tokens_revocacion", "[fecha_revocacion] IS NULL OR [fecha_revocacion] >= [fecha_creacion]");
                    table.ForeignKey(
                        name: "FK_refresh_tokens_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "factura_detalles",
                columns: table => new
                {
                    id_factura_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_factura = table.Column<int>(type: "int", nullable: false),
                    id_tratamiento = table.Column<int>(type: "int", nullable: true),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    precio_unitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_factura_detalles", x => x.id_factura_detalle);
                    table.CheckConstraint("CK_factura_detalles_cantidad", "[cantidad] > 0");
                    table.CheckConstraint("CK_factura_detalles_precio", "[precio_unitario] >= 0");
                    table.ForeignKey(
                        name: "FK_factura_detalles_facturas_id_factura",
                        column: x => x.id_factura,
                        principalTable: "facturas",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_factura_detalles_tratamientos_id_tratamiento",
                        column: x => x.id_tratamiento,
                        principalTable: "tratamientos",
                        principalColumn: "id_tratamiento",
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
                    table.CheckConstraint("CK_archivos_tamano", "[tamano_bytes] > 0");
                    table.ForeignKey(
                        name: "FK_archivos_clinicos_historial_clinico_id_historial",
                        column: x => x.id_historial,
                        principalTable: "historial_clinico",
                        principalColumn: "id_historial",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_tratamiento",
                columns: table => new
                {
                    id_historial_tratamiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_historial = table.Column<int>(type: "int", nullable: false),
                    id_tratamiento = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    precio_unitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_tratamiento", x => x.id_historial_tratamiento);
                    table.CheckConstraint("CK_historial_tratamientos_cantidad", "[cantidad] > 0");
                    table.CheckConstraint("CK_historial_tratamientos_precio", "[precio_unitario] >= 0");
                    table.ForeignKey(
                        name: "FK_historial_tratamiento_historial_clinico_id_historial",
                        column: x => x.id_historial,
                        principalTable: "historial_clinico",
                        principalColumn: "id_historial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historial_tratamiento_tratamientos_id_tratamiento",
                        column: x => x.id_tratamiento,
                        principalTable: "tratamientos",
                        principalColumn: "id_tratamiento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "odontograma_detalles",
                columns: table => new
                {
                    id_odontograma_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_odontograma = table.Column<int>(type: "int", nullable: false),
                    numero_pieza = table.Column<int>(type: "int", nullable: false),
                    cara = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_odontograma_detalles", x => x.id_odontograma_detalle);
                    table.CheckConstraint("CK_odontograma_detalles_pieza", "([numero_pieza] BETWEEN 11 AND 18) OR ([numero_pieza] BETWEEN 21 AND 28) OR ([numero_pieza] BETWEEN 31 AND 38) OR ([numero_pieza] BETWEEN 41 AND 48) OR ([numero_pieza] BETWEEN 51 AND 55) OR ([numero_pieza] BETWEEN 61 AND 65) OR ([numero_pieza] BETWEEN 71 AND 75) OR ([numero_pieza] BETWEEN 81 AND 85)");
                    table.ForeignKey(
                        name: "FK_odontograma_detalles_odontogramas_id_odontograma",
                        column: x => x.id_odontograma,
                        principalTable: "odontogramas",
                        principalColumn: "id_odontograma",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pago_aplicaciones",
                columns: table => new
                {
                    id_pago_aplicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pago = table.Column<int>(type: "int", nullable: false),
                    id_factura = table.Column<int>(type: "int", nullable: false),
                    monto_aplicado = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    fecha_aplicacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pago_aplicaciones", x => x.id_pago_aplicacion);
                    table.CheckConstraint("CK_pagoaplicaciones_monto", "[monto_aplicado] > 0");
                    table.ForeignKey(
                        name: "FK_pago_aplicaciones_facturas_id_factura",
                        column: x => x.id_factura,
                        principalTable: "facturas",
                        principalColumn: "id_factura",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pago_aplicaciones_pagos_id_pago",
                        column: x => x.id_pago,
                        principalTable: "pagos",
                        principalColumn: "id_pago",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recordatorios_turno",
                columns: table => new
                {
                    id_recordatorio = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTurno = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    destinatario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    fecha_programada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pendiente"),
                    intentos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ultimo_error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recordatorios_turno", x => x.id_recordatorio);
                    table.CheckConstraint("CK_recordatorios_estado", "[estado] IN ('pendiente', 'procesando', 'enviado', 'fallido', 'cancelado')");
                    table.CheckConstraint("CK_recordatorios_intentos", "[intentos] >= 0");
                    table.CheckConstraint("CK_recordatorios_tipo", "[tipo] IN ('email', 'whatsapp', 'sms')");
                    table.ForeignKey(
                        name: "FK_Recordatorios_turno_turnos_IdTurno",
                        column: x => x.IdTurno,
                        principalTable: "turnos",
                        principalColumn: "id_turno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_archivos_clinicos_id_historial",
                table: "archivos_clinicos",
                column: "id_historial");

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_fecha",
                table: "bitacora",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_id_usuario_fecha",
                table: "bitacora",
                columns: new[] { "id_usuario", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_factura_detalles_id_factura",
                table: "factura_detalles",
                column: "id_factura");

            migrationBuilder.CreateIndex(
                name: "IX_factura_detalles_id_tratamiento",
                table: "factura_detalles",
                column: "id_tratamiento");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_id_paciente_fecha_emision",
                table: "facturas",
                columns: new[] { "id_paciente", "fecha_emision" });

            migrationBuilder.CreateIndex(
                name: "IX_facturas_numero",
                table: "facturas",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_historial_clinico_id_odontologo",
                table: "historial_clinico",
                column: "id_odontologo");

            migrationBuilder.CreateIndex(
                name: "IX_historial_clinico_id_paciente_fecha",
                table: "historial_clinico",
                columns: new[] { "id_paciente", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_historial_tratamiento_id_historial",
                table: "historial_tratamiento",
                column: "id_historial");

            migrationBuilder.CreateIndex(
                name: "IX_historial_tratamiento_id_tratamiento",
                table: "historial_tratamiento",
                column: "id_tratamiento");

            migrationBuilder.CreateIndex(
                name: "IX_odontograma_detalles_id_odontograma_numero_pieza",
                table: "odontograma_detalles",
                columns: new[] { "id_odontograma", "numero_pieza" });

            migrationBuilder.CreateIndex(
                name: "IX_odontogramas_id_odontologo",
                table: "odontogramas",
                column: "id_odontologo");

            migrationBuilder.CreateIndex(
                name: "IX_odontogramas_id_paciente_fecha_creacion",
                table: "odontogramas",
                columns: new[] { "id_paciente", "fecha_creacion" });

            migrationBuilder.CreateIndex(
                name: "IX_odontologos_matricula",
                table: "odontologos",
                column: "matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_paciente_dni",
                table: "paciente",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pago_aplicaciones_id_factura",
                table: "pago_aplicaciones",
                column: "id_factura");

            migrationBuilder.CreateIndex(
                name: "IX_pago_aplicaciones_id_pago",
                table: "pago_aplicaciones",
                column: "id_pago");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_id_paciente_fecha_pago",
                table: "pagos",
                columns: new[] { "id_paciente", "fecha_pago" });

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorios_turno_estado_fecha_programada",
                table: "Recordatorios_turno",
                columns: new[] { "estado", "fecha_programada" });

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorios_turno_IdTurno",
                table: "Recordatorios_turno",
                column: "IdTurno");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_id_usuario",
                table: "refresh_tokens",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_id_usuario_fecha_expiracion",
                table: "refresh_tokens",
                columns: new[] { "id_usuario", "fecha_expiracion" });

            migrationBuilder.CreateIndex(
                name: "IX_tratamientos_nombre",
                table: "tratamientos",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_turnos_id_odontologo_fecha_hora_inicio_fecha_hora_fin",
                table: "turnos",
                columns: new[] { "id_odontologo", "fecha_hora_inicio", "fecha_hora_fin" });

            migrationBuilder.CreateIndex(
                name: "IX_turnos_id_paciente_fecha_hora_inicio",
                table: "turnos",
                columns: new[] { "id_paciente", "fecha_hora_inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_odontologo",
                table: "usuarios",
                column: "id_odontologo",
                unique: true,
                filter: "[id_odontologo] IS NOT NULL");

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
                name: "factura_detalles");

            migrationBuilder.DropTable(
                name: "historial_tratamiento");

            migrationBuilder.DropTable(
                name: "odontograma_detalles");

            migrationBuilder.DropTable(
                name: "pago_aplicaciones");

            migrationBuilder.DropTable(
                name: "Recordatorios_turno");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "historial_clinico");

            migrationBuilder.DropTable(
                name: "tratamientos");

            migrationBuilder.DropTable(
                name: "odontogramas");

            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "turnos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "paciente");

            migrationBuilder.DropTable(
                name: "odontologos");
        }
    }
}
