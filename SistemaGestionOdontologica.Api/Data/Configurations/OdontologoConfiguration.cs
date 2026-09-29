using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class OdontologoConfiguration : IEntityTypeConfiguration<Odontologo>
    {
        public void Configure(EntityTypeBuilder<Odontologo> builder)
        {
            builder.ToTable("odontologos");

            // CLAVE PRIMARIA
            builder.HasKey(o => o.IdOdontologo);

            builder.Property(o => o.IdOdontologo)
                .HasColumnName("id_odontologo")
                .ValueGeneratedOnAdd();

            // DATOS DEL ODONTÓLOGO
            builder.Property(o => o.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.Apellido)
                .HasColumnName("apellido")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.Matricula)
                .HasColumnName("matricula")
                .HasMaxLength(30)
                .IsRequired();

            // MATRÍCULA ÚNICA
            builder.HasIndex(o => o.Matricula)
                .IsUnique();

            builder.Property(o => o.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Especialidad)
                .HasColumnName("especialidad")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            // RELACIÓN CON TURNOS
            builder.HasMany(o => o.Turnos)
                .WithOne(t => t.Odontologo)
                .HasForeignKey(t => t.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON HISTORIAL CLÍNICO
            builder.HasMany(o => o.HistorialesClinicos)
                .WithOne(h => h.Odontologo)
                .HasForeignKey(h => h.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON USUARIOS
            builder.HasMany(o => o.Usuarios)
                .WithOne(u => u.Odontologo)
                .HasForeignKey(u => u.IdOdontologo)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
