using Balance.BackEnd.v2.Servicios.MovimientosService;
using Balance.BackEnd.v2.Servicios.MovimientosService.Mapeos;

namespace Balance.BackEnd.v2.StartupExtension
{
    public static class MovimientoServiceExtension
    {
        public static IServiceCollection AddScopesMovimientosService(this IServiceCollection services)
        {
            services.AddScoped<IMovimientosService, MovimientosService>();
            services.AddAutoMapper(typeof(MovimientoServiceProfile));

            return services;
        }
    }
}
