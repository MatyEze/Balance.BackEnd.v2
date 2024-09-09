using Balance.BackEnd.v2.Servicios.ActivosService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos
{
    public class DataCompleta
    {
        public List<Movimiento> Movimientos { get; set; }
        public Activos Activos { get; set; }
    }
}
