#tool "xunit.runner.console"
#tool "GitVersion.CommandLine"
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/DocCreator.sln");
var solution = ParseSolution(solutionPath);
var projects = solution.Projects;
var projectPaths = projects.Select(p => p.Path.GetDirectory());
var testAssemblies = projects.Where(p => p.Name.Contains("Test")).Select(p => p.Path.GetDirectory() + "/bin/" + configuration + "/" + p.Name + ".dll");
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
GitVersion versionInfo = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
	versionInfo = GitVersion();
	Information("Building for version {0}", versionInfo.FullSemVer);
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in projectPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
    Information("Cleaning common files...");
    CleanDirectory("./dist/");
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    Information("Restoring solution...");
    NuGetRestore(solutionPath);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	Information("Building solution...");
	MSBuild(solutionPath, settings =>
		settings.SetPlatformTarget(PlatformTarget.MSIL)
			.WithProperty("TreatWarningsAsErrors","true")
			.SetVerbosity(Verbosity.Quiet)
			.WithTarget("Build")
			.SetConfiguration(configuration));
});

Task("Copy-Files")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(artifacts + "build");
	foreach (var project in projects) {
		CreateDirectory(artifacts + "build/" + project.Name);
		var files = GetFiles(project.Path.GetDirectory() +"/bin/" +configuration +"/" +project.Name +".*");
		CopyFiles(files, artifacts + "build/" + project.Name);
	}
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(testResultsPath);

    var settings = new XUnit2Settings {
        NoAppDomain = true,
        OutputDirectory = testResultsPath,
    };
    settings.ExcludeTrait("Category", "Integration");

    XUnit2(testAssemblies, settings);
});


Task("Repack")
	.IsDependentOn("Build")
	.Does(() => {
		Information("Merging DocCreator.exe...");
		CreateDirectory("./dist/tools");
		var assemblyList = GetFiles("./src/DocCreator/bin/" + configuration + "/**/*.dll");
		Information("Executing ILMerge to merge {0} assemblies", assemblyList.Count);
		ILRepack(
			"./dist/tools/DocCreator.exe",
			"./src/DocCreator/bin/" + configuration + "/DocCreator.exe",
			assemblyList);
	});

Task("NuGet")
    .IsDependentOn("Copy-Files")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Repack")
    .Does(() => {
		CreateDirectory("./dist/package/");
        Information("Building NuGet package");
        var nuspecFiles = GetFiles("./**/*.nuspec");
        NuGetPack(nuspecFiles, new NuGetPackSettings() {
			Version = versionInfo.NuGetVersionV2
			});
        MoveFiles("./**/*DocCreator.*.nupkg", "./dist/package/");
    });

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Copy-Files")
	.IsDependentOn("Run-Unit-Tests");
Task("Publish").IsDependentOn("NuGet");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
