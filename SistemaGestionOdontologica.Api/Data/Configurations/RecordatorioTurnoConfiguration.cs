using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class RecordatorioTurnoConfiguration : IEntityTypeConfiguration<RecordatorioTurno>
    {
        public void Configure(EntityTypeBuilder<RecordatorioTurno> builder)
        {
            builder.ToTable(
                "recordatorios_turno",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_recordatorios_turno",
                        "[tipo] IN ('email','whatsapp', 'sms')");
                });

            // CLAVE PRIMARIA
            builder.HasKey(r => r.IdRecordatorio);

            builder.Property(r => r.IdRecordatorio)
                .HasColumnName("id_recordatorio")
                .ValueGeneratedOnAdd();

            // TURNO
            builder.Property(r => r.IdTurno)
                .HasColumnName("id_turno")
                .IsRequired();

            // FECHA PROGRAMADA
            builder.Property(r => r.FechaProgramada)
                .HasColumnName("fecha_programada")
                .HasColumnType("datetime2")
                .IsRequired();

            // TIPO
            builder.Property(r => r.Tipo)
                .HasColumnName("tipo")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<TipoRecordatorio>(v, true))
                .HasMaxLength(20)
                .IsRequired();

            // ESTADO DE ENVÍO
            builder.Property(r => r.Enviado)
                .HasColumnName("enviado")
                .HasDefaultValue(false);

            // FECHA DE ENVÍO
            builder.Property(r => r.FechaEnvio)
                .HasColumnName("fecha_envio")
                .HasColumnType("datetime2");

            // ERROR
            builder.Property(r => r.Error)
                .HasColumnName("error");

            // RELACIÓN CON TURNO
            builder.HasOne(r => r.Turno)
                .WithMany(t => t.Recordatorios)
                .HasForeignKey(r => r.IdTurno)
                .OnDelete(DeleteBehavior.Cascade);

            // ÍNDICE PARA EL PROCESAMIENTO DE RECORDATORIOS
            builder.HasIndex(r => new
            {
                r.Enviado,
                r.FechaProgramada
            });
        }
    }
}
