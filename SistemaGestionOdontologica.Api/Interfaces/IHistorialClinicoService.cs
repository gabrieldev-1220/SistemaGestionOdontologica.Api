using SistemaGestionOdontologica.Api.DTOs.HistorialesClinicos;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IHistorialClinicoService
    {
        Task<ResultadoPaginadoDto<HistorialClinicoRespuestaDto>> ObtenerPaginadosAsync(
            HistorialClinicoFiltroDto filtro, CancellationToken cancellationToken = default);

        Task<HistorialClinicoRespuestaDto?> ObtenerPorIdAsync(int idHistorial, CancellationToken cancellationToken = default);

        Task<HistorialClinicoRespuestaDto> CrearAsync(CrearHistorialClinicoDto request,
            CancellationToken cancellationToken = default);
    }
}
