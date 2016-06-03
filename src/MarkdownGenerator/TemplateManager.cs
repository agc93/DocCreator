using System;
using System.IO.Abstractions;
using System.Linq;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace MarkdownGenerator
{
    public class TemplateManager : IDisposable
    {
        public TemplateManager(IFileSystem fs, DirectoryInfoBase templateDirectory, DirectoryInfoBase outputDirectory)
        {
            FileSystem = fs;
            Output = outputDirectory.FullName;
            Workspace = CreateWorkspace(templateDirectory.FullName);
            var engine = RazorEngineService.Create(new TemplateServiceConfiguration
            {
                TemplateManager = new ResolvePathTemplateManager(new[] {Workspace}),
                CachingProvider = new DefaultCachingProvider(
                    c => { }
                    )
            });
            Engine.Razor = engine;
        }

        private IFileSystem FileSystem { get; set; }

        private string Workspace { get; }

        private string Output { get; }

        private TemplateModel Model { get; set; }

        public void Dispose()
        {
            try
            {
                FileSystem.Directory.Delete(Workspace, true);
                Console.WriteLine("Cleaning workspace...");
            }
            catch
            {
                // ignored
            }
        }

        private string CreateWorkspace(string fullName)
        {
            var di = FileSystem.DirectoryInfo.FromDirectoryName(fullName);
            var tempPath = FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), $"dc-{Guid.NewGuid()}");
            if (!FileSystem.Directory.Exists(tempPath)) FileSystem.Directory.CreateDirectory(tempPath);
            di.CopyDirectory(FileSystem.DirectoryInfo.FromDirectoryName(tempPath));
            return tempPath;
        }

        public string GenerateHtml(TemplateModel model)
        {
            Model = model;
            var s = Engine.Razor.RunCompile("Template.cshtml", typeof(TemplateModel), model);
            return s;
        }

        public FileInfoBase WriteToFile(string content, string fileName)
        {
            var path = FileSystem.Path.Combine(Output, fileName);
            FileSystem.DirectoryInfo.FromDirectoryName(Workspace)
                .CopyDirectory(
                    FileSystem.DirectoryInfo.FromDirectoryName(
                        FileSystem.FileInfo.FromFileName(path).Directory.FullName));
            CleanupOutput(FileSystem.DirectoryInfo.FromDirectoryName(Output));
            FileSystem.File.WriteAllText(path, content, Encoding.UTF8);
            return FileSystem.FileInfo.FromFileName(path);
        }

        private void CleanupOutput(DirectoryInfoBase outputDirectory)
        {
            try
            {
                var templatePath = FileSystem.Path.Combine(outputDirectory.FullName, "Template.cshtml");
                if (FileSystem.File.Exists(templatePath)) FileSystem.File.Delete(templatePath);
                if (Model == null) return;
                var themeFiles = FileSystem.Directory.EnumerateFiles(FileSystem.Path.Combine(outputDirectory.FullName, "themes"), "*.css",
                    System.IO.SearchOption.TopDirectoryOnly);
                foreach (
                    var theme in
                        themeFiles.Where(f => !f.Contains(Model.Theme.ToString().ToLower()) && !f.Contains("bootstrap"))
                    )
                {
                    FileSystem.File.Delete(FileSystem.Path.Combine(outputDirectory.FullName, "themes", theme));
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}