using SistemaGestionOdontologica.Api.DTOs.Usuarios;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioRespuestaDto> CrearAsync(CrearUsuarioDto request, CancellationToken cancellationToken = default);
    }
}
