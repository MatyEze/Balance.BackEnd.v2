using Balance.BackEnd.v2.Servicios.ActivosService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.ActivosService
{
    public interface IActivosService
    {
        Task<Activos> GenerarActivos(List<Movimiento> movimientos);
    }
}
