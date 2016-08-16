//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log.Facade;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Extensions for the HttpRequestMessage class
	/// </summary>
	public static class HttpRequestMessageExtensions
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Add default request header without validation
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="name">Header name</param>
		/// <param name="value">Header value</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		public static HttpRequestMessage AddRequestHeader(this HttpRequestMessage httpRequestMessage, string name, string value)
		{
			httpRequestMessage.Headers.TryAddWithoutValidation(name, value);
			return httpRequestMessage;
		}

		/// <summary>
		///     Send the supplied HttpRequestMessage, and get a response back
		/// </summary>
		/// <typeparam name="TResponse">The Type to read into</typeparam>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResponse> SendAsync<TResponse>(this HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default(CancellationToken)) where TResponse : class
		{
			using (var httpClient = HttpClientFactory.Create(httpRequestMessage.RequestUri))
			{
				return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Send the supplied HttpRequestMessage, and get a response back
		/// </summary>
		/// <typeparam name="TResponse">The Type to read into</typeparam>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResponse> SendAsync<TResponse>(this HttpRequestMessage httpRequestMessage, HttpClient httpClient, CancellationToken cancellationToken = default(CancellationToken))
			where TResponse : class
		{
			var httpBehaviour = HttpBehaviour.Current;
			Log.Verbose().WriteLine("Sending {0} HttpRequestMessage with Uri: {1}", httpRequestMessage.Method, httpRequestMessage.RequestUri);
			using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, cancellationToken).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResponse>(cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Send the supplied HttpRequestMessage, ignoring the response
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="cancellationToken">CancellationToken</param>
		public static async Task SendAsync(this HttpRequestMessage httpRequestMessage, HttpClient httpClient, CancellationToken cancellationToken = default(CancellationToken))
		{
			var httpBehaviour = HttpBehaviour.Current;
			Log.Verbose().WriteLine("Sending {0} HttpRequestMessage with Uri: {1}", httpRequestMessage.Method, httpRequestMessage.RequestUri);

			using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
			{
				if (httpBehaviour.ThrowOnError)
				{
					httpResponseMessage.EnsureSuccessStatusCode();
				}
			}
		}

		/// <summary>
		///     Simplest way to set the authorization header
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="scheme">The authorization scheme, e.g. Bearer or Basic</param>
		/// <param name="parameter">the value to the scheme</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		public static HttpRequestMessage SetAuthorization(this HttpRequestMessage httpRequestMessage, string scheme, string parameter)
		{
			var authenticationHeaderValue = new AuthenticationHeaderValue(scheme, parameter);
			httpRequestMessage.Headers.Authorization = authenticationHeaderValue;
			return httpRequestMessage;
		}

		/// <summary>
		///     Set Basic Authentication for the HttpRequestMessage
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="user">username</param>
		/// <param name="password">password</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		public static HttpRequestMessage SetBasicAuthorization(this HttpRequestMessage httpRequestMessage, string user, string password)
		{
			var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
			return httpRequestMessage.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		///     Use the UserInfo from the Uri to set the basic authorization information
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="uri">Uri with UserInfo</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		public static HttpRequestMessage SetBasicAuthorization(this HttpRequestMessage httpRequestMessage, Uri uri)
		{
			if (string.IsNullOrEmpty(uri?.UserInfo))
			{
				return httpRequestMessage;
			}
			var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(uri.UserInfo));
			return httpRequestMessage.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		///     Set Bearer "Authentication" for the HttpRequestMessage
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="bearer">Bearer for the authorization</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		public static HttpRequestMessage SetBearer(this HttpRequestMessage httpRequestMessage, string bearer)
		{
			return httpRequestMessage.SetAuthorization("Bearer", bearer);
		}
	}
}