namespace SistemaGestionOdontologica.Api.Entities
{
    public class HistorialTratamiento
    {
        public int IdHistorialTratamiento { get; set; }
        public int IdHistorial {  get; set; }
        public int IdTrtamiento { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }

        // Relaciones.
        public HistorialClinico HistorialClinico { get; set; } = null!;

        public Tratamiento Tratamiento { get; set;} = null!;
    }
}
