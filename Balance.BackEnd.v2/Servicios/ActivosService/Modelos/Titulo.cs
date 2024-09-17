using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.ActivosService.Modelos
{
    public class Titulo
    {
        public Ticket Ticket { get; set; }
        public int Cantidad { get; set; }
        public Moneda? Precio { get; set; }
        public List<Movimiento> Historial { get; set; } = new List<Movimiento>();

    }
}
