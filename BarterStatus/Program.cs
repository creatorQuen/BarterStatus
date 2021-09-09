using Serilog;
using BarterStatus.Services;
using System;

namespace BarterStatus
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
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Log.Logger = new LoggerConfiguration()
    //           .MinimumLevel.Debug()
    //           .WriteTo.Console()
    //           .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    //           .CreateLogger();

    //        Log.Information("Hello, world!");

    //        int a = 10, b = 0;
    //        try
    //        {
    //            Log.Debug("Dividing {A} by {B}", a, b);
    //            Console.WriteLine(a / b);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(ex, "Something went wrong");
    //        }
    //        finally
    //        {
    //            Log.CloseAndFlush();
    //        }
    //    }
    //}
}
