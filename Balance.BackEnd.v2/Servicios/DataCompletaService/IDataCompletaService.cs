using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;

namespace Balance.BackEnd.v2.Servicios.DataCompletaService
{
    public interface IDataCompletaService
    {
        Task<DataCompleta> ArrayStringToDataCompleta(List<string> data, string brokerResourceKey, string idUsuario);
    }
}
