using SistemaGestionOdontologica.Api.DTOs.Auth;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

        Task<LoginResponseDto?> RenovarTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

        Task<bool> CerrarSesionAsync(string refreshToken, int IdUsuario, CancellationToken cancellationToken = default);

        Task<UsuarioAutenticadoDto?> ObtenerUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default);
    }
}
