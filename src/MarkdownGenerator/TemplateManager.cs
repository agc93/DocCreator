using System;
using System.IO;
using System.Linq;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace MarkdownGenerator
{
    public class TemplateManager : IDisposable
    {
        public TemplateManager(DirectoryInfo templateDirectory, DirectoryInfo outputDirectory)
        {
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

        private string Workspace { get; }

        private string Output { get; }

        private TemplateModel Model { get; set; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Workspace);
                Console.WriteLine("Cleaning workspace...");
            }
            catch
            {
                // ignored
            }
        }

        private string CreateWorkspace(string fullName)
        {
            var di = new DirectoryInfo(fullName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"dc-{Guid.NewGuid()}");
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            di.CopyDirectory(new DirectoryInfo(tempPath));
            return tempPath;
        }

        public string GenerateHtml(TemplateModel model)
        {
            Model = model;
            var s = Engine.Razor.RunCompile("Template.cshtml", typeof(TemplateModel), model);
            return s;
        }

        public FileInfo WriteToFile(string content, string fileName)
        {
            var path = Path.Combine(Output, fileName);
            CleanupOutput(new DirectoryInfo(Output));
            File.WriteAllText(path, content, Encoding.UTF8);
            return new FileInfo(path);
        }

        private void CleanupOutput(DirectoryInfo outputDirectory)
        {
            try
            {
                var templatePath = Path.Combine(outputDirectory.FullName, "Template.cshtml");
                if (File.Exists(templatePath)) File.Delete(templatePath);
                if (Model == null) return;
                var themeFiles = Directory.EnumerateFiles(Path.Combine(outputDirectory.FullName, "themes"), "*.css",
                    SearchOption.TopDirectoryOnly);
                foreach (
                    var theme in
                        themeFiles.Where(f => !f.Contains(Model.Theme.ToString().ToLower()) && !f.Contains("bootstrap"))
                    )
                {
                    File.Delete(Path.Combine(outputDirectory.FullName, "themes", theme));
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}