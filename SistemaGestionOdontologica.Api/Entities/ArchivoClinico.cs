namespace SistemaGestionOdontologica.Api.Entities
{
    public class ArchivoClinico
    {
        public long IdArchivo { get; set; }
        public int IdHistorial { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreArchiv {  get; set; } = string.Empty;
        public string Ruta {  get; set; } = string.Empty; // Ubicación privada del archivo.
        public string TipoMime {  get; set; } = string.Empty; // Tipo de contenido.
        public long TamanoBytes { get; set; } // Tamaño del archivo.
        public DateTime FechaSubida { get; set; }

        //Relación.
        public HistorialClinico HistorialClinico { get; set; } = null!;
    }
}
