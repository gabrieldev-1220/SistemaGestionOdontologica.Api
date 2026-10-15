using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class PagoAplicacionConfiguration : IEntityTypeConfiguration<PagoAplicacion>
    {
        public void Configure(EntityTypeBuilder<PagoAplicacion> builder)
        {
            builder.ToTable("pago_aplicaciones", table =>
            {
                table.HasCheckConstraint(
                    "CK_pagoaplicaciones_monto", "[monto_aplicado] > 0");
            });

            builder.HasKey(a => a.IdPagoAplicacion);

            builder.Property(a => a.IdPagoAplicacion)
                .HasColumnName("id_pago_aplicacion")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.IdPago)
                .HasColumnName("id_pago")
                .IsRequired();

            builder.Property(a => a.IdFactura)
                .HasColumnName("id_factura")
                .IsRequired();

            builder.Property(a => a.MontoAplicado)
                .HasColumnName("monto_aplicado")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(a => a.FechaAplicacion)
                .HasColumnName("fecha_aplicacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasIndex(a => a.IdPago);

            builder.HasIndex(a => a.IdFactura);

            builder.HasOne(a => a.Pago)
                .WithMany(p => p.Aplicaciones)
                .HasForeignKey(a => a.IdPago)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Factura)
                .WithMany(f => f.Aplicaciones)
                .HasForeignKey(a => a.IdFactura)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
