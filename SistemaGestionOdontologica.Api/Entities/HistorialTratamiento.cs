namespace SistemaGestionOdontologica.Api.Entities
{
    public class HistorialTratamiento
    {
        public int IdHistorialTratamiento { get; set; }
        public int IdHistorial {  get; set; }
        public int IdTratamiento { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // RELACIONES
        public HistorialClinico Historial { get; set; } = null;

        public Tratamiento Tratamiento { get; set; } = null;
    }
}
