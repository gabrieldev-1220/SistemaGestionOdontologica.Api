using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            builder.ToTable("facturas", table =>
            {
                table.HasCheckConstraint(
                    "CK_facturas_total", "[total] >= 0");

                table.HasCheckConstraint(
                    "CK_facturas_estado", "[estado] IN ('emitida', 'pagada', 'anulada')");
            });

            builder.HasKey(f => f.IdFactura);

            builder.Property(f => f.IdFactura)
                .HasColumnName("id_factura")
                .ValueGeneratedOnAdd();

            builder.Property(f => f.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(f => f.FechaEmision)
                .HasColumnName("fecha_emision")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(f => f.Numero)
                .HasColumnName("numero")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(f => f.Numero)
                .IsUnique();

            builder.Property(f => f.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("emitida")
                .IsRequired();

            builder.Property(f => f.Total)
                .HasColumnName("total")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(f => f.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(f => new
            {
                f.IdPaciente,
                f.FechaEmision
            });

            builder.HasOne(f => f.Paciente)
                .WithMany(p => p.Facturas)
                .HasForeignKey(f => f.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Detalles)
                .WithOne(d => d.Factura)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Aplicaciones)
                .WithOne(a => a.Factura)
                .HasForeignKey(a => a.IdFactura)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
