namespace SistemaGestionOdontologica.Api.Entities
{
    public class PagoAplicacion
    {
        public int IdPagoAplicacion { get; set; }
        public int IdPago {  get; set; }
        public int IdFactura { get; set; }
        public decimal MontoAplicado { get; set; }
        public DateTime FechaAplicacion { get; set; }

        public Pago Pago { get; set; } = null!;
        public Factura Factura { get; set; } = null!;
    }
}
