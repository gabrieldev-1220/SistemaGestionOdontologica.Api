using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class RecordatorioTurno
    {
        public long IdRecordatorio {  get; set; }
        public int IdTurno { get; set; }
        public DateTime FechaProgramada { get; set; }
        public TipoRecordatorio Tipo {  get; set; }
        public bool Enviado { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? Error { get; set; }

        // RELACIONES
        public Turno Turno { get; set; } = null;
    }
}
