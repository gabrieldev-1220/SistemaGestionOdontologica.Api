namespace SistemaGestionOdontologica.Api.Entities
{
    public class Tratamiento
    {
        public int IdTratamiento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion {  get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }

        // Relaciones.
        public ICollection<HistorialClinico> HistorialTratamientos { get; set; }
            = new List<HistorialClinico>();
        public ICollection<FacturaDetalle> FacturaDetalles { get; set; }
            = new List<FacturaDetalle>();
    }
}
