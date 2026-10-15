using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class TratamientoConfiguration : IEntityTypeConfiguration<Tratamiento>
    {
        public void Configure(EntityTypeBuilder<Tratamiento> builder)
        {
            builder.ToTable("tratamientos", table =>
            {
                table.HasCheckConstraint(
                    "CK_tratamientos_precio",
                    "[precio] >= 0");
            });

            builder.HasKey(t => t.IdTratamiento);

            builder.Property(t => t.IdTratamiento)
                .HasColumnName("id_tratamiento")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Descripcion)
                .HasColumnName("descripcion")
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.Precio)
                .HasColumnName("precio")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(t => t.Activo)
                .HasColumnName("Activo")
                .HasDefaultValue(true);

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasIndex(t => t.Nombre);

            builder.HasMany(t => t.HistorialTratamientos)
                .WithOne(ht => ht.Tratamiento)
                .HasForeignKey(ht => ht.IdTratamiento)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.FacturaDetalles)
                .WithOne(fd => fd.Tratamiento)
                .HasForeignKey(fd => fd.IdTratamiento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
