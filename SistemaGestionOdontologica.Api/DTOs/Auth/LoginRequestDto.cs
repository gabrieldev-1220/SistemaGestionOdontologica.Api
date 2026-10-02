/// Este DTO representa los datos  que entran a la API.

using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}
