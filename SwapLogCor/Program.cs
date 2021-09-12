using Serilog;
using SwapLogCor.Services;
using System;

namespace SwapLogCor
{
    class Program
    {
        private static Startup _startup;
        public static void Main(string[] args)
        {
            _startup = new Startup();
            _startup.ProvideService<ISetVipService>().Process();
            Console.ReadLine();
        }

    }
}
