using Balance.BackEnd.v2.Servicios.AnalisisTecnicoService.Modelos;

namespace Balance.BackEnd.v2.Servicios.AnalisisTecnicoService
{
    public interface IAnalisisTecnicoService
    {
        Task<TicketInfoo?> TicketInfoCdear(string ticket);
        Task<TicketInfoo?> TicketInfoAccion(string ticket);
    }
}
