//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

namespace Dapplo.HttpExtensions.Extensions
{
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
}