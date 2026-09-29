namespace SistemaGestionOdontologica.Api.Entities
{
    public class HistorialClinico
    {
        public int IdHistorial {  get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public DateTime Fecha { get; set; }
        public string MotivoConsulta { get; set; } = string.Empty;
        public string Diagnostico {  get; set; } = string.Empty;
        public string Observaciones {  get; set; } = string.Empty;

        // RELACIONES
        public Paciente Paciente { get; set; } = null!;

        public Odontologo Odontologo { get; set; } = null!;

        public ICollection<HistorialTratamiento> HistorialTratamientos { get; set; }
            = new List<HistorialTratamiento>();

        public ICollection<ArchivoClinico> Archivos { get; set; }
            = new List<ArchivoClinico>();
    }
}
