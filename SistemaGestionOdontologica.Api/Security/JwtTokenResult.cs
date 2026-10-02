/// Esta clase representa solo el resultado de generar un JWT.

namespace SistemaGestionOdontologica.Api.Security
{
    public class JwtTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
