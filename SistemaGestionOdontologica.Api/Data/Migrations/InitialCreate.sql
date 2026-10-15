IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [odontologos] (
    [id_odontologo] int NOT NULL IDENTITY,
    [nombre] nvarchar(50) NOT NULL,
    [apellido] nvarchar(50) NOT NULL,
    [matricula] nvarchar(30) NOT NULL,
    [telefono] nvarchar(20) NOT NULL,
    [email] nvarchar(100) NOT NULL,
    [especialidad] nvarchar(100) NOT NULL,
    [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_odontologos] PRIMARY KEY ([id_odontologo])
);

CREATE TABLE [paciente] (
    [id_paciente] int NOT NULL IDENTITY,
    [nombre] nvarchar(50) NOT NULL,
    [apellido] nvarchar(50) NOT NULL,
    [dni] nvarchar(15) NOT NULL,
    [fecha_nacimiento] date NOT NULL,
    [telefono] nvarchar(20) NOT NULL,
    [email] nvarchar(100) NOT NULL,
    [direccion] nvarchar(200) NULL,
    [fecha_registro] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_paciente] PRIMARY KEY ([id_paciente])
);

CREATE TABLE [tratamientos] (
    [id_tratamiento] int NOT NULL IDENTITY,
    [nombre] nvarchar(100) NOT NULL,
    [descripcion] nvarchar(max) NOT NULL,
    [precio] decimal(12,2) NOT NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [PK_tratamientos] PRIMARY KEY ([id_tratamiento]),
    CONSTRAINT [CK_tratamientos_precio] CHECK ([precio] >= 0)
);

CREATE TABLE [usuarios] (
    [id_usuario] int NOT NULL IDENTITY,
    [username] nvarchar(50) NOT NULL,
    [password_hash] nvarchar(500) NOT NULL,
    [rol] nvarchar(20) NOT NULL,
    [id_odontologo] int NULL,
    [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [ultimo_acceso] datetime2 NULL,
    CONSTRAINT [PK_usuarios] PRIMARY KEY ([id_usuario]),
    CONSTRAINT [CK_usuarios_odontologo] CHECK (([rol] = 'odontologo' AND[id_odontologo] IS NOT NULL) OR([rol] <> 'odontologo' AND[id_odontologo] IS NULL)),
    CONSTRAINT [CK_usuarios_rol] CHECK ([rol] IN ('admin', 'odontologo', 'asistente')),
    CONSTRAINT [FK_usuarios_odontologos_id_odontologo] FOREIGN KEY ([id_odontologo]) REFERENCES [odontologos] ([id_odontologo]) ON DELETE NO ACTION
);

CREATE TABLE [facturas] (
    [id_factura] int NOT NULL IDENTITY,
    [id_paciente] int NOT NULL,
    [fecha_emision] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [numero] nvarchar(50) NOT NULL,
    [estado] nvarchar(20) NOT NULL DEFAULT N'emitida',
    [total] decimal(12,2) NOT NULL,
    [observaciones] nvarchar(max) NULL,
    CONSTRAINT [PK_facturas] PRIMARY KEY ([id_factura]),
    CONSTRAINT [CK_facturas_estado] CHECK ([estado] IN ('emitida', 'pagada', 'anulada')),
    CONSTRAINT [CK_facturas_total] CHECK ([total] >= 0),
    CONSTRAINT [FK_facturas_paciente_id_paciente] FOREIGN KEY ([id_paciente]) REFERENCES [paciente] ([id_paciente]) ON DELETE NO ACTION
);

CREATE TABLE [historial_clinico] (
    [id_historial] int NOT NULL IDENTITY,
    [id_paciente] int NOT NULL,
    [id_odontologo] int NOT NULL,
    [fecha] datetime2 NOT NULL,
    [motivo_consulta] nvarchar(max) NOT NULL,
    [diagnostico] nvarchar(max) NOT NULL,
    [observaciones] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_historial_clinico] PRIMARY KEY ([id_historial]),
    CONSTRAINT [FK_historial_clinico_odontologos_id_odontologo] FOREIGN KEY ([id_odontologo]) REFERENCES [odontologos] ([id_odontologo]) ON DELETE NO ACTION,
    CONSTRAINT [FK_historial_clinico_paciente_id_paciente] FOREIGN KEY ([id_paciente]) REFERENCES [paciente] ([id_paciente]) ON DELETE NO ACTION
);

