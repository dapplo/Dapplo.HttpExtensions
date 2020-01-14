// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET461 || NETSTANDARD2_0 || NETCOREAPP3_0

using System;
using System.Net;
using System.Net.Sockets;
using Dapplo.Log;

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
            possiblePorts ??= new[] {0};

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