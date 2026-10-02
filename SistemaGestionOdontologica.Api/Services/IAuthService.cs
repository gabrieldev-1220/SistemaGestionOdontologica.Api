using SistemaGestionOdontologica.Api.DTOs.Auth;

namespace SistemaGestionOdontologica.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
