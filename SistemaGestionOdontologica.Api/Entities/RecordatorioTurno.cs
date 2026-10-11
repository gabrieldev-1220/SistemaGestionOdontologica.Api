namespace SistemaGestionOdontologica.Api.Entities
{
    public class RecordatorioTurno
    {
        public long IdRecordatorio {  get; set; }
        public int IdTurno { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Destinatario {  get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string Estado { get; set; } = "pendiente";
        public int Intentos { get; set; } = 0;
        public string? UltimoError { get; set; }

        // Relaciones.
        public Turno Turno { get; set; } = null!;
    }
}
