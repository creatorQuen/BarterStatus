using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace LeadStatusUpdater.Extention
{
    public static class ConfigurationExtensions
    {
        public const string catalogName = "Logs";
        private const string _fileNameAndFormantForLog = "Log-.txt";

        public static void ConfigureLogger(this IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                    configuration.GetPathToFile(),
                    rollingInterval: RollingInterval.Day)
                    .CreateLogger();
        }


        public static string GetPathToFile(this IConfiguration configuration)
        {
            var pathToFolder = CheckFolderIfAbsentThenCreate();
            var path = Path.Combine(pathToFolder, _fileNameAndFormantForLog);
            return path;
        }

        private static string GetPathToFolder()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            string pathToFolder = Path.Combine(currentDirectory, catalogName);

            return pathToFolder;
        }

        private static string CheckFolderIfAbsentThenCreate()
        {
            var pathToFolder = GetPathToFolder();
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }
            return pathToFolder;
        }
    }
}
