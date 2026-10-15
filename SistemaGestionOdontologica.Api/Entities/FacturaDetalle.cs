namespace SistemaGestionOdontologica.Api.Entities
{
    public class FacturaDetalle
    {
        public int IdFacturaDetalle { get; set; }
        public int IdFactura {  get; set; }
        public int? IdTratamiento { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }

        public Factura Factura { get; set; } = null!;
        public Tratamiento? Tratamiento { get; set; }
    }
}
