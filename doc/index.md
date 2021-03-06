# Dapplo.HttpExtensions

- Current build status: [![Build status](https://ci.appveyor.com/api/projects/status/y4n7u63336vhuy46?svg=true)](https://ci.appveyor.com/project/dapplo/dapplo-httpextensions)
- Coverage Status: [![Coverage Status](https://coveralls.io/repos/github/dapplo/Dapplo.HttpExtensions/badge.svg?branch=master)](https://coveralls.io/github/dapplo/Dapplo.HttpExtensions?branch=master)
- NuGet package: [![NuGet package](https://img.shields.io/nuget/v/Dapplo.HttpExtensions.svg)](https://www.nuget.org/packages/Dapplo.HttpExtensions)

Sometimes you just want to access a service in the internet or on a local server.
This project helps you to deal with a lot of difficult stuff, like having a default proxy or even making a specify proxy possible.
Also it can handle Json communication, which a lot of REST based APIs use.

A short example:
```
	using Dapplo.HttpExtensions;

	var uri = new Uri("http://myserver/");
	var response = await uri.GetAsAsync<string>();
```

A more "complex" example of how you can use this, is available in the test. 

There is a test with calling a JSON Service (GitHub): [UriActionExtensionsTests.TestGetAsJsonAsync_GitHubApiReleases](https://github.com/dapplo/Dapplo.HttpExtensions/blob/master/Dapplo.HttpExtensions.Test/UriActionExtensionsTests.cs).

There is also an OAuth test: [OAuth/OAuthTests.TestOAuthHttpMessageHandler](https://github.com/dapplo/Dapplo.HttpExtensions/blob/master/Dapplo.HttpExtensions.Test/OAuth/OAuthTests.cs).
This is not running during the build, as it needs "human" interaction with a browser.

Some of the features:

1. Fluent API
2. Progress support, for uploading and downloading
3. Typed access to Http content (e.g. GetAsAsync<Bitmap> )
4. Typed upload
5. Json support, in two variants:
  1. [SimpleJson](https://github.com/facebook-csharp-sdk/simple-json) via nuget package Dapplo.HttpExtensions.JsonSimple and call SimpleJsonSerializer.RegisterGlobally() or set IHttpBehavior.JsonSerializer to new SimpleJsonSerializer();
  2. install nuget package Dapplo.HttpExtensions.JsonNet and call JsonNetJsonSerializer.RegisterGlobally(); or set IHttpBehavior.JsonSerializer to new JsonNetJsonSerializer();
6. OAuth 1 & 2 via nuget package Dapplo.HttpExtensions.OAuth, this is currently work in process but with some servers it should already be usable.

Notes:
This project uses async code, and tries to conform to the Task-bases Asynchronous Pattern (TAP). Just so you know why sometimes the method name look odd... Meaning all async methods have names which end with Async and (where possible) accept a CancellationToken. This is the final parameter, as adviced here: https://blogs.msdn.microsoft.com/andrewarnottms/2014/03/19/recommended-patterns-for-cancellationtoken/

API Changes, with every 0.x change the signatures have changed:
In 0.2.x a HttpBehaviour object was added to prevent future signature changes as much as possible.
In 0.3.x a lot of previous method were combined. (GetAsMemoryStream -> GetAsAsync<MemoryStream>, GetAsStringAsync -> GetAsAsync<string>)
In 0.4.x the IHttpBehaviour was removed again, it's now passed via the CallContext. If you need to a specific behaviour for a call, than you will need to call MakeCurrent() on the behaviour before the call.
In 0.5.x The configuration logic for the converters was changed
In 0.6.x the Json and OAuth support were removed from the main package
