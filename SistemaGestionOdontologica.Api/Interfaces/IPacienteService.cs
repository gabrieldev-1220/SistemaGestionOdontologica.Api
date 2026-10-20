using SistemaGestionOdontologica.Api.DTOs.Pacientes;
using SistemaGestionOdontologica.Api.DTOs.Paginacion;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IPacienteService
    {
        Task<ResultadoPaginadoDto<PacienteRespuestaDto>> ObtenerPaginadosAsync(
            PacienteFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<PacienteRespuestaDto?> ObtenerPorIdAsync(int idPaciente, CancellationToken cancellationToken = default);
        Task<PacienteRespuestaDto> CrearAsync(CrearPacienteDto request, CancellationToken cancellationToken = default);
        Task<PacienteRespuestaDto?> ActualizarAsync(int idPaciente, ActualizarPacienteDto request, CancellationToken cancellationToken = default);
        Task<bool> DesactivarAsync(int idPaciente, CancellationToken cancellationToken = default);
        Task<bool> ActivarAsync(int idPaciente, CancellationToken cancellationToken);
    }
}
