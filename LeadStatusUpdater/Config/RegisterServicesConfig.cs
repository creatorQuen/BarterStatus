using Microsoft.Extensions.DependencyInjection;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Requests;

namespace LeadStatusUpdater.Config
{
    public static class ServicesConfig
    {
        public static void RegisterServicesConfig(this IServiceCollection services)
        {
            services.AddScoped<ISetVipService, SetVipService>();
            services.AddScoped<IRequestsSender, RequestsSender>();
        }
    }
}
