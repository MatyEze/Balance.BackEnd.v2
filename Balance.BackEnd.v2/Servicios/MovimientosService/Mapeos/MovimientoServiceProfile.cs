using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.MovimientosService.Mapeos
{
    public class MovimientoServiceProfile : Profile
    {
        public MovimientoServiceProfile()
        {
            CreateMap<Movimiento, MovimientoSPB>();
            CreateMap<BrokerSPB, Broker>();
        }
    }
}
