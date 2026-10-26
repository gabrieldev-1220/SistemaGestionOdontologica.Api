using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.Turnos
{
    public sealed class CrearTurnoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente válido.")]
        public int IdPaciente { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un odontólogo válido.")]
        public int IdOdontologo { get; set; }

        [Required(ErrorMessage = "La fecha y hora de inicio son obligatorios.")]
        public DateTime? FechaHoraInicio { get; set; }

        [Required(ErrorMessage = "La fecha y hora de finalización son obligatorios.")]
        public DateTime? FechaHoraFin {  get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden superar los 1000 cacteres.")]
        public string? Observaciones { get; set; }
    }
}
