namespace SistemaGestionOdontologica.Api.Helpers
{
    public class ValidacionNegocioException : Exception
    {
        public ValidacionNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
