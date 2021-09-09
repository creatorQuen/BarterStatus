using Microsoft.Extensions.DependencyInjection;
using BarterStatus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarterStatus.Config
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
