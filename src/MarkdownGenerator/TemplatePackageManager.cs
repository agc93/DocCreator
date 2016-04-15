using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace MarkdownGenerator
{
    public class TemplatePackageManager
    {
        public TemplatePackageManager(string sourceRepo)
        {
            Repository =
                PackageRepositoryFactory.Default.CreateRepository(sourceRepo);
            Local = Path.Combine(Path.GetTempPath(), "DocPackages");
            Manager = new PackageManager(Repository, Path.Combine(Path.GetTempPath(), Local));
        }

        public DirectoryInfo GetPackage(string packageId)
        {
            var package = Repository.FindPackage(packageId);
            CleanPackages(packageId);
            Manager.InstallPackage(package, true, true);
            //var targetPath = Path.Combine(Local, packageId);
            //if (!Directory.Exists(targetPath))
            //{
            //    Directory.CreateDirectory(targetPath);
            //}
            //foreach (var packageFile in package.GetContentFiles())
            //{
            //    var segments = new List<string> {Local, $"{packageId}.{package.Version}"};
            //    segments.AddRange(packageFile.Path.Split('\\'));
            //    File.Copy(Path.Combine(segments.ToArray()), Path.Combine(targetPath, packageFile.Path));
            //}
            return new DirectoryInfo(Path.Combine(Local, $"{packageId}.{package.Version}", "content"));
        }

        private void CleanPackages(string packageId)
        {
            foreach (var package in Manager.LocalRepository.GetPackages().Where(p => p.Id == packageId))
            {
                var dir = Manager.PathResolver.GetPackageDirectory(package);
                Manager.UninstallPackage(packageId);
                try
                {
                    Directory.Delete(dir);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private PackageManager Manager { get; set; }

        private string Local { get; set; }

        private IPackageRepository Repository { get; set; }
    }
}
