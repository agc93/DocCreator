using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Fclp;
using MarkdownGenerator;

namespace DocCreator
{
	class Program
	{
		private static IFileSystem FileSystem { get; set; }
		static void Main(string[] args)
		{
			FileSystem = new FileSystem();
			var p = BuildCommandParser();
			var parserResult = p.Parse(args);
			if (parserResult.HasErrors || parserResult.HelpCalled)
			{
				Console.WriteLine(parserResult.ErrorText);
			}
			else
			{
				var output = GenerateDocument(p.Object);
			    if (p.Object.QuietMode) return;
			    try
			    {
			        System.Diagnostics.Process.Start(output?.FullName ?? p.Object.OutputDirectory);
			    }
			    catch (Exception)
			    {
			        // ignored
			    }
			}
		}

		private static DirectoryInfoBase GenerateDocument(ApplicationArgs args)
		{
			var fs = FileSystem;
			var files = new List<string>();
			if (fs.File.Exists(args.InputFile))
			{
				files.Add(args.InputFile);
			}
			else
			{
				if (fs.Directory.Exists(args.InputFile))
				{
					files.AddRange(fs.Directory.EnumerateFiles(args.InputFile, "*.md"));
				}
				else
				{
					Environment.Exit(1);
				}
			}
			var manager = new TemplatePackageManager(Configuration.GetSetting("SourceRepository"));
			var package = manager.GetPackage(args.OfflineMode ? Configuration.GetSetting("OfflinePackageId") : Configuration.GetSetting("PackageId"));
			if (!fs.Directory.Exists(args.OutputDirectory))
			{
				fs.Directory.CreateDirectory(args.OutputDirectory);
			}
			using (var templater = new TemplateManager(fs, package, fs.DirectoryInfo.FromDirectoryName(args.OutputDirectory)))
			{
				return GenerateFiles(templater, args, files);
			}
		}

		private static DirectoryInfoBase GenerateFiles(TemplateManager templater, ApplicationArgs args, List<string> files)
		{
			var outputFiles = new List<FileInfoBase>();
			foreach (var file in files.Select(f => FileSystem.FileInfo.FromFileName(f)))
			{
				var html = templater.GenerateHtml(args.ToModel(FileSystem.File.ReadAllText(file.FullName)));
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
				.SetDefault(FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), Guid.NewGuid().ToString("N")))
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
		    p.Setup(arg => arg.OfflineMode)
		        .As("offline")
		        .SetDefault(false)
		        .WithDescription("Creates templates in offline mode (stores JS and CSS in output dir)");
		    p.Setup(arg => arg.QuietMode)
		        .As('q', "quiet")
		        .SetDefault(false)
		        .WithDescription("Runs non-interactively, does not open results folder.");
			p.SetupHelp("?", "h","help").Callback(t => Console.WriteLine(t));
			return p;
		}
	}
}
