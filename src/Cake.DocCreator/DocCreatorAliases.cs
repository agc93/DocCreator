using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.DocCreator
{
	[CakeAliasCategory("Create Docs")]
	public static class DocCreatorAliases
	{
		[CakeMethodAlias]
		public static DocCreator DocCreator(this ICakeContext ctx, Path docsDir)
		{
			return new DocCreator(docsDir, ctx);
		}
	}

	
}
