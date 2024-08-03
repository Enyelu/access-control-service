using access_control.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace access_control.api.Extensions
{
    public static class ConfigureDatabase
    {
        public static void ConfigureApplicationDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
