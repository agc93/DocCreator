using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core.IO;
using Shouldly;
using Xunit;

namespace Cake.DocCreator.Tests
{
	
	public class DocCreatorAddinTests : IDisposable
	{
		public DocCreatorAddinTests()
		{
			Fixture = new DocCreatorFixture();
		}

		private DocCreatorFixture Fixture { get; set; }

		[Fact]
		public void FailsWhenNoInputFilesPresent()
		{
			Assert.Throws<ArgumentException>(() => Fixture.Run());
		}

		[Fact]
		public void OutputPathDefaultsWhenNotSetTest()
		{
			Fixture.DocCreatorSettings = s => s.InputPath = new FilePath(DocCreatorFixture.FilePath);
			var result = Fixture.Run();
			result.Args.ShouldContain("-o");
			result.Args.ShouldContain("html-docs");
		}

		[Fact]
		public void UsesDefaultThemeWhenNotSet()
		{
			Fixture.DocCreatorSettings = s => s.InputPath = new FilePath(DocCreatorFixture.FilePath);
			var result = Fixture.Run();
			result.Args.ShouldContain("-b Bootstrap"); // this is the same behaviour as the CLI, but enforced by C#
		}

		[Fact]
		public void ShouldSetThemeWhenSet()
		{
			Fixture.DocCreatorSettings = s =>
			{
				s.InputPath = new FilePath(DocCreatorFixture.FilePath);
				s.With(Theme.Amelia);
			};
			var result = Fixture.Run();
			result.Args.ShouldContain("-b Amelia");
		}

		[Fact]
		public void ShouldIncludeTitleWhenSet()
		{
			Fixture.DocCreatorSettings = s =>
			{
				s.InputPath = new FilePath(DocCreatorFixture.FilePath);
				s.With(Theme.Amelia).WithTitle("Cake.DocCreator");
			};
			var result = Fixture.Run();
			result.Args.ShouldContain("-t \"Cake.DocCreator\"");
		}

		[Fact]
		public void ShouldEnableRewriteWhenSet()
		{
			Fixture.DocCreatorSettings = s =>
			{
				s.InputPath = new FilePath(DocCreatorFixture.FilePath);
				s.EnableLinkRewrite();
			};
			var result = Fixture.Run();
			result.Args.ShouldContain("--rewrite-links");
		}

		
		public void ShouldCreateOutputFile()
		{
			File.WriteAllText(DocCreatorFixture.FilePath, "# Title\n\r\n\r## Heading\n\r\n\rSome **text**.");
			
			Fixture.DocCreatorSettings = s =>
			{
				s.InputPath = new FilePath(DocCreatorFixture.FilePath);
				s.OutputToPath(Fixture.DataDirectory.Path);
			};
			var result = Fixture.Run();
			result.Args.ShouldContain($"-o {Fixture.DataDirectory.Path}");
			Assert.True(Fixture.DataDirectory.Exists);
			Assert.True(Fixture.DataDirectory.GetFiles("*.html", SearchScope.Current).Any());
		}

		private Action<DocCreatorSettings> MinimalSettings => s => s.InputPath = new FilePath(DocCreatorFixture.FilePath);

		public void Dispose()
		{
			Fixture.CleanUp();
		}
	}
}
