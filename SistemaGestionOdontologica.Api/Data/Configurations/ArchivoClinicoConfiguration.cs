using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class ArchivoClinicoConfiguration : IEntityTypeConfiguration<ArchivoClinico>
    {
        public void Configure(EntityTypeBuilder<ArchivoClinico> builder)
        {
            builder.ToTable("archivos_clinicos");

            builder.HasKey(a => a.IdArchivo);

            builder.Property(a => a.IdArchivo)
                .HasColumnName("id_archivo")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.IdHistorial)
                .HasColumnName("id_historial")
                .IsRequired();

            builder.Property(a => a.NombreOriginal)
                .HasColumnName("nombre_original")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.NombreArchivo)
                .HasColumnName("nombre_archivo")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.Ruta)
                .HasColumnName("ruta")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.TipoMime)
                .HasColumnName("tipo_mime")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.TamanoBytes)
                .HasColumnName("tamano_bytes")
                .IsRequired();

            builder.Property(a => a.FechaSubida)
                .HasColumnName("fecha_subida")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasIndex(a => a.IdHistorial);
        }
    }
}
