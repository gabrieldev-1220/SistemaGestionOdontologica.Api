using System.ComponentModel.DataAnnotations;

namespace SistemaGestionOdontologica.Api.DTOs.HistorialesClinicos
{
    public sealed class CrearHistorialClinicoDto
    {
        [Range(1, int.MaxValue)]
        public int IdPaciente { get; set; }

        public int? IdTurno { get; set; }

        [Required(ErrorMessage = "La fecha de atención es obligatoria.")]
        public DateTime? Fecha { get; set; }

        [Required(ErrorMessage = "El motivo de consulta es obligatorio.")]
        [StringLength(2000, MinimumLength = 3)]
        public string MotivoConsulta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
        [StringLength(4000, MinimumLength = 3)]
        public string Diagnostico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las observaciones son obligatorias.")]
        [StringLength(4000)]
        public string Observaciones { get; set; } = string.Empty;
    }
}