CREATE TABLE [odontogramas] (
    [id_odontograma] int NOT NULL IDENTITY,
    [id_paciente] int NOT NULL,
    [id_odontologo] int NOT NULL,
    [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [observaciones] nvarchar(max) NULL,
    CONSTRAINT [PK_odontogramas] PRIMARY KEY ([id_odontograma]),
    CONSTRAINT [FK_odontogramas_odontologos_id_odontologo] FOREIGN KEY ([id_odontologo]) REFERENCES [odontologos] ([id_odontologo]) ON DELETE NO ACTION,
    CONSTRAINT [FK_odontogramas_paciente_id_paciente] FOREIGN KEY ([id_paciente]) REFERENCES [paciente] ([id_paciente]) ON DELETE NO ACTION
);

CREATE TABLE [pagos] (
    [id_pago] int NOT NULL IDENTITY,
    [id_paciente] int NOT NULL,
    [fecha_pago] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [monto] decimal(12,2) NOT NULL,
    [metodo_pago] nvarchar(20) NOT NULL,
    [observaciones] nvarchar(max) NULL,
    CONSTRAINT [PK_pagos] PRIMARY KEY ([id_pago]),
    CONSTRAINT [CK_pagos_metodo] CHECK ([metodo_pago] IN ('efectivo', 'tarjeta', 'Transferencia')),
    CONSTRAINT [CK_pagos_monto] CHECK ([monto] > 0),
    CONSTRAINT [FK_pagos_paciente_id_paciente] FOREIGN KEY ([id_paciente]) REFERENCES [paciente] ([id_paciente]) ON DELETE NO ACTION
);

CREATE TABLE [turnos] (
    [id_turno] int NOT NULL IDENTITY,
    [id_paciente] int NOT NULL,
    [id_odontologo] int NOT NULL,
    [fecha_hora_inicio] datetime2 NOT NULL,
    [fecha_hora_fin] datetime2 NOT NULL,
    [estado] nvarchar(20) NOT NULL DEFAULT N'pendiente',
    [observaciones] nvarchar(max) NULL,
    [fecha_creacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [PK_turnos] PRIMARY KEY ([id_turno]),
    CONSTRAINT [CK_turnos_estado] CHECK ([estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado')),
    CONSTRAINT [CK_turnos_fechas] CHECK ([fecha_hora_fin] > [fecha_hora_inicio]),
    CONSTRAINT [FK_turnos_odontologos_id_odontologo] FOREIGN KEY ([id_odontologo]) REFERENCES [odontologos] ([id_odontologo]) ON DELETE NO ACTION,
    CONSTRAINT [FK_turnos_paciente_id_paciente] FOREIGN KEY ([id_paciente]) REFERENCES [paciente] ([id_paciente]) ON DELETE NO ACTION
);

CREATE TABLE [bitacora] (
    [id_bitacora] bigint NOT NULL IDENTITY,
    [id_usuario] int NOT NULL,
    [accion] nvarchar(100) NOT NULL,
    [fecha] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    [detalles] nvarchar(max) NULL,
    CONSTRAINT [PK_bitacora] PRIMARY KEY ([id_bitacora]),
    CONSTRAINT [FK_bitacora_usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuarios] ([id_usuario]) ON DELETE NO ACTION
);

CREATE TABLE [refresh_tokens] (
    [id_refresh_token] bigint NOT NULL IDENTITY,
    [id_usuario] int NOT NULL,
    [token_hash] nvarchar(128) NOT NULL,
    [fecha_creacion] datetime2 NOT NULL,
    [fecha_expiracion] datetime2 NOT NULL,
    [fecha_revocacion] datetime2 NULL,
    CONSTRAINT [PK_refresh_tokens] PRIMARY KEY ([id_refresh_token]),
    CONSTRAINT [CK_refresh_tokens_fechas] CHECK ([fecha_expiracion] > [fecha_creacion]),
    CONSTRAINT [CK_refresh_tokens_revocacion] CHECK ([fecha_revocacion] IS NULL OR [fecha_revocacion] >= [fecha_creacion]),
    CONSTRAINT [FK_refresh_tokens_usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuarios] ([id_usuario]) ON DELETE CASCADE
);

CREATE TABLE [factura_detalles] (
    [id_factura_detalle] int NOT NULL IDENTITY,
    [id_factura] int NOT NULL,
    [id_tratamiento] int NULL,
    [descripcion] nvarchar(500) NOT NULL,
    [cantidad] int NOT NULL DEFAULT 1,
    [precio_unitario] decimal(12,2) NOT NULL,
    CONSTRAINT [PK_factura_detalles] PRIMARY KEY ([id_factura_detalle]),
    CONSTRAINT [CK_factura_detalles_cantidad] CHECK ([cantidad] > 0),
    CONSTRAINT [CK_factura_detalles_precio] CHECK ([precio_unitario] >= 0),
    CONSTRAINT [FK_factura_detalles_facturas_id_factura] FOREIGN KEY ([id_factura]) REFERENCES [facturas] ([id_factura]) ON DELETE NO ACTION,
    CONSTRAINT [FK_factura_detalles_tratamientos_id_tratamiento] FOREIGN KEY ([id_tratamiento]) REFERENCES [tratamientos] ([id_tratamiento]) ON DELETE NO ACTION
);

CREATE TABLE [archivos_clinicos] (
    [id_archivo] bigint NOT NULL IDENTITY,
    [id_historial] int NOT NULL,
    [nombre_original] nvarchar(255) NOT NULL,
    [nombre_archivo] nvarchar(255) NOT NULL,
    [ruta] nvarchar(500) NOT NULL,
    [tipo_mime] nvarchar(100) NOT NULL,
    [tamano_bytes] bigint NOT NULL,
    [fecha_subida] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [PK_archivos_clinicos] PRIMARY KEY ([id_archivo]),
    CONSTRAINT [CK_archivos_tamano] CHECK ([tamano_bytes] > 0),
    CONSTRAINT [FK_archivos_clinicos_historial_clinico_id_historial] FOREIGN KEY ([id_historial]) REFERENCES [historial_clinico] ([id_historial]) ON DELETE NO ACTION
);

CREATE TABLE [historial_tratamiento] (
    [id_historial_tratamiento] int NOT NULL IDENTITY,
    [id_historial] int NOT NULL,
    [id_tratamiento] int NOT NULL,
    [cantidad] int NOT NULL DEFAULT 1,
    [precio_unitario] decimal(12,2) NOT NULL,
    CONSTRAINT [PK_historial_tratamiento] PRIMARY KEY ([id_historial_tratamiento]),
    CONSTRAINT [CK_historial_tratamientos_cantidad] CHECK ([cantidad] > 0),
    CONSTRAINT [CK_historial_tratamientos_precio] CHECK ([precio_unitario] >= 0),
    CONSTRAINT [FK_historial_tratamiento_historial_clinico_id_historial] FOREIGN KEY ([id_historial]) REFERENCES [historial_clinico] ([id_historial]) ON DELETE NO ACTION,
    CONSTRAINT [FK_historial_tratamiento_tratamientos_id_tratamiento] FOREIGN KEY ([id_tratamiento]) REFERENCES [tratamientos] ([id_tratamiento]) ON DELETE NO ACTION
);

CREATE TABLE [odontograma_detalles] (
    [id_odontograma_detalle] int NOT NULL IDENTITY,
    [id_odontograma] int NOT NULL,
    [numero_pieza] int NOT NULL,
    [cara] nvarchar(30) NOT NULL,
    [estado] nvarchar(100) NOT NULL,
    [observaciones] nvarchar(max) NULL,
    CONSTRAINT [PK_odontograma_detalles] PRIMARY KEY ([id_odontograma_detalle]),
    CONSTRAINT [CK_odontograma_detalles_pieza] CHECK (([numero_pieza] BETWEEN 11 AND 18) OR ([numero_pieza] BETWEEN 21 AND 28) OR ([numero_pieza] BETWEEN 31 AND 38) OR ([numero_pieza] BETWEEN 41 AND 48) OR ([numero_pieza] BETWEEN 51 AND 55) OR ([numero_pieza] BETWEEN 61 AND 65) OR ([numero_pieza] BETWEEN 71 AND 75) OR ([numero_pieza] BETWEEN 81 AND 85)),
    CONSTRAINT [FK_odontograma_detalles_odontogramas_id_odontograma] FOREIGN KEY ([id_odontograma]) REFERENCES [odontogramas] ([id_odontograma]) ON DELETE NO ACTION
);

CREATE TABLE [pago_aplicaciones] (
    [id_pago_aplicacion] int NOT NULL IDENTITY,
    [id_pago] int NOT NULL,
    [id_factura] int NOT NULL,
    [monto_aplicado] decimal(12,2) NOT NULL,
    [fecha_aplicacion] datetime2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [PK_pago_aplicaciones] PRIMARY KEY ([id_pago_aplicacion]),
    CONSTRAINT [CK_pagoaplicaciones_monto] CHECK ([monto_aplicado] > 0),
    CONSTRAINT [FK_pago_aplicaciones_facturas_id_factura] FOREIGN KEY ([id_factura]) REFERENCES [facturas] ([id_factura]) ON DELETE NO ACTION,
    CONSTRAINT [FK_pago_aplicaciones_pagos_id_pago] FOREIGN KEY ([id_pago]) REFERENCES [pagos] ([id_pago]) ON DELETE NO ACTION
);

CREATE TABLE [Recordatorios_turno] (
    [id_recordatorio] bigint NOT NULL IDENTITY,
    [IdTurno] int NOT NULL,
    [tipo] nvarchar(20) NOT NULL,
    [destinatario] nvarchar(150) NOT NULL,
    [fecha_programada] datetime2 NOT NULL,
    [fecha_envio] datetime2 NULL,
    [estado] nvarchar(20) NOT NULL DEFAULT N'pendiente',
    [intentos] int NOT NULL DEFAULT 0,
    [ultimo_error] nvarchar(max) NULL,
    CONSTRAINT [PK_Recordatorios_turno] PRIMARY KEY ([id_recordatorio]),
    CONSTRAINT [CK_recordatorios_estado] CHECK ([estado] IN ('pendiente', 'procesando', 'enviado', 'fallido', 'cancelado')),
    CONSTRAINT [CK_recordatorios_intentos] CHECK ([intentos] >= 0),
    CONSTRAINT [CK_recordatorios_tipo] CHECK ([tipo] IN ('email', 'whatsapp', 'sms')),
    CONSTRAINT [FK_Recordatorios_turno_turnos_IdTurno] FOREIGN KEY ([IdTurno]) REFERENCES [turnos] ([id_turno]) ON DELETE NO ACTION
);

CREATE INDEX [IX_archivos_clinicos_id_historial] ON [archivos_clinicos] ([id_historial]);

CREATE INDEX [IX_bitacora_fecha] ON [bitacora] ([fecha]);

CREATE INDEX [IX_bitacora_id_usuario_fecha] ON [bitacora] ([id_usuario], [fecha]);

CREATE INDEX [IX_factura_detalles_id_factura] ON [factura_detalles] ([id_factura]);

CREATE INDEX [IX_factura_detalles_id_tratamiento] ON [factura_detalles] ([id_tratamiento]);

CREATE INDEX [IX_facturas_id_paciente_fecha_emision] ON [facturas] ([id_paciente], [fecha_emision]);

CREATE UNIQUE INDEX [IX_facturas_numero] ON [facturas] ([numero]);

CREATE INDEX [IX_historial_clinico_id_odontologo] ON [historial_clinico] ([id_odontologo]);

CREATE INDEX [IX_historial_clinico_id_paciente_fecha] ON [historial_clinico] ([id_paciente], [fecha]);

CREATE INDEX [IX_historial_tratamiento_id_historial] ON [historial_tratamiento] ([id_historial]);

CREATE INDEX [IX_historial_tratamiento_id_tratamiento] ON [historial_tratamiento] ([id_tratamiento]);

CREATE INDEX [IX_odontograma_detalles_id_odontograma_numero_pieza] ON [odontograma_detalles] ([id_odontograma], [numero_pieza]);

CREATE INDEX [IX_odontogramas_id_odontologo] ON [odontogramas] ([id_odontologo]);

CREATE INDEX [IX_odontogramas_id_paciente_fecha_creacion] ON [odontogramas] ([id_paciente], [fecha_creacion]);

CREATE UNIQUE INDEX [IX_odontologos_matricula] ON [odontologos] ([matricula]);

CREATE UNIQUE INDEX [IX_paciente_dni] ON [paciente] ([dni]);

CREATE INDEX [IX_pago_aplicaciones_id_factura] ON [pago_aplicaciones] ([id_factura]);

CREATE INDEX [IX_pago_aplicaciones_id_pago] ON [pago_aplicaciones] ([id_pago]);

CREATE INDEX [IX_pagos_id_paciente_fecha_pago] ON [pagos] ([id_paciente], [fecha_pago]);

CREATE INDEX [IX_Recordatorios_turno_estado_fecha_programada] ON [Recordatorios_turno] ([estado], [fecha_programada]);

CREATE INDEX [IX_Recordatorios_turno_IdTurno] ON [Recordatorios_turno] ([IdTurno]);

CREATE INDEX [IX_refresh_tokens_id_usuario] ON [refresh_tokens] ([id_usuario]);

CREATE INDEX [IX_refresh_tokens_id_usuario_fecha_expiracion] ON [refresh_tokens] ([id_usuario], [fecha_expiracion]);

CREATE INDEX [IX_tratamientos_nombre] ON [tratamientos] ([nombre]);

CREATE INDEX [IX_turnos_id_odontologo_fecha_hora_inicio_fecha_hora_fin] ON [turnos] ([id_odontologo], [fecha_hora_inicio], [fecha_hora_fin]);

CREATE INDEX [IX_turnos_id_paciente_fecha_hora_inicio] ON [turnos] ([id_paciente], [fecha_hora_inicio]);

CREATE UNIQUE INDEX [IX_usuarios_id_odontologo] ON [usuarios] ([id_odontologo]) WHERE [id_odontologo] IS NOT NULL;

CREATE UNIQUE INDEX [IX_usuarios_username] ON [usuarios] ([username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261015204659_InitialCreate', N'10.0.13');

COMMIT;
GO

