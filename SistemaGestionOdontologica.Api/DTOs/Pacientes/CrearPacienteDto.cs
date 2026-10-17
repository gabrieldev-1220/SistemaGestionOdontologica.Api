using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Pacientes
{
    public class CrearPacienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        public string Apellido {  get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 15 caracteres.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El DNI debe contener solamente números.")]
        public string Dni {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateOnly? FechaNacimiento { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico noes válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
        public string? Direccion { get; set; }
    }
}