using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "El refresh token es obligatorio.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
