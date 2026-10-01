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

            // CLAVE PRIMARIA
            builder.HasKey(h => h.IdHistorial);

            builder.Property(h => h.IdHistorial)
                .HasColumnName("id_historial")
                .ValueGeneratedOnAdd();

            // CLAVES FORÁNEAS
            builder.Property(h => h.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(h => h.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            // FECHA
            builder.Property(h => h.Fecha)
                .HasColumnName("fecha")
                .HasColumnType("datetime2")
                .IsRequired();

            // INFORMACIÓN CLÍNICA
            builder.Property(h => h.MotivoConsulta)
                .HasColumnName("motivo_consulta")
                .IsRequired();

            builder.Property(h => h.Diagnostico)
                .HasColumnName("diagnostico")
                .IsRequired();

            builder.Property(h => h.Observaciones)
                .HasColumnName("observaciones")
                .IsRequired();

            // RELACIÓN CON PACIENTE
            builder.HasOne(h => h.Paciente)
                .WithMany(p => p.HistorialesClinicos)
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON ODONTÓLOGO
            builder.HasOne(h => h.Odontologo)
                .WithMany(o => o.HistorialesClinicos)
                .HasForeignKey(h => h.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON TRATAMIENTOS
            builder.HasMany(h => h.HistorialTratamientos)
                .WithOne(ht => ht.Historial)
                .HasForeignKey(ht => ht.IdHistorial)
                .OnDelete(DeleteBehavior.Cascade);

            // RELACIÓN CON ARCHIVOS
            builder.HasMany(h => h.Archivos)
                .WithOne(a => a.Historial)
                .HasForeignKey(a => a.IdHistorial)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
