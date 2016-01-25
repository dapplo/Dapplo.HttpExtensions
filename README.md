dapplo.httpextensions
=====================
Work in progress

Documentation can be found [here](http://www.dapplo.net/blocks/Dapplo.HttpExtensions.html) (later!!)
Current build status: [![Build status](https://ci.appveyor.com/api/projects/status/y4n7u63336vhuy46?svg=true)](https://ci.appveyor.com/project/dapplo/dapplo-httpextensions)

Sometimes you just want to access a service in the internet or on a local server.
This project helps you to deal with a lot of difficult stuff like having a default proxy or even making a specify proxy possible.
Also it can handle Json communication, which a lot of REST based APIs use.

A short example:
```
	using Dapplo.HttpExtensions;

	var uri = new Uri("http://myserver/");
	var response = await uri.GetAsAsync<string>();
```

A more "complex" example of how you can use this, is visible in the test, e.g. [UriJsonActionExtensionsTests.TestGetAsJsonAsync_GitHubApiReleases](https://github.com/dapplo/Dapplo.HttpExtensions/blob/master/Dapplo.HttpExtensions.Test/UriJsonActionExtensionsTests.cs).

Notes:

1. [This project](https://github.com/facebook-csharp-sdk/simple-json) is internally used for the Json parser, as this is abstracted via a IJsonSerializer you could e.g. change this to Json.NET.
2. This project uses async code, and tries to conform to the Task-bases Asynchronous Pattern (TAP). Just so you know why sometimes the method name look odd... Meaning all async methods have names which end with Async and (where possible) accept a CancellationToken. This is the final parameter, as adviced here: https://blogs.msdn.microsoft.com/andrewarnottms/2014/03/19/recommended-patterns-for-cancellationtoken/
3. With every 0.x change the signatures have changed, in 0.2.x a HttpBehaviour object was added to prevent future signature changes as much as possible. In 0.3 a lot of previous method were combined. (GetAsMemoryStream -> GetAsAsync<MemoryStream>, GetAsStringAsync -> GetAsAsync<string>)