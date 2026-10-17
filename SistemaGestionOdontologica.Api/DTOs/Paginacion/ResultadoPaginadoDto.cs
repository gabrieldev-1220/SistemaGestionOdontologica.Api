namespace SistemaGestionOdontologica.Api.DTOs.Paginacion
{
    public class ResultadoPaginadoDto<T>
    {
        public List<T> Datos { get; set; } = new();
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}