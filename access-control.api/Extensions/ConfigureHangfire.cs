using Hangfire;

namespace access_control.api.Extensions
{
    public static class ConfigureHangfire
    {
        public static void ConfigureHangfireSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            // Add Hangfire server
            services.AddHangfireServer();
        }
    }
}
