using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.HistorialesClinicos
{
    public sealed class HistorialClinicoFiltroDto
    {
        [Range(1, int.MaxValue)]
        public int? IdPaciente { get; set; }

        [Range(1, int.MaxValue)]
        public int? IdOdontologo { get; set; }

        public DateOnly? FechaDesde { get; set; }
        public DateOnly? FechaHasta { get; set; }

        [Range(1, int.MaxValue)]
        public int Pagina { get; set; } = 1;

        [Range(1, 100)]
        public int TamanoPagina { get; set; } = 10;
    }
}
