namespace SistemaGestionOdontologica.Api.Entities
{
    public class OdontogramaDetalle
    {
        public int IdOdontogramaDetalle { get; set; }
        public int IdOdontograma {  get; set; }
        public int NumeroPieza { get; set; }
        public string Cara { get; set; } = string.Empty;
        public string Estado {  get; set; } = string.Empty;
        public string? Observaciones {  get; set; }

        public Odontograma Odontograma { get; set; } = null!;
    }
}
