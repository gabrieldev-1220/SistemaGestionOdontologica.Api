namespace SistemaGestionOdontologica.Api.Entities
{
    public class RefreshToken
    {
        public long IdRefreshToken { get; set; }
        public int IdUsuario { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public DateTime? FechaRevocacion { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
