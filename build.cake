#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=gitlink"
#tool coveralls.io
#addin "MagicChunks"
#addin "nuget:?package=Cake.FileHelpers"
#addin Cake.Coveralls

var target = Argument("target", "Build");
var projectName = Argument("projectName", "Dapplo.HttpExtensions");
var configuration = Argument("configuration", "release");
var dotnetVersion = Argument("dotnetVersion", "net45");
var version = Argument("version", EnvironmentVariable("APPVEYOR_BUILD_VERSION"));
var solution = File("./" + projectName + ".sln");

Task("Default")
	.IsDependentOn("Package");

Task("Clean")
	.Does(() =>
{
	CleanDirectories("./**/obj");
	CleanDirectories("./**/bin");
	
	// A nasty leftover
	CleanDirectories("./tools/OpenCover/SampleSln");
});

Task("Versioning")
	.Does(() =>
{
	var projects = GetFiles(string.Format("./{0}*/project.json", projectName));

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
	 
	var projects = GetFiles(string.Format("./{0}*/project.json", projectName));
	foreach(var project in projects)
	{
		DotNetCoreBuild(project.FullPath, settings);
	}
	
	// Make sure the .dlls in the obj path are not found elsewhere
	CleanDirectories("./**/obj");
});

Task("GitLink")
	.IsDependentOn("Build")
	.Does(() =>
{
	var projects = GetFiles(string.Format("./{0}*/project.json", projectName));
	foreach(var project in projects.Where(p => !p.FullPath.Contains("Test")))
	{
		GitLink("./", new GitLinkSettings {
			Configuration	= configuration,
			PdbDirectoryPath = string.Format("./{0}/bin/{1}/{2}", project.GetDirectory(), configuration, dotnetVersion)
		});
	}
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
	.IsDependentOn("Upload-Coverage-Report")
	.Does(()=>
{
	var settings = new DotNetCorePackSettings
    {
		Configuration = configuration,
	};
	var projects = GetFiles(string.Format("./{0}*/project.json", projectName));
	foreach(var project in projects.Where(p => !p.FullPath.Contains("Test")))
	{
		DotNetCorePack(project.FullPath, settings);
	}
});

RunTarget(target);