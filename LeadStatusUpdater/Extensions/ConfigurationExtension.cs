using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace LeadStatusUpdater.Extensions
{
    public static class ConfigurationExtension
    {
        public static void SetEnvironmentVariableForConfiguration(this IConfiguration configuration)
        {
            foreach (var item in configuration.AsEnumerable())
            {
                if (item.Value != null && item.Value.Contains("{{"))
                {
                    var envName = RemoveCurlyBrackets(item.Value);
                    var envValue = Environment.GetEnvironmentVariable(envName);
                    configuration.GetSection(item.Key).Value = envValue;
                }
            }
        }

        private static string RemoveCurlyBrackets(string str)
        {
            return str.Replace("{{", "").Replace("}}", "");
        }

        public const string catalogName = "LogsMyLeads";
        private const string _fileNameAndFormantForLog = "LogMy-.txt";

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
