// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth.CodeReceivers;

/// <summary>
///     Simply pass the Request token as the authentication
/// </summary>
internal class PassThroughCodeReceiver : IOAuthCodeReceiver
{
    /// <summary>
    ///     The OAuth code receiver implementation
    /// </summary>
    /// <param name="authorizeMode">which of the AuthorizeModes was used to call the method</param>
    /// <param name="codeReceiverSettings"></param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Dictionary with values</returns>
    public Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, string>();

        if (codeReceiverSettings is OAuth1Settings oauth1Settings)
        {
            result.Add(OAuth1Parameters.Token.EnumValueOf(), oauth1Settings.RequestToken);
        }
        return Task.FromResult<IDictionary<string, string>>(result);
    }
}