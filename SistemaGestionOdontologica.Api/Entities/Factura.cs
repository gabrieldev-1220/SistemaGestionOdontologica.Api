namespace SistemaGestionOdontologica.Api.Entities
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaEmision {  get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Estado { get; set; } = "emitida";
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }

        // Relaciones.
        public Paciente Paciente { get; set; } = null!;
        public ICollection<FacturaDetalle> Detalles { get; set; }
            =  new List<FacturaDetalle>();
        public ICollection<PagoAplicacion> Aplicaciones { get; set; }
            = new List<PagoAplicacion>();
    }
}
