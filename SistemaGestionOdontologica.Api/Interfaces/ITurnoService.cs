using SistemaGestionOdontologica.Api.DTOs.Paginacion;
using SistemaGestionOdontologica.Api.DTOs.Turnos;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface ITurnoService
    {
        Task<ResultadoPaginadoDto<TurnoRespuestaDto>> ObtenerPaginadosAsync(TurnoFiltroDto filtro, CancellationToken cancellationToken = default);

        Task<TurnoRespuestaDto?> ObtenerPorIdAsync(int idTurno, CancellationToken cancellationToken = default);

        Task<TurnoRespuestaDto> CrearAsync(CrearTurnoDto request, CancellationToken cancellationToken = default);

        Task<TurnoRespuestaDto?> ActualizarAsync(int idTurno, ActualizarTurnoDto request, CancellationToken cancellationToken = default);
    }
}
