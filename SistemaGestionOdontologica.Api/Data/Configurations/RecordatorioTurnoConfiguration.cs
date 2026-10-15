using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class RecordatorioTurnoConfiguration : IEntityTypeConfiguration<RecordatorioTurno>
    {
        public void Configure(EntityTypeBuilder<RecordatorioTurno> builder)
        {
            builder.ToTable("Recordatorios_turno", table =>
            {
                table.HasCheckConstraint(
                    "CK_recordatorios_tipo",
                    "[tipo] IN ('email', 'whatsapp', 'sms')");

                table.HasCheckConstraint(
                    "CK_recordatorios_estado",
                    "[estado] IN ('pendiente', 'procesando', 'enviado', 'fallido', 'cancelado')");

                table.HasCheckConstraint(
                    "CK_recordatorios_intentos",
                    "[intentos] >= 0");
            });

            builder.HasKey(r => r.IdRecordatorio);

            builder.Property(r => r.IdRecordatorio)
                .HasColumnName("id_recordatorio")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Tipo)
                .HasColumnName("tipo")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(r => r.Destinatario)
                .HasColumnName("destinatario")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(r => r.FechaProgramada)
                .HasColumnName("fecha_programada")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(r => r.FechaEnvio)
                .HasColumnName("fecha_envio")
                .HasColumnType("datetime2");

            builder.Property(r => r.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .IsRequired();

            builder.Property(r => r.Intentos)
                .HasColumnName("intentos")
                .HasDefaultValue(0);

            builder.Property(r => r.UltimoError)
                .HasColumnName("ultimo_error")
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(r => new
            {
                r.Estado,
                r.FechaProgramada
            });

            builder.HasOne(r => r.Turno)
                .WithMany(t => t.Recordatorios)
                .HasForeignKey(r => r.IdTurno)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
