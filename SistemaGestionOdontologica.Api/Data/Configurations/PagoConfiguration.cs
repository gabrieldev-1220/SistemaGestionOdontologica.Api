using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("pagos", table =>
            {
                table.HasCheckConstraint(
                    "CK_pagos_monto",
                    "[monto] > 0");

                table.HasCheckConstraint(
                    "CK_pagos_metodo",
                    "[metodo_pago] IN ('efectivo', 'tarjeta', 'Transferencia')");
            });

            builder.HasKey(p => p.IdPago);

            builder.Property(p => p.IdPago)
                .HasColumnName("id_pago")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(p => p.FechaPago)
                .HasColumnName("fecha_pago")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(p => p.Monto)
                .HasColumnName("monto")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(p => p.MetodoPago)
                .HasColumnName("metodo_pago")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(p => new
            {
                p.IdPaciente,
                p.FechaPago
            });

            builder.HasOne(p => p.Paciente)
                .WithMany(pa => pa.Pagos)
                .HasForeignKey(p => p.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Aplicaciones)
                .WithOne(a => a.Pago)
                .HasForeignKey(a => a.IdPago)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
