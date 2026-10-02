/// Esto representa los datos que la API devolverá cuando el login sea exitoso.

namespace SistemaGestionOdontologica.Api.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public int IdUsuario { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
