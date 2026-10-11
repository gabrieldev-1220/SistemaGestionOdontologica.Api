namespace SistemaGestionOdontologica.Api.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? IdOdontologo { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }

        //Relacion
        public Odontologo? Odontologo { get; set; }
        public ICollection<Bitacora> RegistrosBitacora { get; set; }
            = new List<Bitacora>();
        public ICollection<RefreshToken> RefreshTokens { get; set; }
            = new List<RefreshToken>();
    }
}
