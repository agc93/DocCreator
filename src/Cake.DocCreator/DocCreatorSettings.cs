using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Path = Cake.Core.IO.Path;

namespace Cake.DocCreator
{
	public class DocCreatorSettings : ToolSettings
	{
		public DocCreatorSettings(Path input, DirectoryPath outputPath)
		{
			InputPath = input;
			OutputPath = outputPath;
		}

		public DocCreatorSettings()
		{
		}

		public IList<string> Arguments { get; set; } = new List<string>();

		public Path InputPath { get; set; }

		private bool RewriteLinks { get; set; }

		private Theme Theme { get; set; }

		private string Title { get; set; }

		public DirectoryPath OutputPath { get; private set; }

		public void Build(ProcessArgumentBuilder args)
		{
			if (OutputPath == null || InputPath == null)
			{
				throw new ArgumentException("Must provide input file list and output path");
			}
			if (InputPath.ToString().IsPresent()) args.Append($"-i {InputPath}");
			if (Title.IsPresent()) args.Append($"-t {Title}");
			if (Theme.ToString().IsPresent()) args.Append($"-b {Theme}");
			if (RewriteLinks) args.Append($"--rewrite-links");
			args.Append($"-o {OutputPath}");
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
			if (!Directory.Exists(OutputPath.FullPath)) Directory.CreateDirectory(OutputPath.FullPath);
			return this;
		}
	}
}