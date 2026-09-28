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
            builder.ToTable("usuarios");

            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.IdUsuario)
                .HasColumnName("id_usuario")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Username)
                .HasColumnName("username")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(u => u.Rol)
                .HasColumnName("rol")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<RolUsuario>(v, true));

            builder.Property(u => u.IdOdontologo)
                .HasColumnName("id_odontologo");

            builder.Property(u => u.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true);

            builder.Property(u => u.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(u => u.UltimoAcceso)
                .HasColumnName("ultimo_acceso")
                .HasColumnType("datetiem2");

            builder.ToTable(
                "usuarios",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_usuarios_rol",
                        "[rol] IN ('admin', 'odontologo', 'recepcionista')");
                });

            builder.HasMany(u => u.Bitacoras)
                .WithOne(b => b.Usuario)
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
