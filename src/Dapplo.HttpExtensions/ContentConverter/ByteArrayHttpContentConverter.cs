// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.ContentConverter;

/// <summary>
///     This can convert HttpContent from/to a byte[]
/// </summary>
public class ByteArrayHttpContentConverter : IHttpContentConverter
{
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'


    /// <summary>
    ///     Instance of this IHttpContentConverter for reusing
    /// </summary>
    public static Lazy<ByteArrayHttpContentConverter> Instance { get; } = new(() => new ByteArrayHttpContentConverter());

    /// <summary>
    ///     Order or priority of the IHttpContentConverter
    /// </summary>
    public int Order => 0;

    /// <summary>
    ///     Check if we can convert from the HttpContent to a byte array
    /// </summary>
    /// <param name="typeToConvertTo">To what type will the result be assigned</param>
    /// <param name="httpContent">HttpContent</param>
    /// <returns>true if we can convert the HttpContent to a ByteArray</returns>
    public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
    {
        return typeToConvertTo == typeof(byte[]);
    }

    /// <inheritdoc />
    public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
    {
        if (!CanConvertFromHttpContent(resultType, httpContent))
        {
            throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
        }
        Log.Debug().WriteLine("Retrieving the content as byte[], Content-Type: {0}", httpContent.Headers.ContentType);
#if NET461 || NETCOREAPP3_1 || NETSTANDARD1_3 || NETSTANDARD2_0
        return await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
#else
        return await httpContent.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc />
    public bool CanConvertToHttpContent(Type typeToConvert, object content)
    {
        return typeToConvert == typeof(byte[]);
    }

    /// <inheritdoc />
    public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
    {
        var byteArray = content as byte[];
        return new ByteArrayContent(byteArray);
    }

    /// <inheritdoc />
    public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
    {
        if (resultType is null)
        {
            throw new ArgumentNullException(nameof(resultType));
        }
        if (httpRequestMessage is null)
        {
            throw new ArgumentNullException(nameof(httpRequestMessage));
        }
    }
}