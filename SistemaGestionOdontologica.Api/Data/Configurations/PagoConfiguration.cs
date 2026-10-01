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
            builder.ToTable(
                "pagos",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_pagos_monto",
                        "[monto] > 0");

                    TableBuilder.HasCheckConstraint(
                        "CK_pagos_metodo",
                        "[metodo_pago] IN ('efectivo', 'tarjeta', 'transferencia')");
                });

            // CLAVE PRIMARIA
            builder.HasKey(p => p.IdPago);

            builder.Property(p => p.IdPago)
                .HasColumnName("id_pago")
                .ValueGeneratedOnAdd();

            // PACIENTE
            builder.Property(p => p.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            // FECHA
            builder.Property(p => p.FechaPago)
                .HasColumnName("fecha_pago")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            // MONTO
            builder.Property(p => p.Monto)
                .HasColumnName("monto")
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            // MÉTODO DE PAGO
            builder.Property(p => p.MetodoPago)
                .HasColumnName("metodo_pago")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<MetodoPago>(v, true))
                .HasMaxLength(20)
                .IsRequired();

            // OBSERVACIONES
            builder.Property(p => p.Observaciones)
                .HasColumnName("observaciones");

            // RELACIÓN CON PACIENTE
            builder.HasOne(p => p.Paciente)
                .WithMany(p => p.Pagos)
                .HasForeignKey(p => p.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
