using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class BitacoraConfiguration : IEntityTypeConfiguration<Bitacora>
    {
        public void Configure(EntityTypeBuilder<Bitacora> builder)
        {
            builder.ToTable("bitacora");

            builder.HasKey(b => b.IdBitacora);

            builder.Property(b => b.IdBitacora)
                .HasColumnName("id_bitacora")
                .ValueGeneratedOnAdd();

            builder.Property(b => b.IdUsuario)
                .HasColumnName("id_usuario")
                .IsRequired();

            builder.Property(b => b.Accion)
                .HasColumnName("accion")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.Fecha)
                .HasColumnName("fecha")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(b => b.Detalles)
                .HasColumnName("detalles");

            builder.HasIndex(b => new
            {
                b.IdUsuario,
                b.Fecha
            });
        }
    }
}
