using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class HistorialClinicoConfiguration : IEntityTypeConfiguration<HistorialClinico>
    {
        public void Configure(EntityTypeBuilder<HistorialClinico> builder)
        {
            builder.ToTable("historial_clinico");

            builder.HasKey(h => h.IdHistorial);

            // PROPIEDADES.
            builder.Property(h => h.IdHistorial)
                .HasColumnName("id_historial")
                .ValueGeneratedOnAdd();

            builder.Property(h => h.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(h => h.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            builder.Property(h => h.IdTurno)
                .HasColumnName("id_turno")
                .IsRequired(false);

            builder.Property(h => h.Fecha)
                .HasColumnName("fecha")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(h => h.FechaRegistroUtc)
                .HasColumnName("fecha_registro_utc")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(h => h.MotivoConsulta)
                .HasColumnName("motivo_consulta")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(h => h.Diagnostico)
                .HasColumnName("diagnostico")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(h => h.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            // INDICES
            builder.HasIndex(h => new
            {
                h.IdPaciente,
                h.Fecha
            });

            builder.HasIndex(h => h.IdTurno);

            // RELACIONES.
            builder.HasOne(h => h.Paciente)
                .WithMany(p => p.HistorialesClinicos)
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Odontologo)
                .WithMany(o => o.HistorialesClinicos)
                .HasForeignKey(h => h.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Turno)
                .WithMany(t => t.HistorialesClinicos)
                .HasForeignKey(h => h.IdTurno)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.TratamientosRealizados)
                .WithOne(ht => ht.HistorialClinico)
                .HasForeignKey(ht => ht.IdHistorial)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.ArchivosClinicos)
                .WithOne(a => a.HistorialClinico)
                .HasForeignKey(a => a.IdHistorial)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
