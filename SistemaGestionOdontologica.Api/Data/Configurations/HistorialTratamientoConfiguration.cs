using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class HistorialTratamientoConfiguration : IEntityTypeConfiguration<HistorialTratamiento>
    {
        public void Configure(EntityTypeBuilder<HistorialTratamiento> builder)
        {
            builder.ToTable("historial_tratamiento", table =>
            {
                table.HasCheckConstraint(
                    "CK_historial_tratamientos_cantidad",
                    "[cantidad] > 0");

                table.HasCheckConstraint(
                    "CK_historial_tratamientos_precio",
                    "[precio_unitario] >= 0");
            });

            builder.HasKey(ht => ht.IdHistorialTratamiento);

            builder.Property(ht => ht.IdHistorialTratamiento)
                .HasColumnName("id_historial_tratamiento")
                .ValueGeneratedOnAdd();

            builder.Property(ht => ht.IdHistorial)
                .HasColumnName("id_historial")
                .IsRequired();

            builder.Property(ht => ht.IdTratamiento)
                .HasColumnName("id_tratamiento")
                .IsRequired();

            builder.Property(ht => ht.Cantidad)
                .HasColumnName("cantidad")
                .HasDefaultValue(1);

            builder.Property(ht => ht.PrecioUnitario)
                .HasColumnName("precio_unitario")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.HasOne(ht => ht.HistorialClinico)
                .WithMany(h => h.TratamientosRealizados)
                .HasForeignKey(ht => ht.IdHistorial)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ht => ht.Tratamiento)
                .WithMany(t => t.HistorialTratamientos)
                .HasForeignKey(ht => ht.IdTratamiento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
