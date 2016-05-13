using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Abstractions;
using System.Reflection;

namespace DocCreator
{
    internal static class Configuration
    {
        internal static IFileSystem FileSystem { get; set; }
        private static Dictionary<string, string> Defaults => new Dictionary<string, string>
        {
            ["PackageId"] = "DocCreator.TemplatePackage",
            ["SourceRepository"] = "https://nuget.org/api/v2/"
        };

        internal static string GetSetting(string key)
        {
            
            if (Environment.GetEnvironmentVariable(key) != null)
            {
                return Environment.GetEnvironmentVariable(key);
            }
            if (Environment.GetEnvironmentVariable($"doc_{key}") != null)
            {
                return Environment.GetEnvironmentVariable($"doc_{key}");
            }
            var path = FileSystem.Path.Combine(Environment.CurrentDirectory,
                $"{Assembly.GetEntryAssembly().GetName().Name}.config");
            if (
                FileSystem.File.Exists(path))
            {
                return
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings[key]
                        .Value;
            }
            return Defaults[key];
        }
    }
}