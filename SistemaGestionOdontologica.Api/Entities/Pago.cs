namespace SistemaGestionOdontologica.Api.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Observaciones {  get; set; }

        //Relaciones
        public Paciente Paciente { get; set; } = null!;
        public ICollection<PagoAplicacion> Aplicaciones { get; set; }
            = new List<PagoAplicacion>();
    }
}
