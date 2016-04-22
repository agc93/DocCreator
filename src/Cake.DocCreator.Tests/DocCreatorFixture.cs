using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Cake.Testing.Fixtures;

namespace Cake.DocCreator.Tests
{
	public class DocCreatorFixture : ToolFixture<DocCreatorSettings>
	{
		internal static string FilePath = "./file.md";
		public DocCreatorFixture() : base("DocCreator.exe")
		{
			var dp = new DirectoryPath("./testdata");
			var d = FileSystem.GetDirectory(dp);

			if (d.Exists)
				d.Delete(true);

			d.Create();
			DataDirectory = d;
		}

		internal FakeDirectory DataDirectory { get; }

		public Action<DocCreatorSettings> DocCreatorSettings { private get; set; }

		protected override void RunTool()
		{
			var tool = new DocCreator(FileSystem, Environment, ProcessRunner, Globber);
			tool.Generate(DocCreatorSettings);
		}

		public void CleanUp()
		{
			DataDirectory.Delete(true);
		}

		internal Func<DocCreatorSettings, DocCreatorSettings> AddDefaults = delegate(DocCreatorSettings s)
		{
			s.InputPath = new FilePath("./file.md");
			if (s.OutputPath == null) s.OutputToPath(new DirectoryPath("./testdata"));
			return s;
		};
	}
}
