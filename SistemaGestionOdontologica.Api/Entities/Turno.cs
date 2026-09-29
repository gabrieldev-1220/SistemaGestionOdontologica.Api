using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class Turno
    {
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public EstadoTurno Estado {  get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }

        //RELACIONES.
        public Paciente Paciente { get; set; } = null!;
        public Odontologo Odontologo { get; set; } = null!;

        public ICollection<RecordatorioTurno> Recordatorios { get; set; }
            = new List<RecordatorioTurno>();
    }
}
