using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace BarterStatus
{
    public class Startup
    {
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            _serviceProvider = new ServiceCollection()
                .ConfigureServices(_configuration)
                .BuildServiceProvider();
        }

        public T ProvideService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
