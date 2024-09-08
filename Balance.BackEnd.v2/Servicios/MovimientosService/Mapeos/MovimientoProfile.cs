using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.MovimientosService.Mapeos
{
    public class MovimientoProfile : Profile
    {
        public MovimientoProfile()
        {
            CreateMap<Movimiento, MovimientoSPB>();
        }
    }
}
