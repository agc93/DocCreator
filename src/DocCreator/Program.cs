using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Fclp;
using MarkdownGenerator;

namespace DocCreator
{
	class Program
	{
		static void Main(string[] args)
		{
			var p = BuildCommandParser();
			var parserResult = p.Parse(args);
			if (parserResult.HasErrors || parserResult.HelpCalled)
			{
				Console.WriteLine(parserResult.ErrorText);
			}
			else
			{
				GenerateDocument(p.Object);
				try
				{
					System.Diagnostics.Process.Start(p.Object.OutputDirectory);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		}

		private static void GenerateDocument(ApplicationArgs args)
		{
			var files = new List<string>();
			if (File.Exists(args.InputFile))
			{
				files.Add(args.InputFile);
			}
			else
			{
				if (Directory.Exists(args.InputFile))
				{
					files.AddRange(Directory.GetFileSystemEntries(args.InputFile, "*.md", SearchOption.AllDirectories));
				}
				else
				{
					Environment.Exit(1);
				}
			}
			var manager = new TemplatePackageManager(Configuration.GetSetting("SourceRepository"));
			var package = manager.GetPackage(Configuration.GetSetting("PackageId"));
			if (!Directory.Exists(args.OutputDirectory))
			{
				Directory.CreateDirectory(args.OutputDirectory);
			}
			var templater = new TemplateManager(package, new DirectoryInfo(args.OutputDirectory));
			var directory = GenerateFiles(templater, args, files);
			//directory.CopyDirectory(new DirectoryInfo(args.OutputDirectory));
		}

		private static DirectoryInfo GenerateFiles(TemplateManager templater, ApplicationArgs args, List<string> files)
		{
			var outputFiles = new List<FileInfo>();
			foreach (var file in files.Select(f => new FileInfo(f)))
			{
				var html = templater.GenerateHtml(args.ToModel(File.ReadAllText(file.FullName)));
				if (args.RewriteLinks)
				{
					html = LinkConverter.RewriteLinks(html);
				}
				outputFiles.Add(templater.WriteToFile(html, file.Name.Replace(file.Extension, ".html")));
			}
			return outputFiles.First().Directory;
		}

		private static FluentCommandLineParser<ApplicationArgs> BuildCommandParser()
		{
			var p = new FluentCommandLineParser<ApplicationArgs> {IsCaseSensitive = false};
			p.Setup(arg => arg.InputFile)
				.As('i', "input")
				.Required()
				.WithDescription("Input markdown file");
			p.Setup(arg => arg.OutputDirectory)
				.As('o', "output-dir")
				.SetDefault(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")))
				.WithDescription("Directory for output files");
			p.Setup(arg => arg.Theme)
				.As('b', "theme")
				.SetDefault(Theme.Bootstrap)
				.WithDescription("Bootswatch theme to use. Defaults to plain bootstrap");
			p.Setup(arg => arg.Title)
				.As('t', "title")
				.SetDefault("Documentation")
				.WithDescription("Title for the output file. Defaults to 'Documentation'.");
			p.Setup(arg => arg.RewriteLinks)
				.As("rewrite-links")
				.SetDefault(false)
				.WithDescription("Rewrites relative .md links in processed documents");
			p.SetupHelp("?", "h","help").Callback(t => Console.WriteLine(t));
			return p;
		}
	}
}
