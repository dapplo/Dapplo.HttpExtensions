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

#if NET461 || NETSTANDARD2_0

#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.Listener
{
    /// <summary>
    ///     int[] extensions, which in this case is an array of ports
    /// </summary>
    public static class ListenerPortExtensions
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Create an Localhost Uri for an unused port
        /// </summary>
        /// <param name="possiblePorts">An int array with ports, the routine will return the first free port.</param>
        /// <returns>Uri</returns>
        public static Uri CreateLocalHostUri(this int[] possiblePorts)
        {
            return new Uri($"http://localhost:{possiblePorts.GetFreeListenerPort()}");
        }

        /// <summary>
        ///     Returns an unused port.
        ///     A port of 0 in the list will have the following behaviour: https://msdn.microsoft.com/en-us/library/c6z86e63.aspx
        ///     If you do not care which local port is used, you can specify 0 for the port number. In this case, the service
        ///     provider will assign an available port number between 1024 and 5000.
        /// </summary>
        /// <param name="possiblePorts">An int array with ports, the routine will return the first free port.</param>
        /// <returns>A free port</returns>
        public static int GetFreeListenerPort(this int[] possiblePorts)
        {
            possiblePorts = possiblePorts ?? new[] {0};

            foreach (var portToCheck in possiblePorts)
            {
                var listener = new TcpListener(IPAddress.Loopback, portToCheck);
                try
                {
                    listener.Start();
                    // As the LocalEndpoint is of type EndPoint, this doesn't have the port, we need to cast it to IPEndPoint
                    var port = ((IPEndPoint) listener.LocalEndpoint).Port;
                    Log.Debug().WriteLine("Found free listener port {0}.", port);
                    return port;
                }
                catch
                {
                    Log.Debug().WriteLine("Port {0} isn't free.", portToCheck);
                }
                finally
                {
                    listener.Stop();
                }
            }
            var message = $"No free ports in the range {possiblePorts} found!";
            Log.Debug().WriteLine(message);
            throw new ApplicationException(message);
        }
    }
}

#endif