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
            builder.ToTable("recordatorios_turno");

            builder.HasKey(r => r.IdRecordatorio);

            builder.Property(r => r.IdRecordatorio)
                .HasColumnName("id_recordatorio")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.IdTurno)
                .HasColumnName("id_turno")
                .IsRequired();

            builder.Property(r => r.FechaProgramada)
                .HasColumnName("fecha_programada")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(r => r.Tipo)
                .HasColumnName("tipo")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<TipoRecordatorio>(v, true))
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(r => r.Enviado)
                .HasColumnName("enviado")
                .HasDefaultValue(false);

            builder.Property(r => r.FechaEnvio)
                .HasColumnName("fecha_envio")
                .HasColumnType("datetime2");

            builder.Property(r => r.Error)
                .HasColumnName("error");

            builder.ToTable(
                "recordatorios_turno",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_recordatorios_turno",
                        "[tipo] IN ('email','Whatsapp', 'sma')");
                });

            builder.HasIndex(r => new
            {
                r.Enviado,
                r.FechaProgramada
            });
        }
    }
}
