using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.ToTable("pacientes");

            // DATOS DEL PACIENTE
            builder.HasKey(p => p.IdPaciente);

            builder.Property(p => p.IdPaciente)
                .HasColumnName("id_paciente")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Apellido)
                .HasColumnName("apellido")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Dni)
                .HasColumnName("dni")
                .HasMaxLength(15)
                .IsRequired();

            // DNI ÚNICO
            builder.HasIndex(p => p.Dni)
                .IsUnique();

            builder.Property(p => p.FechaNacimiento)
                .HasColumnName("fecha_nacimiento")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(p => p.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Direccion)
                .HasColumnName("direccion")
                .HasMaxLength(150);

            builder.Property(p => p.FechaRegistro)
                .HasColumnName("fecha_registro")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(p => p.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            // RELACIÓN CON TURNOS.
            builder.HasMany(p => p.Turnos)
                .WithOne(t => t.Paciente)
                .HasForeignKey(t => t.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON HISTORIAL CLÍNICO
            builder.HasMany(p => p.HistorialesClinicos)
                .WithOne(h => h.Paciente)
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON PAGOS
            builder.HasMany(p => p.Pagos)
                .WithOne(p => p.Paciente)
                .HasForeignKey(p => p.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
