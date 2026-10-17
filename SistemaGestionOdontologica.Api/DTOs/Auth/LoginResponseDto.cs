namespace SistemaGestionOdontologica.Api.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken {  get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAtUtc { get; set; }
        public int IdUsuario {  get; set; }
        public string Username { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
