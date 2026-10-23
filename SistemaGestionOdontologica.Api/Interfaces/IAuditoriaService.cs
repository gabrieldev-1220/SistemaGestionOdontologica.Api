namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface IAuditoriaService
    {
        void Registrar(string accion, int idPaciente);
    }
}
