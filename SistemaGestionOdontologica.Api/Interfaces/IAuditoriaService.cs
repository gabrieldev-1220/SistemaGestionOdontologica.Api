namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IAuditoriaService
    {
        // Método utilizado por PacienteService.
        void Registrar(string accion, int idPaciente);

        // Método para diferentes entidades.
        void Registrar(string accion, string entidad, int idEntidad);
    }
}
