using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using NuGet;

namespace MarkdownGenerator
{
    public class TemplatePackageManager
    {
        public TemplatePackageManager(string sourceRepo, System.IO.Abstractions.IFileSystem fs = null)
        {
            FileSystem = fs ?? new FileSystem();
            Repository =
                PackageRepositoryFactory.Default.CreateRepository(sourceRepo);
            Local = FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "TemplatePackageCache");
            Manager = new PackageManager(Repository, FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), Local));
        }

        public DirectoryInfoBase GetPackage(string packageId)
        {
            var package = Repository.FindPackage(packageId);
            CleanPackages(packageId, package.Version);
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
            return FileSystem.DirectoryInfo.FromDirectoryName(FileSystem.Path.Combine(Local, $"{packageId}.{package.Version}", "content"));
        }

        private void CleanPackages(string packageId, SemanticVersion version)
        {
            foreach (var package in Manager.LocalRepository.GetPackages().Where(p => p.Id == packageId))
            {
                if (package.IsAbsoluteLatestVersion || package.Version.CompareTo(version) >= 0) return;
                var dir = Manager.PathResolver.GetPackageDirectory(package);
                Manager.UninstallPackage(packageId);
                try
                {
                    FileSystem.Directory.Delete(dir, true);
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
        private System.IO.Abstractions.IFileSystem FileSystem { get; set; }
    }
}
