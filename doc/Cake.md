# Cake Addin Usage

> This is for invoking DocCreator from a Cake build script. For the CLI, see the [CommandLine documentation](CommandLine.md) instead.

## Loading the addin
Simply add the following at the top of your build script to import the addin from NuGet:

```csharp
#addin "Cake.DocCreator"
```

If you get "Could not load executable errors", try also adding `#tool "DocCreator"`

## Write your task

See below for a complete example:

```csharp
Task("GenerateDocs")
.Does(() => {
    DocCreator("./doc/").Generate(settings => settings
		.OutputToPath("./artifacts/docs")
		.WithTitle("Cake.DocCreator")
		.With(Theme.Simplex)
		.EnableLinkRewrite()
		.EnableOfflineMode());
});

RunTarget("GenerateDocs");
```
Let's step through the example:

First call `DocCreator()` passing in the list of input files. Much like the [CLI](CommandLine.md), this accepts a single file, or a directory path containing any number of `.md` files.

Now call `Generate()` passing in the directory you want your files output to.

You can optionally pass an `Action<DocCreatorSettings>` as a second parameter to change any configuration options including title and theme, or to enable link rewriting.

> You can also omit the output directory path and just pass in an `Action<DocCreatorSettings>`. The files will be output to the default path, or to any path by using the `OutputToPath()` setting.

### Simplified example

The simplest possible invocation is basically:

```csharp
Task("GenerateDocs")
.Does(() => {
	DocCreator("./doc/").Generate("./artifacts/docs");
});
```
This will use the defaults where possible, and convert any `.md` files in the `./doc/` folder into html in the `./artifacts/docs` folder as-is.
