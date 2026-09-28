using SistemaGestionOdontologica.Api.Entities.Enums;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public RolUsuario Rol {  get; set; }
        public int? IdOdontologo { get; set; }
        public bool Activo {  get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }

        // RELACIONES
        public Odontologo? Odontologo { get; set; }

        public ICollection<Bitacora> Bitacoras { get; set; }
            = new List<Bitacora>();
    }
}
