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

            builder.Property(h => h.IdHistorial)
                .HasColumnName("id_historial")
                .ValueGeneratedOnAdd();

            builder.Property(h => h.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(h => h.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            builder.Property(h => h.Fecha)
                .HasColumnName("fecha")
                .HasColumnType("datetiem2")
                .IsRequired();

            builder.Property(h => h.MotivoConsulta)
                .HasColumnName("motivo_consulta")
                .IsRequired();

            builder.Property(h => h.Diagnostico)
                .HasColumnName("diagnostico")
                .IsRequired();

            builder.Property(h => h.Observaciones)
                .HasColumnName("observaciones")
                .IsRequired();

            builder.HasMany(h => h.HistorialTratamientos)
                .WithOne(ht => ht.Historial)
                .HasForeignKey(ht => ht.IdHistorial)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.Archivos)
                .WithOne(a => a.Historial)
                .HasForeignKey(a => a.IdHistorial)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
