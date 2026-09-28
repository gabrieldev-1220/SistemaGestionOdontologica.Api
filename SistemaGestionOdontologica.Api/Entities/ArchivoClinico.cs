using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class ArchivoClinico
    {
        public long IdArchivo { get; set; }
        public int IdHistorial { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string Ruta {  get; set; } = string.Empty;
        public string TipoMime {  get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public DateTime FechaSubida { get; set; }

        // RELACIONES
        public HistorialClinico Historial { get; set; } = null;
    }
}
