using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class OdontogramaConfiguration : IEntityTypeConfiguration<Odontograma>
    {
        public void Configure(EntityTypeBuilder<Odontograma> builder)
        {
            builder.ToTable("odontogramas");

            builder.HasKey(o => o.IdOdontograma);

            builder.Property(o => o.IdOdontograma)
                .HasColumnName("id_odontograma")
                .ValueGeneratedOnAdd();

            builder.Property(o => o.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(o => o.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            builder.Property(o => o.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(o => o.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(o => new
            {
                o.IdPaciente,
                o.FechaCreacion
            });

            builder.HasOne(o => o.Paciente)
                .WithMany(p => p.Odontogramas)
                .HasForeignKey(o => o.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Odontologo)
                .WithMany(od => od.Odontogramas)
                .HasForeignKey(o => o.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Detalles)
                .WithOne(d => d.Odontograma)
                .HasForeignKey(d => d.IdOdontograma)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
