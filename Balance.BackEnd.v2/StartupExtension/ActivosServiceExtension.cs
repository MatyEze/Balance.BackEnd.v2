using Balance.BackEnd.v2.Servicios.ActivosService;

namespace Balance.BackEnd.v2.StartupExtension
{
    public static class ActivosServiceExtension
    {
        public static IServiceCollection AddScopesActivosService(this IServiceCollection services)
        {
            services.AddScoped<IActivosService, ActivosService>();

            return services;
        }
    }
}
