using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.DocCreator
{
	public class DocCreator : Tool<DocCreatorSettings>
	{
		public DocCreator(Path file, ICakeContext ctx ) : this(ctx.FileSystem, ctx.Environment, ctx.ProcessRunner, ctx.Globber)
		{
			File = file;
			Context = ctx;
		}

		public DocCreator(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IGlobber globber) : base(fileSystem, environment, processRunner, globber)
		{
		}

		private ICakeContext Context { get; set; }

		private Path File { get; set; }

		protected override string GetToolName() => "DocCreator Runner";

		protected override IEnumerable<string> GetToolExecutableNames()
		{
			yield return "DocCreator.exe";
		}

		public DirectoryPath Generate(Action<DocCreatorSettings> configure)
		{
			return RunTool(new DirectoryPath("./html-docs"), configure);
		}

		public DirectoryPath Generate(DirectoryPath outputDir)
		{
			return RunTool(outputDir, null);
		}

		public DirectoryPath Generate(DirectoryPath outputDir, Action<DocCreatorSettings> configure)
		{
			return RunTool(outputDir, configure);
		}

		private DirectoryPath RunTool(DirectoryPath outputDir, Action<DocCreatorSettings> configure)
		{
			var settings = new DocCreatorSettings(Context, File, outputDir);
			configure?.Invoke(settings);
			if (!Context.FileSystem.Exist(settings.OutputPath)) Context.FileSystem.GetDirectory(settings.OutputPath).Create();
			var args = GetDocCreatorArguments(settings);
			Run(settings, args);
			return settings.OutputPath;
		}

		private ProcessArgumentBuilder GetDocCreatorArguments(DocCreatorSettings settings)
		{
			var args = new ProcessArgumentBuilder();
			settings.Build(args);
			return args;
		}
	}
}
