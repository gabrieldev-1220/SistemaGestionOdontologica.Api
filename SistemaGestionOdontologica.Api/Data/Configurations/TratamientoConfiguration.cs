using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class TratamientoConfiguration : IEntityTypeConfiguration<Tratamiento>
    {
        public void Configure(EntityTypeBuilder<Tratamiento> builder)
        {
            builder.ToTable("tratamientos");

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
                .IsRequired();

            builder.Property(t => t.Precio)
                .HasColumnName("precio")
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(t => t.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.ToTable(
                "tratamientos",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_tratamientos_precio",
                        "[precio] >= 0");
                });
        }
    }
}
