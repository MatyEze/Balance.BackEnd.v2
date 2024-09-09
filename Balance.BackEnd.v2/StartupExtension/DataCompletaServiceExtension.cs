using Balance.BackEnd.v2.Servicios.DataCompletaService;

namespace Balance.BackEnd.v2.StartupExtension
{
    public static class DataCompletaServiceExtension
    {
        public static IServiceCollection AddScopesDataCompletaService(this IServiceCollection services)
        {
            services.AddScoped<IDataCompletaService, DataCompletaService>();

            return services;
        }
    }
}
