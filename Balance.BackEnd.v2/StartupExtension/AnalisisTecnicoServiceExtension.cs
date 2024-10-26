using Balance.BackEnd.v2.Servicios.AnalisisTecnicoService;

namespace Balance.BackEnd.v2.StartupExtension
{
    public static class AnalisisTecnicoServiceExtension
    {
        public static IServiceCollection AddScopesAnalisisTecnicoService(this IServiceCollection services)
        {
            services.AddScoped<IAnalisisTecnicoService, AnalisisTecnicoService>();

            return services;
        }
    }
}
