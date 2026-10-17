using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Pacientes
{
    public class PacienteFiltroDto
    {
        [StringLength(100)]
        public string? Busqueda {  get; set; }

        public bool? Activo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor que cero.")]
        public int Pagina { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
        public int TamanoPagina { get; set; }
    }
}
