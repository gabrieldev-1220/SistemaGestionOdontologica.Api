using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("pagos");

            builder.HasKey(p => p.IdPago);

            builder.Property(p => p.IdPago)
                .HasColumnName("id_pago")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(p => p.FechaPago)
                .HasColumnName("fecha_pago")
                .HasColumnType("datetiem2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(p => p.Monto)
                .HasColumnName("monto")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(p => p.MetodoPago)
                .HasColumnName("metodo_pago")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<MetodoPago>(v, true))
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.Observaciones)
                .HasColumnName("observaciones");

            builder.ToTable(
                "pagos",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_pagos_monto",
                        "[monto] > 0");
                });

            builder.HasIndex(p => new
            {
                p.IdPaciente,
                p.FechaPago
            });
        }
    }
}
