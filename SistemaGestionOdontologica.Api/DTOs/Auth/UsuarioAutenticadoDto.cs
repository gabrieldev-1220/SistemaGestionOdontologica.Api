namespace SistemaGestionOdontologica.Api.DTOs.Auth
{
    public class UsuarioAutenticadoDto
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Rol {  get; set; } = string.Empty;
        public int? IdOdontologo { get; set; }
    }
}
