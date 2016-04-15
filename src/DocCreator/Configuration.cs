using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DocCreator
{
    internal static class Configuration
    {
        private static Dictionary<string, string> Defaults => new Dictionary<string, string>
        {
            ["PackageId"] = "TemplatePackage",
            ["SourceRepository"] = "http://10.7.180.9/nuget"
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
            var path = Path.Combine(Environment.CurrentDirectory,
                $"{Assembly.GetEntryAssembly().GetName().Name}.config");
            if (
                File.Exists(path))
            {
                return
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings[key]
                        .Value;
            }
            return Defaults[key];
        }
    }
}