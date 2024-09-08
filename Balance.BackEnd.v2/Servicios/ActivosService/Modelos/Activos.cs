using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.ActivosService.Modelos
{
    public class Activos
    {
        public List<Moneda> Divisas { get; set; }
        public List<Titulo> Titulos { get; set; }
    }
}
