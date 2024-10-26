using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.MovimientosService.Mapeos
{
    public class MovimientoServiceProfile : Profile
    {
        public MovimientoServiceProfile()
        {
            CreateMap<Movimiento, MovimientoSPB>()
                .ForMember(dest => dest.NrMovimiento, opt => opt.MapFrom(src => src.NroMovimiento))
                .ForMember(dest => dest.Cantidad, opt => opt.MapFrom(src => src.Cantidad))
                .ForMember(dest => dest.IdBroker, opt => opt.MapFrom(src => src.Broker.Id))
                .ForMember(dest => dest.IdTipoMovimiento, opt => opt.MapFrom(src => src.TipoMovimiento.Id))
                .ForMember(dest => dest.IdTicket, opt => opt.MapFrom(src => src.Ticket.Id))
                .ForMember(dest => dest.FechaMovimiento, opt => opt.MapFrom(src => src.FechaMovimiento))
                .ForMember(dest => dest.IdTipoPrecio, opt => opt.MapFrom(src => src.Precio.IdTipo))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Precio.Cantidad))
                .ForMember(dest => dest.IdTipoMontoTotal, opt => opt.MapFrom(src => src.MontoTotal.IdTipo))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.MontoTotal.Cantidad))
                .ForMember(dest => dest.EnDb, opt => opt.MapFrom(src => src.EnDb))
                .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.Observaciones));
            CreateMap<BrokerSPB, Broker>();
        }
    }
}
