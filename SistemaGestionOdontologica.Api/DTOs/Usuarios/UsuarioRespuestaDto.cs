namespace SistemaGestionOdontologica.Api.DTOs.Usuarios
{
    public class UsuarioRespuestaDto
    {
        public int IdUsuario {  get; set; }
        public string Username { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? IdOdontologo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
