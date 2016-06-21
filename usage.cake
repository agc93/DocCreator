#addin "Cake.DocCreator"

Task("GenerateDocs")
  .Does(() => {
    DocCreator("./doc").Generate(settings => settings
  		.OutputToPath("./dist/html-docs")
  		.WithTitle("Cake.DocCreator")
  		.With(Theme.Simplex)
  		.EnableLinkRewrite()
  		.EnableOfflineMode());
  });

RunTarget("GenerateDocs");
