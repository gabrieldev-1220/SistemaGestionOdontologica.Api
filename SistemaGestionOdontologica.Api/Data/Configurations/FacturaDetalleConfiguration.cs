using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class FacturaDetalleConfiguration : IEntityTypeConfiguration<FacturaDetalle>
    {
        public void Configure(EntityTypeBuilder<FacturaDetalle> builder)
        {
            builder.ToTable("factura_detalles", table =>
            {
                table.HasCheckConstraint(
                    "CK_factura_detalles_cantidad", "[cantidad] > 0");

                table.HasCheckConstraint(
                    "CK_factura_detalles_precio", "[precio_unitario] >= 0");
            });

            builder.HasKey(d => d.IdFacturaDetalle);

            builder.Property(d => d.IdFacturaDetalle)
                .HasColumnName("id_factura_detalle")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.IdFactura)
                .HasColumnName("id_factura")
                .IsRequired();

            builder.Property(d => d.IdTratamiento)
                .HasColumnName("id_tratamiento");

            builder.Property(d => d.Descripcion)
                .HasColumnName("descripcion")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(d => d.Cantidad)
                .HasColumnName("cantidad")
                .HasDefaultValue(1);

            builder.Property(d => d.PrecioUnitario)
                .HasColumnName("precio_unitario")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.HasIndex(d => d.IdFactura);

            builder.HasOne(d => d.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Tratamiento)
                .WithMany(t => t.FacturaDetalles)
                .HasForeignKey(d => d.IdTratamiento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
