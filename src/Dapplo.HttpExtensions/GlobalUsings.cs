// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

global using System;
global using System.Collections.Generic;
global using System.Net;
global using System.Net.Http;
global using System.Net.Security;
global using System.Text;
global using Dapplo.Log;
global using System.Threading;
global using System.Threading.Tasks;

#if !NETSTANDARD1_3
global using System.Net.Cache;
global using System.Security.Principal;
#endif