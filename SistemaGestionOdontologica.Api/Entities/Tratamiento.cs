namespace SistemaGestionOdontologica.Api.Entities
{
    public class Tratamiento
    {
        public int IdTratamiento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion {  get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // RELACIONES.
        public ICollection<HistorialTratamiento> HistorialTratamientos { get; set; }
            = new List<HistorialTratamiento>();
    }
}
