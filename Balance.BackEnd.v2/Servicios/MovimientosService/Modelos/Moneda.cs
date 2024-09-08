namespace Balance.BackEnd.v2.Servicios.MovimientosService.Modelos
{
    public class Moneda
    {
        public int IdTipo { get; set; }
        public string Tipo { get; set; }
        public string Simbolo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
    }
}
