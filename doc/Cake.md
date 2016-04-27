# Cake Addin Usage

> This is for invoking DocCreator from a Cake build script. For the CLI, see the [CommandLine documentation](CommandLine.md) instead.

## Loading the addin
Simply add the following at the top of your build script to import the addin from NuGet:

```csharp
#addin Cake.DocCreator
```
## Write your task

See below for a complete example:

```csharp
Task("GenerateDocs")
.Does(() => {
	var outputPath = "./artifacts/docs";
	var docsDir = Directory("./doc/");
    DocCreator(docsDir).Generate(settings => settings
		.OutputToPath(outputPath)
		.WithTitle("Cake.DocCreator")
		.With(Theme.Simplex)
		.EnableLinkRewrite());
});

RunTarget("GenerateDocs");
```
The `CreateDirectory` should be obvious enough. Let's step through the rest:

First call `DocCreator()` passing in the list of input files. Much like the [CLI](CommandLine.md), this accepts a single file, or a directory path containing any number of `.md` files.

Now call `Generate()` passing in the directory you want your files output to.

You can optionally pass an `Action<DocCreatorSettings>` as a second parameter to change the title and theme, or to enable link rewriting.

### Simplified example

The simplest possible invocation is basically:

```csharp
Task("GenerateDocs")
.Does(() => {
	DocCreator("./doc/*.md").Generate("./artifacts/docs");
});
```
This will use the defaults where possible, and convert any `.md` files in the `./doc/` folder into html in the `./artifacts/docs` folder as-is.
