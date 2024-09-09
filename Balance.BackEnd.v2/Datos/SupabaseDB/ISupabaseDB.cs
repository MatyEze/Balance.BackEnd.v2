using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Datos.SupabaseDB
{
    public interface ISupabaseDB
    {
        Task<List<TipoTicketSPB>> GetTiposTicket();
        Task<List<TipoMonedaSPB>> GetTiposMoneda();
        Task<List<TipoMovimientoSPB>> GetTiposMovimiento();
        Task<BrokerSPB?> GetBrokerSPBByCabecera(string cabecera);
        Task<BrokerSPB?> GetBrokerSPBByResourceKey(string resourceKey);
        Task<List<TicketSPB>> GetTicketByString(string ticketString);
        /// <summary>
        /// Inserta un ticket nuevo en DB y lo devulve con la Id creada
        /// </summary>
        /// <param name="ticketString"></param>
        /// <param name="IdTipo"></param>
        /// <returns>TicketSPB con la Id creada</returns>
        Task<TicketSPB> InsertTicket(string ticketString, int idTipo, string descripcion);
        Task<int> InsertMovimientos(List<Movimiento> movimientos, string idUsuario);
        Task<string> InsertMovimentosSPB(List<MovimientoSPB> movimientosSPB);
        Task<List<MovimientoSPB>> GetMovimientosSPB(string idUsuario);
    }
}
