using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public string? Observaciones { get; set; }

        // RELACIONES
        public Paciente Paciente { get; set; } = null;
    }
}
