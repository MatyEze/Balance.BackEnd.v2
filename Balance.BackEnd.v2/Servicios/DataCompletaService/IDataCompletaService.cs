using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.DataCompletaService
{
    public interface IDataCompletaService
    {
        Task<DataCompleta> ArrayStringToDataCompleta(List<string> data, string brokerResourceKey, string idUsuario);
        Task<DataCompleta> GetDataCompletaDB(List<Movimiento> movimientosEnDbFalse, string idUsuario);
    }
}
