namespace SistemaGestionOdontologica.Api.DTOs.HistorialesClinicos
{
    public sealed class HistorialClinicoRespuestaDto
    {
        public int IdHistorial { get; set; }

        public int IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = string.Empty;

        public int IdOdontologo { get; set; }

        public string NombreOdontologo { get; set; } = string.Empty;

        public int? IdTurno { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaRegistroUtc { get; set; }

        public string MotivoConsulta { get; set; } = string.Empty;

        public string Diagnostico { get; set; } = string.Empty;

        public string Observaciones { get; set; } = string.Empty;
    }
}