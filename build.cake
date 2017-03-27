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
var objPaths = string.Format("./**/obj/{0}/{1}", configuration, dotnetVersion);
var binPaths = string.Format("./**/bin/{0}/{1}", configuration, dotnetVersion);

Task("Default")
	.IsDependentOn("Package");

Task("Clean")
	.Does(() =>
{
	CleanDirectories(objPaths);
	CleanDirectories(binPaths);
	
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
	
	// Make sure the .dlls in the obj path are not found elsewhere
	CleanDirectories(objPaths);
});

Task("GitLink")
	.IsDependentOn("Build")
	.Does(() =>
{
	GitLink("./", new GitLinkSettings {
		Configuration	= configuration,
		PdbDirectoryPath = string.Format("./{0}/bin/{1}/{2}", projectName, configuration, dotnetVersion)
	});
});

Task("Coverage")
	.IsDependentOn("Build")
	.Does(() =>
{
	// Make XUnit 2 run via the OpenCover process
	OpenCover(
		// The test tool Lamdba
		tool => {
			tool.XUnit2("./**/" + projectName + ".Tests.dll",
				new XUnit2Settings {
					ShadowCopy = false
				});
			},
		// The output path
		new FilePath("./coverage.xml"),
		// Settings
		new OpenCoverSettings() {
			ReturnTargetCodeOffset = 0
		}.WithFilter("+[" + projectName + "]*").WithFilter("-[" + projectName + ".Tests]*")
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

Task("Package")
//	.IsDependentOn("GitLink")
//	.IsDependentOn("Upload-Coverage-Report")
	.Does(()=>
{
	var nuGetPackSettings   = new NuGetPackSettings {
		BasePath                = projectName,
		OutputDirectory         = "./packages"
	};
								 
    NuGetPack("Dapplo.HttpExtensions/project.json", nuGetPackSettings);
});

RunTarget(target);