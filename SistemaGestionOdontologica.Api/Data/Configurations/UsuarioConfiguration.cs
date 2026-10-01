using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable(
                "usuarios",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_usuarios_rol",
                        "[rol] IN ('admin', 'odontologo', 'recepcionista')");
                });

            // CLAVE PRIMARIA
            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.IdUsuario)
                .HasColumnName("id_usuario")
                .ValueGeneratedOnAdd();

            // USUARIO
            builder.Property(u => u.Username)
                .HasColumnName("username")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.Username)
                .IsUnique();

            // CONTRASEÑA
            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired();

            // ROL
            builder.Property(u => u.Rol)
                .HasColumnName("rol")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<RolUsuario>(v, true))
                .HasMaxLength(20)
                .IsRequired();

            // ODONTÓLOGO OPCIONAL
            builder.Property(u => u.IdOdontologo)
                .HasColumnName("id_odontologo");

            // ESTADO
            builder.Property(u => u.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            // FECHA DE CREACIÓN
            builder.Property(u => u.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            // ÚLTIMO ACCESO
            builder.Property(u => u.UltimoAcceso)
                .HasColumnName("ultimo_acceso")
                .HasColumnType("datetime2");

            // RELACIÓN CON ODONTÓLOGO
            builder.HasOne(u => u.Odontologo)
                .WithMany(o => o.Usuarios)
                .HasForeignKey(u => u.IdOdontologo)
                .OnDelete(DeleteBehavior.SetNull);

            // RELACIÓN CON BITÁCORA
            builder.HasMany(u => u.Bitacoras)
                .WithOne(b => b.Usuario)
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
