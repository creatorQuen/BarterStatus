using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BarterStatus.Config;
using BarterStatus.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarterStatus
{
    public static class ServiceConfigurator
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterServicesConfig();
            services.Configure<AppSettings>(configuration);
            return services;
        }
    }
}
