using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.MovimientosService
{
    public interface IMovimientosService
    {
        Task<List<Movimiento>> ProcesarMovimientos(List<string> data, string brokerResourceKey, string idUsuario);
    }
}
