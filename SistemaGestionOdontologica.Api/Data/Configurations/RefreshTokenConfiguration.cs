using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens", table =>
            {
                table.HasCheckConstraint(
                    "CK_refresh_tokens_fechas",
                    "[fecha_expiracion] > [fecha_creacion]");

                table.HasCheckConstraint(
                    "CK_refresh_tokens_revocacion",
                    "[fecha_revocacion] IS NULL OR " + "[fecha_revocacion] >= [fecha_creacion]");
            });

            builder.HasKey(r => r.IdRefreshToken);

            builder.Property(r => r.IdRefreshToken)
                .HasColumnName("id_refresh_token")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.IdUsuario)
                .HasColumnName("id_usuario")
                .IsRequired();

            builder.Property(r => r.TokenHash)
                .HasColumnName("token_hash")
                .HasMaxLength(128)
                .IsRequired();

            builder.HasIndex(r => r.IdUsuario);

            builder.Property(r => r.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(r => r.FechaExpiracion)
                .HasColumnName("fecha_expiracion")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(r => r.FechaRevocacion)
                .HasColumnName("fecha_revocacion")
                .HasColumnType("datetime2");

            builder.HasIndex(r => new
            {
                r.IdUsuario,
                r.FechaExpiracion
            });

            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
