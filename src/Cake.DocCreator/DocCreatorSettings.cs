using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Path = Cake.Core.IO.Path;

namespace Cake.DocCreator
{
	public class DocCreatorSettings : ToolSettings
	{
		public DocCreatorSettings(ICakeContext context, Path input, DirectoryPath outputPath)
		{
		    Context = context;
			InputPath = input;
			OutputPath = outputPath;
		}

	    private ICakeContext Context { get; set; }

	    public DocCreatorSettings()
		{
		}

		public IList<string> Arguments { get; set; } = new List<string>();

		public Path InputPath { get; set; }

		private bool RewriteLinks { get; set; }

		private Theme Theme { get; set; }

		private string Title { get; set; }

		public DirectoryPath OutputPath { get; private set; }

		private bool Debug { get; set; }

		public void Build(ProcessArgumentBuilder args)
		{
			if (OutputPath == null || InputPath == null)
			{
				throw new ArgumentException("Must provide input file list and output path");
			}
			if (InputPath.ToString().IsPresent()) args.Append($"-i {InputPath}");
			if (Title.IsPresent()) {
				args.Append("-t");
				args.AppendQuoted(Title);
			}
			if (Theme.ToString().IsPresent()) args.Append($"-b {Theme}");
			if (RewriteLinks) args.Append($"--rewrite-links");
			args.Append($"-o {OutputPath}");
			if (Debug)
			{
				Console.WriteLine(args.Render());
			}
		}

		public DocCreatorSettings WithTitle(string title)
		{
			Title = title;
			return this;
		}

		public DocCreatorSettings With(Theme theme)
		{
			Theme = theme;
			return this;
		}

		public DocCreatorSettings EnableLinkRewrite(bool enabled = true)
		{
			RewriteLinks = enabled;
			return this;
		}

		public DocCreatorSettings OutputToPath(DirectoryPath directory)
		{
			OutputPath = directory;
			if (!Context.FileSystem.GetDirectory(OutputPath).Exists) Context.FileSystem.GetDirectory(OutputPath).Create();
			return this;
		}

		public DocCreatorSettings EnableDebug(bool enabled = true)
		{
			Debug = enabled;
			return this;
		}
	}
}
