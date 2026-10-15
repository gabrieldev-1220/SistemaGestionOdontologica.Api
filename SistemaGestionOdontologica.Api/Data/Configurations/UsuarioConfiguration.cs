using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios", table =>
            {
                table.HasCheckConstraint(
                    "CK_usuarios_rol",
                    "[rol] IN ('admin', 'odontologo', 'asistente')");

                table.HasCheckConstraint(
                    "CK_usuarios_odontologo",
                    "([rol] = 'odontologo' AND" +
                    "[id_odontologo] IS NOT NULL) OR" +
                    "([rol] <> 'odontologo' AND" +
                    "[id_odontologo] IS NULL)");
            });

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
                .HasMaxLength(20)
                .IsRequired();

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
                .HasColumnType("datetime2");

            builder.HasIndex(u => u.IdOdontologo)
                .IsUnique()
                .HasFilter("[id_odontologo] IS NOT NULL");

            builder.HasOne(u => u.Odontologo)
                .WithMany(o => o.Usuarios)
                .HasForeignKey(u => u.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.RegistrosBitacora)
                .WithOne(b => b.Usuario)
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.Usuario)
                .HasForeignKey(rt => rt.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
