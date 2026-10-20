namespace SistemaGestionOdontologica.Api.Helpers
{
    public class ConflictoNegocioException : Exception
    {
        public ConflictoNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
