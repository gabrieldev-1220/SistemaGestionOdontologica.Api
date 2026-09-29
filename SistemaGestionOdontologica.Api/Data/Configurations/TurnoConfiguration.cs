using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Data.Configurations
{
    public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
    {
        public void Configure(EntityTypeBuilder<Turno> builder)
        {
            builder.ToTable(
                "turnos",
                TableBuilder =>
                {
                    TableBuilder.HasCheckConstraint(
                        "CK_turnos_fechas",
                        "[fecha_hora_fin] > [fecha_hora_inicio]");

                    TableBuilder.HasCheckConstraint(
                        "CK_turnos_estado",
                        "[estado] IN ('pendiente', 'confirmado', 'cancelado', 'realizado')");
                });

            // CLAVE PRIMARIA
            builder.HasKey(t => t.IdTurno);

            builder.Property(t => t.IdTurno)
                .HasColumnName("id_turno")
                .ValueGeneratedOnAdd();

            // CLAVES FORÁNEAS
            builder.Property(t => t.IdPaciente)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(t => t.IdOdontologo)
                .HasColumnName("id_odontologo")
                .IsRequired();

            // FECHAS
            builder.Property(t => t.FechaHoraInicio)
                .HasColumnName("fecha_hora_inicio")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(t => t.FechaHoraFin)
                .HasColumnName("fecha_hora_fin")
                .HasColumnType("datetime2")
                .IsRequired();

            // ESTADO
            builder.Property(t => t.Estado)
                .HasColumnName("estado")
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<EstadoTurno>(v, true))
                .HasMaxLength(20)
                .HasDefaultValue(EstadoTurno.Pendiente)
                .IsRequired();

            //OBSERVACIONES
            builder.Property(t => t.Observaciones)
                .HasColumnName("observaciones");

            // FECHA DE CREACIÓN
            builder.Property(t => t.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");            

            // ÍNDICE PARA AGENDA
            builder.HasIndex(t => new
            {
                t.IdOdontologo,
                t.FechaHoraInicio
            });

            // RELACIÓN CON RECORDATORIOS
            builder.HasMany(t => t.Recordatorios)
                .WithOne(r => r.Turno)
                .HasForeignKey(r => r.IdTurno)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
