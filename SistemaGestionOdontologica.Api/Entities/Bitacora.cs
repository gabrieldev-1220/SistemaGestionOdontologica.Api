namespace SistemaGestionOdontologica.Api.Entities
{
    public class Bitacora
    {
        public long IdBotacora { get; set; }
        public int IdUsuario { get; set; }
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Detalles { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; } = null!;
    }
}
