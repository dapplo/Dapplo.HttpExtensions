// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Extensions;

/// <summary>
///     Extensions for HttpBehaviour
/// </summary>
public static class HttpBehaviourExtensions
{
    /// <summary>
    ///     Get the IHttpRequestConfiguration from the IHttpBehaviour
    /// </summary>
    /// <typeparam name="THttpRequestConfiguration">Type which implements IHttpRequestConfiguration</typeparam>
    /// <param name="httpBehaviour">IHttpBehaviour instance, if null HttpBehaviour.Current is used</param>
    /// <returns>THttpReqestConfiguration</returns>
    public static THttpRequestConfiguration GetConfig<THttpRequestConfiguration>(this IHttpBehaviour httpBehaviour)
        where THttpRequestConfiguration : class, IHttpRequestConfiguration, new()
    {
        // Although this probably doesn't happen... if null is passed HttpBehaviour.Current is used
        if (httpBehaviour is null)
        {
            httpBehaviour = HttpBehaviour.Current;
        }

        httpBehaviour.RequestConfigurations.TryGetValue(typeof(THttpRequestConfiguration).Name, out var configurationUntyped);
        return configurationUntyped as THttpRequestConfiguration ?? new THttpRequestConfiguration();
    }

    /// <summary>
    ///     Set the specified configuration, if it already exists it is overwritten
    /// </summary>
    /// <param name="httpBehaviour">IHttpBehaviour</param>
    /// <param name="configuration">THttpReqestConfiguration</param>
    /// <returns>IHttpBehaviour</returns>
    public static IHttpBehaviour SetConfig(this IHttpBehaviour httpBehaviour, IHttpRequestConfiguration configuration)
    {
        // Although this probably doesn't happen... if null is passed HttpBehaviour.Current is used
        if (httpBehaviour is null)
        {
            httpBehaviour = HttpBehaviour.Current;
        }
        httpBehaviour.RequestConfigurations[configuration.Name] = configuration;
        return httpBehaviour;
    }
}