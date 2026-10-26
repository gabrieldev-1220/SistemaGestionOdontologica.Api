using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
    {
        public void Configure(EntityTypeBuilder<Turno> builder)
        {
            builder.ToTable("turnos", table =>
            {
                table.HasCheckConstraint(
                    "CK_turnos_estado",
                    "[estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado', 'ausente')");

                table.HasCheckConstraint(
                    "CK_turnos_fechas",
                    "[fecha_hora_fin] > [fecha_hora_inicio]");
            });

            builder.HasKey(t => t.IdTurno);

            builder.Property(t => t.IdTurno)
                .HasColumnName("id_turno")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(t => t.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            builder.Property(t => t.FechaHoraInicio)
                .HasColumnName("fecha_hora_inicio")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(t => t.FechaHoraFin)
                .HasColumnName("fecha_hora_fin")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(t => t.Estado)
                .HasColumnName("estado")
                .HasMaxLength(20)
                .HasDefaultValue("pendiente")
                .IsRequired();

            builder.Property(t => t.Observaciones)
                .HasColumnName("observaciones")
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasIndex(t => new
            {
                t.IdOdontologo,
                t.FechaHoraInicio,
                t.FechaHoraFin
            });

            builder.HasIndex(t => new
            {
                t.IdPaciente,
                t.FechaHoraInicio
            });

            builder.HasOne(t => t.Paciente)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Odontologo)
                .WithMany(o => o.Turnos)
                .HasForeignKey(t => t.IdOdontologo)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
