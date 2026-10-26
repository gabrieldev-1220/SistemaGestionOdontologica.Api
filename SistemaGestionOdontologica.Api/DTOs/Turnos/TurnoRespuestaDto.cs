namespace SistemaGestionOdontologica.Api.DTOs.Turnos
{
    public class TurnoRespuestaDto
    {
        public int IdTurno {  get; set; }
        public int IdPaciente { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public int IdOdontologo { get; set; }
        public string NombreOdontologo { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin {  get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion {  get; set; }
    }
}
