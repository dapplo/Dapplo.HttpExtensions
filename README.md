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
	var response = await uri.GetAsStringAsync();
```

A more "complex" example of how you can use this, is visible in the [UriJsonActionExtensionsTests.TestGetAsJsonAsync_GitHubApiReleases](https://github.com/dapplo/Dapplo.HttpExtensions/blob/master/Dapplo.HttpExtensions.Test/UriJsonActionExtensionsTests.cs).

Notes:

1. [This project](https://github.com/facebook-csharp-sdk/simple-json) is internally used for the Json parser, I will have to look into a possibility to specify your own.
2. This project uses async code, and tries to conform to the Task-bases Asynchronous Pattern (TAP). Meaning all async methods have names which end with Async and (where possible) accept a CancellationToken. This is the final parameter, as adviced here: https://blogs.msdn.microsoft.com/andrewarnottms/2014/03/19/recommended-patterns-for-cancellationtoken/
3. With version 0.2 the signatures have changed, a HttpBehaviour object was added to prevent future signature changes as much as possible