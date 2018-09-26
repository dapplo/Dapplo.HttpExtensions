//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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

#region Usings

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

#endregion

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     Extensions to help with Authorization
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// This is the basic encoding for username and password
        /// </summary>
        /// <param name="user">string</param>
        /// <param name="password">string</param>
        /// <returns>string with Base64 encoded username and password</returns>
        private static string EncodeBasic(string user, string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
        }

        /// <summary>
        ///     Set Authorization for the current client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="scheme">scheme</param>
        /// <param name="authorization">value</param>
        /// <returns>HttpClient for fluent usage</returns>
        public static HttpClient SetAuthorization(this HttpClient client, string scheme, string authorization)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, authorization);
            return client;
        }

        /// <summary>
        ///     Set Basic Authentication for the current client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="user">username</param>
        /// <param name="password">password</param>
        /// <returns>HttpClient for fluent usage</returns>
        public static HttpClient SetBasicAuthorization(this HttpClient client, string user, string password)
        {
            return client.SetBasicAuthorization(EncodeBasic(user, password));
        }

        /// <summary>
        ///     Use the UserInfo from the Uri to set the basic authorization information
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="uri">Uri with UserInfo</param>
        /// <returns>HttpClient for fluent usage</returns>
        public static HttpClient SetBasicAuthorization(this HttpClient client, Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (string.IsNullOrEmpty(uri.UserInfo))
            {
                return client;
            }

            var userInfo = Uri.UnescapeDataString(uri.UserInfo);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userInfo));
            return client.SetBasicAuthorization(credentials);
        }

        /// <summary>
        ///     Set Basic "Authentication" for the current client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="credentials">Credentials for the authorization</param>
        /// <returns>HttpClient for fluent usage</returns>
        public static HttpClient SetBasicAuthorization(this HttpClient client, string credentials)
        {
            return client.SetAuthorization("Basic", credentials);
        }

        /// <summary>
        ///     Set Bearer "Authentication" for the current client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="bearer">Bearer for the authorization</param>
        /// <returns>HttpClient for fluent usage</returns>
        public static HttpClient SetBearerAuthorization(this HttpClient client, string bearer)
        {
            return client.SetAuthorization("Bearer", bearer);
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
        /// <param name="credentials">string</param>
        /// <returns>HttpRequestMessage for fluent usage</returns>
        public static HttpRequestMessage SetBasicAuthorization(this HttpRequestMessage httpRequestMessage, string credentials)
        {
            return httpRequestMessage.SetAuthorization("Basic", credentials);
        }

        /// <summary>
        ///     Set Basic Authentication for the HttpRequestMessage
        /// </summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <param name="user">string username</param>
        /// <param name="password">string password</param>
        /// <returns>HttpRequestMessage for fluent usage</returns>
        public static HttpRequestMessage SetBasicAuthorization(this HttpRequestMessage httpRequestMessage, string user, string password)
        {
            return httpRequestMessage.SetBasicAuthorization(EncodeBasic(user, password));
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
            var userInfo = Uri.UnescapeDataString(uri.UserInfo);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userInfo));
            return httpRequestMessage.SetBasicAuthorization(credentials);
        }

        /// <summary>
        ///     Set Bearer "Authentication" for the HttpRequestMessage
        /// </summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <param name="bearer">Bearer for the authorization</param>
        /// <returns>HttpRequestMessage for fluent usage</returns>
        public static HttpRequestMessage SetBearerAuthorization(this HttpRequestMessage httpRequestMessage, string bearer)
        {
            return httpRequestMessage.SetAuthorization("Bearer", bearer);
        }

        /// <summary>
        ///     Sets the UserInfo of the Uri
        /// </summary>
        /// <param name="uri">Uri to extend</param>
        /// <param name="username">username of value</param>
        /// <param name="password">password for the user</param>
        /// <returns>Uri with extended query</returns>
        public static Uri SetCredentials(this Uri uri, string username, string password)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                UserName = username,
                Password = Uri.EscapeDataString(password)
            };
            return uriBuilder.Uri;
        }
    }
}