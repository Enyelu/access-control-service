using access_control.api.Jobs.Implementations;
using access_control.api.Jobs.Interfaces;

namespace access_control.api.Extensions
{
    public static class ConfigureServices
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<ILockJobs, LockJobs>();
        }
    }
}
