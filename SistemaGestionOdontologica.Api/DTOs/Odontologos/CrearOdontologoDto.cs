using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Odontologos
{
    public class CrearOdontologoDto
    {
        [Required(ErrorMessage = "El nombre es obligatoria.")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido {  get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [StringLength(30)]
        public string Matricula {  get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono {  get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(100)]
        public string? Email {  get; set; }

        [StringLength(100)]
        public string? Especialidad { get; set; }
    }
}
