namespace SistemaGestionOdontologica.Api.Entities
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Dni {  get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public string Telefono {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;

        // RELACIONES.
        public ICollection<Turno> Turnos { get; set; }
            = new List<Turno>();
        public ICollection<HistorialClinico> HistorialesClinicos { get; set; }
            = new List<HistorialClinico>();
        public ICollection<Pago> Pagos { get; set; }
            = new List<Pago>();
        public ICollection<Factura> Facturas { get; set; }
            = new List<Factura>();
        public ICollection<Odontograma> Odontogramas { get; set; }
            = new List<Odontograma>();
    }
}
