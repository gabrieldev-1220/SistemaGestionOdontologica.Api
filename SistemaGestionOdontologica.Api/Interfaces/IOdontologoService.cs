using SistemaGestionOdontologica.Api.DTOs.Odontologos;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IOdontologoService
    {
        Task<ResultadoPaginadoDto<OdontologoRespuestaDto>> ObtenerPaginadosAsync(
            OdontologoFiltroDto filtro, CancellationToken cancellationToken = default);

        Task<OdontologoRespuestaDto?> ObtenerPorIdAsync(
            int idOdontologo,
            CancellationToken cancellationToken = default);

        Task<OdontologoRespuestaDto> CrearAsync(
            CrearOdontologoDto request,
            CancellationToken cancellationToken = default);

        Task<OdontologoRespuestaDto?> ActualizarAsync(
            int idOdontologo,
            ActualizarOdontologoDto request,
            CancellationToken cancellationToken = default);

        Task<bool> DesactivarAsync(
            int idOdontologo,
            CancellationToken cancelled = default);

        Task<bool> ActivarAsync(
            int idOdontologo,
            CancellationToken cancellationToken = default);
    }
}
