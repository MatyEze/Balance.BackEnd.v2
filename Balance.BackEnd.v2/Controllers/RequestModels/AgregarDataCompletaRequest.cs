using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;

namespace Balance.BackEnd.v2.Controllers.RequestModels
{
    public class AgregarDataCompletaRequest
    {
        public DataCompleta DataCompletaCargada { get; set; }
        public DataCompleta DataCompletaNueva { get; set; }
    }
}
