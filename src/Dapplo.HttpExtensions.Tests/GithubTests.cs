// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.HttpExtensions.JsonSimple;
using Dapplo.HttpExtensions.SystemTextJson;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Summary description for GithubTests
/// </summary>
public class GithubTests
{
    private readonly Uri _releasesUri;

    public GithubTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        //SystemTextJsonSerializer.RegisterGlobally();
        //SimpleJsonSerializer.RegisterGlobally();

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var githubApiUri = new Uri("https://api.github.com");
        _releasesUri = githubApiUri.AppendSegments("repos", "dapplo", "Dapplo.HttpExtensions", "releases");

        // This is needed when running in AppVeyor, as AppVeyor has multiple request to GitHub, the rate-limit is exceeded.
        var username = Environment.GetEnvironmentVariable("github_username");
        var password = Environment.GetEnvironmentVariable("github_token");
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            _releasesUri = _releasesUri.SetCredentials(username, password);
        }
    }

    /// <summary>
    ///     To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetAsJsonAsync_GitHubApiReleases_SimpleJsonSerializer()
    {
        var behaviour = HttpBehaviour.Current as IChangeableHttpBehaviour;
        Assert.NotNull(behaviour);
        behaviour.JsonSerializer = new SimpleJsonSerializer();

        var releases = await _releasesUri.GetAsAsync<HttpResponse<List<GitHubRelease>, GitHubError>>();
        Assert.NotNull(releases);
        Assert.False(releases.HasError, $"{releases.StatusCode}: {releases.ErrorResponse?.Message} {releases.ErrorResponse?.DocumentationUrl}");

        var latestRelease = releases.Response
            .Where(x => !x.Prerelease)
            .OrderByDescending(x => x.PublishedAt)
            .FirstOrDefault();
        Assert.NotNull(latestRelease);
        Assert.NotEmpty(latestRelease.HtmlUrl);
    }

    /// <summary>
    ///     To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetAsJsonAsync_GitHubApiReleases_SystemTextJsonSerializer()
    {
        var behaviour = HttpBehaviour.Current as IChangeableHttpBehaviour;
        Assert.NotNull(behaviour);
        behaviour.JsonSerializer = new SystemTextJsonSerializer();

        var releases = await _releasesUri.GetAsAsync<HttpResponse<List<GitHubRelease>, GitHubError>>();
        Assert.NotNull(releases);
        Assert.False(releases.HasError, $"{releases.StatusCode}: {releases.ErrorResponse?.Message} {releases.ErrorResponse?.DocumentationUrl}");

        var latestRelease = releases.Response
            .Where(x => !x.Prerelease)
            .OrderByDescending(x => x.PublishedAt)
            .FirstOrDefault();
        Assert.NotNull(latestRelease);
        Assert.NotEmpty(latestRelease.HtmlUrl);
    }

    /// <summary>
    ///     To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetAsJsonAsync_GitHubApiReleases_JsonNetJsonSerializer()
    {
        var behaviour = HttpBehaviour.Current as IChangeableHttpBehaviour;
        Assert.NotNull(behaviour);
        behaviour.JsonSerializer = new JsonNetJsonSerializer();

        var releases = await _releasesUri.GetAsAsync<HttpResponse<List<GitHubRelease>, GitHubError>>();
        Assert.NotNull(releases);
        Assert.False(releases.HasError, $"{releases.StatusCode}: {releases.ErrorResponse?.Message} {releases.ErrorResponse?.DocumentationUrl}");

        var latestRelease = releases.Response
            .Where(x => !x.Prerelease)
            .OrderByDescending(x => x.PublishedAt)
            .FirstOrDefault();
        Assert.NotNull(latestRelease);
        Assert.NotEmpty(latestRelease.HtmlUrl);
    }
}