using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.DocCreator
{
	internal static class DocCreatorExtensions
	{
		internal static void AppendFiles(this ProcessArgumentBuilder args, IEnumerable<FilePath> files)
		{
			args.Append($"-i {files.First().GetDirectory()}");
		}

		internal static bool IsPresent(this string arg)
		{
			return !string.IsNullOrWhiteSpace(arg);
		}
	}
}