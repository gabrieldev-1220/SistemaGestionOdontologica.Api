using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Odontologos
{
    public class OdontologoFiltroDto
    {
        [StringLength(100)]
        public string? Busqueda {  get; set; }

        public bool? Activo { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int Pagina { get; set; } = 1;

        [Range(1, 100)]
        public int TamanoPagina { get; set; } = 10;
    }
}
