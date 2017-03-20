#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=gitlink"
#tool coveralls.io
#addin "MagicChunks"
#addin "nuget:?package=Cake.FileHelpers"
#addin Cake.Coveralls

var target = Argument("target", "Build");
var projectName = Argument("projectName", "Dapplo.HttpExtensions");
var configuration = Argument("configuration", "Release");
var dotnetVersion = Argument("dotnetVersion", "net45");
var solution = File("./" + projectName + ".sln");

Task("Default")
	.IsDependentOn("Upload-Coverage-Report");

Task("Clean")
	.Does(() =>
{
	CleanDirectories(string.Format("./**/obj/{0}", configuration));
	CleanDirectories(string.Format("./**/bin/{0}", configuration));
	
	// A nasty leftover
	CleanDirectories("./tools/OpenCover/SampleSln");
});

Task("Versioning")
	.Does(() =>
{
	var version = GitVersion();
		var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
		Information("Fixing version in {0} to {1}", project, version.AssemblySemVer.ToString());
		TransformConfig(project.ToString(), 
			new TransformationCollection {
				{ "Project/PropertyGroup/Version", version.AssemblySemVer.ToString() }
			});
	}

});

Task("Restore-NuGet-Packages")
	.Does(() =>
{
	NuGetRestore(solution);
});

Task("Build")
	.IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.IsDependentOn("Versioning")
	.Does(() =>
{
	MSBuild(solution, new MSBuildSettings {
		Verbosity = Verbosity.Minimal,
		ToolVersion = MSBuildToolVersion.VS2017,
		Configuration = configuration,
		PlatformTarget = PlatformTarget.MSIL
	});
	GitLink("./");
});

Task("Coverage")
	.IsDependentOn("Build")
	.Does(() =>
{
	OpenCover(tool => {
	  tool.XUnit2("./**/" + projectName + ".Tests.dll",
		new XUnit2Settings {
		  ShadowCopy = false,

		});
	  },
	  new FilePath("./coverage.xml"),
	  new OpenCoverSettings()
		.WithFilter("+[" + projectName + "]*")
		.WithFilter("-[" + projectName + ".Tests]*")
	);
});

Task("Upload-Coverage-Report")
	.IsDependentOn("Coverage")
    .Does(() =>
{
    CoverallsIo("./coverage.xml", new CoverallsIoSettings()
    {
        RepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN")
    });
});

RunTarget(target);