namespace SistemaGestionOdontologica.Api.Entities
{
    public class HistorialClinico
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public int? IdTurno { get; set; }

        // Fecha en que ocurrió la atención clínica.
        public DateTime Fecha { get; set; }

        // Momento en que se registró la evolución en el sistema (UTC).
        public DateTime FechaRegistroUtc { get; set; }

        public string MotivoConsulta { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;

        // RELACIONES.
        public Paciente Paciente { get; set; } = null!;
        public Odontologo Odontologo { get; set; } = null!;
        public Turno? Turno { get; set; }

        public ICollection<HistorialTratamiento> TratamientosRealizados { get; set; }
            = new List<HistorialTratamiento>();

        public ICollection<ArchivoClinico> ArchivosClinicos { get; set; }
            = new List<ArchivoClinico>();
    }
}