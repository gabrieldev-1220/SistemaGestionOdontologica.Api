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


        }
    }
}
