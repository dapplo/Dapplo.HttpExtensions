#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OpenCover"
#tool coveralls.io
#addin MagicChunks
#addin Cake.FileHelpers
#addin Cake.Coveralls

var target = Argument("target", "Build");
var solutionName = Argument("solutionName", "Dapplo.HttpExtensions");
var configuration = Argument("configuration", "release");
var dotnetVersion = Argument("dotnetVersion", "net45");
var version = Argument("version", EnvironmentVariable("APPVEYOR_BUILD_VERSION")?? "0.0.9.9");
var pullRequest = Argument("pullRequest", EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
var coveralsRepoToken = Argument("coveralsRepoToken", EnvironmentVariable("COVERALLS_REPO_TOKEN"));
var isRelease = Argument<bool>("isRelease", string.Compare("[release]", EnvironmentVariable("appveyor_repo_commit_message_extended"), true) == 0);
var nugetApiKey = Argument("nugetApiKey", EnvironmentVariable("NuGet_Api_Key"));

Task("Default")
	.IsDependentOn("Package");

Task("Clean")
	.Does(() =>
{
	CleanDirectories("./**/obj");
	CleanDirectories("./**/bin");	
});

Task("Versioning")
	.Does(() =>
{
	var projects = GetFiles(string.Format("./{0}*/project.json", solutionName));

	foreach(var project in projects)
	{
		Information("Fixing version in {0} to {1}", project.FullPath, version);
		TransformConfig(project.FullPath, 
			new TransformationCollection {
				{ "Version", version }
			});
	}

});

Task("Restore-NuGet-Packages")
	.Does(() =>
{
	DotNetCoreRestore();
});

Task("Build")
	.IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.IsDependentOn("Versioning")
	.Does(() =>
{
	var settings = new DotNetCoreBuildSettings
    {
		Configuration = configuration,
	};
	 
	var projects = GetFiles(string.Format("./{0}*/project.json", solutionName));
	foreach(var project in projects)
	{
		DotNetCoreBuild(project.FullPath, settings);
	}
	
	// Make sure the .dlls in the obj path are not found elsewhere
	CleanDirectories("./**/obj");
});

Task("Coverage")
	.IsDependentOn("Build")
	.Does(() =>
{
	CreateDirectory("artifacts");

	// Make XUnit 2 run via the OpenCover process
	OpenCover(
		// The test tool Lamdba
		tool => {
			tool.XUnit2("./**/" + solutionName + ".Tests.dll",
				new XUnit2Settings {
					ShadowCopy = false
				});
			},
		// The output path
		new FilePath("./artifacts/coverage.xml"),
		// Settings
		new OpenCoverSettings() {
			ReturnTargetCodeOffset = 0
		}.WithFilter("+[" + solutionName + "]*").WithFilter("-[" + solutionName + ".Tests]*")
	);
});

Task("Upload-Coverage-Report")
	.IsDependentOn("Coverage")
	.WithCriteria(() => !string.IsNullOrEmpty(coveralsRepoToken))
    .Does(() =>
{
    CoverallsIo("./artifacts/coverage.xml", new CoverallsIoSettings()
    {
        RepoToken = coveralsRepoToken
    });
});

Task("Package")
	.IsDependentOn("Upload-Coverage-Report")
	.WithCriteria(() => !string.IsNullOrEmpty(pullRequest))
	.WithCriteria(() => isRelease)
	.Does(()=>
{
	var settings = new DotNetCorePackSettings
    {
		Configuration = configuration,
		OutputDirectory = "./artifacts/"
	};
	var projects = GetFiles(string.Format("./{0}*/project.json", solutionName));
	foreach(var project in projects.Where(p => !p.FullPath.Contains("Test")))
	{
		DotNetCorePack(project.FullPath, settings);
	}
});

RunTarget(target);