namespace SistemaGestionOdontologica.Api.DTOs.Pacientes
{
    public class PacienteRespuestaDto
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string? Telefono { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
    }
}
