namespace SistemaGestionOdontologica.Api.Entities
{
    public class Bitacora
    {
        public long IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Detalles { get; set; }

        // RELACIONES.
        public Usuario Usuario { get; set; } = null!;
    }
}
