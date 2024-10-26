using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Mapeos
{
    public class SupabaseDBProfile : Profile
    {
        public SupabaseDBProfile()
        {
            CreateMap<Insert_TicketSPB, TicketSPB>();
        }
    }
}
