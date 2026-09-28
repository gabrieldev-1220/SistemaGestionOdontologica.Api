using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class HistorialTratamientoConfiguration : IEntityTypeConfiguration<HistorialTratamiento>
    {
        public void Configure(EntityTypeBuilder<HistorialTratamiento> builder)
        {
            builder.ToTable("historial_tratamientos");

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
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(ht => ht.PrecioUnitario)
                .HasColumnName("precio_unitario")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.ToTable(
                "historial_tratamientos",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_historial_tratamientos_cantidad",
                        "[cantidad] >= 0");

                    TableBuilder.HasCheckConstraint(
                        "CK_historial_tratamientos_precio",
                        "[precio_unitario] >= 0");
                });

            builder.HasOne(ht => ht.Historial)
                .WithMany(h => h.HistorialTratamientos)
                .HasForeignKey(ht => ht.IdHistorial)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ht => ht.Tratamiento)
                .WithMany(t => t.HistorialTratamientos)
                .HasForeignKey(ht => ht.IdTratamiento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
