namespace SistemaGestionOdontologica.Api.Entities
{
    public class Odontologo
    {
        public int IdOdontologo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido {  get; set; } = string.Empty;
        public string Matricula {  get; set; } = string.Empty;
        public string Telefono {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Especialidad {  get; set; } = string.Empty;
        public bool Activo { get; set; }

        // RELACIONES
        public ICollection<Turno> Turnos { get; set; }
            = new List<Turno>();

        public ICollection<HistorialClinico> HistorialesClinicos { get; set; }
            = new List<HistorialClinico>();

        public ICollection<Usuario> Usuarios { get; set; }
            = new List<Usuario>();
    }
}
