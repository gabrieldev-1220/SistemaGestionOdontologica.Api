using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.Entities
{
    public class Paciente
    {
        public int IdPaciente { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public string Dni { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string Email { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        // RELACIONES
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
        public ICollection<HistorialClinico> HistorialesClinicos { get; set; }
            = new List<HistorialClinico>();

        public ICollection<Pago> Pagos { get; set; }
            = new List<Pago>();
    }
}
