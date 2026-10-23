namespace SistemaGestionOdontologica.Api.DTOs.Odontologos
{
    public class OdontologoRespuestaDto
    {
        public int IdOdontologo {  get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido {  get; set; } = string.Empty;
        public string Matricula {  get; set; } = string.Empty;
        public string? Telefono {  get; set; }
        public string? Email {  get; set; }
        public string? Especialidad { get; set; }
        public bool Activo { get; set; }
    }
}
