using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class OdontogramaDetalleConfiguration : IEntityTypeConfiguration<OdontogramaDetalle>
    {
        public void Configure(EntityTypeBuilder<OdontogramaDetalle> builder)
        {
            builder.ToTable("odontograma_detalles", table =>
            {
                table.HasCheckConstraint(
                    "CK_odontograma_detalles_pieza",
                    "([numero_pieza] BETWEEN 11 AND 18) OR " +
                    "([numero_pieza] BETWEEN 21 AND 28) OR " +
                    "([numero_pieza] BETWEEN 31 AND 38) OR " +
                    "([numero_pieza] BETWEEN 41 AND 48) OR " +
                    "([numero_pieza] BETWEEN 51 AND 55) OR " +
                    "([numero_pieza] BETWEEN 61 AND 65) OR " +
                    "([numero_pieza] BETWEEN 71 AND 75) OR " +
                    "([numero_pieza] BETWEEN 81 AND 85)");
            });

            builder.HasKey(d => d.IdOdontogramaDetalle);

            builder.Property(d => d.IdOdontogramaDetalle)
                .HasColumnName("id_odontograma_detalle")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.IdOdontograma)
                .HasColumnName("id_odontograma")
                .IsRequired();

            builder.Property(d => d.NumeroPieza)
                .HasColumnName("numero_pieza")
                .IsRequired();

            builder.Property(d => d.Cara)
                .HasColumnName("cara")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(d => d.Estado)
                .HasColumnName("estado")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(d => new
            {
                d.IdOdontograma,
                d.NumeroPieza
            });

            builder.HasOne(d => d.Odontograma)
                .WithMany(o => o.Detalles)
                .HasForeignKey(d => d.IdOdontograma)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
