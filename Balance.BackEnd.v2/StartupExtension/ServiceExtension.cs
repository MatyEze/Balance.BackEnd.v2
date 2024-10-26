namespace Balance.BackEnd.v2.StartupExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddScopesServices(this IServiceCollection services)
        {
            services.AddScopesMovimientosService();
            services.AddScopesActivosService();
            services.AddScopesDataCompletaService();
            services.AddScopesAnalisisTecnicoService();

            return services;
        }
    }
}
