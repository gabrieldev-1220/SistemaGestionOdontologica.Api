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

            // CLAVE PRIMARIA
            builder.HasKey(b => b.IdBitacora);

            builder.Property(b => b.IdBitacora)
                .HasColumnName("id_bitacora")
                .ValueGeneratedOnAdd();

            // USUARIO
            builder.Property(b => b.IdUsuario)
                .HasColumnName("id_usuario")
                .IsRequired();

            // ACCIÓN
            builder.Property(b => b.Accion)
                .HasColumnName("accion")
                .HasMaxLength(100)
                .IsRequired();

            // FECHA
            builder.Property(b => b.Fecha)
                .HasColumnName("fecha")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            // DETALLES
            builder.Property(b => b.Detalles)
                .HasColumnName("detalles");

            // RELACIÓN CON USUARIO
            builder.HasOne(b => b.Usuario)
                .WithMany(u => u.Bitacoras)
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // ÍNDICE PARA CONSULTAS DE AUDITORÍA
            builder.HasIndex(b => new
            {
                b.IdUsuario,
                b.Fecha
            });
        }
    }
}
