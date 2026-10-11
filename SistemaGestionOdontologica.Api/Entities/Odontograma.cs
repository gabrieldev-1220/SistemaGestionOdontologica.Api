namespace SistemaGestionOdontologica.Api.Entities
{
    public class Odontograma
    {
        public int IdOdontograma { get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }


        public Paciente Paciente { get; set; } = null!;
        public Odontologo Odontologo { get; set; } = null!;
        public ICollection<OdontogramaDetalle> Detalle { get; set; }
         = new List<OdontogramaDetalle>();
    }
}
