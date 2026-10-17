using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Pacientes
{
    public class ActualizarPacienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2)]
        public string Apellido {  get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(15, MinimumLength = 7)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El DNI debe contener solamente números.")]
        public string Dni {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateOnly? FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }
    }
}
