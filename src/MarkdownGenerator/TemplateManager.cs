using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace MarkdownGenerator
{
	public class TemplateManager
	{
		public TemplateManager(DirectoryInfo templateDirectory, DirectoryInfo outputDirectory)
		{
			Output = outputDirectory.FullName;
			Root = templateDirectory.FullName;
			templateDirectory.CopyDirectory(outputDirectory);
			var engine = RazorEngineService.Create(new TemplateServiceConfiguration()
			{
				TemplateManager = new ResolvePathTemplateManager(new[] {templateDirectory.FullName}),
				CachingProvider = new DefaultCachingProvider(
					c => { }
					)
			});
			Engine.Razor = engine;
			
		}

		private string Output { get; set; }

		private string Root { get; set; }

		public string GenerateHtml(TemplateModel model)
		{
			Model = model;
			var s = Engine.Razor.RunCompile("Template.cshtml", typeof (TemplateModel), model);
			return s;
		}

		private TemplateModel Model { get; set; }

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
				var themeFiles = Directory.EnumerateFiles(Path.Combine(outputDirectory.FullName, "themes"), "*.css", SearchOption.TopDirectoryOnly);
				foreach (var theme in themeFiles.Where(f => !f.Contains(Model.Theme.ToString().ToLower()) && !f.Contains("bootstrap")))
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
