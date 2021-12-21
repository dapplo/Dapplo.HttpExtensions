// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     Contains the pertinent data for a ProgressStream Report event.
/// </summary>
public class ProgressStreamReport
{
    /// <summary>
    ///     Default constructor for ProgressStreamReport.
    /// </summary>
    public ProgressStreamReport()
    {
    }

    /// <summary>
    ///     Creates a new ProgressStreamReport initializing its members.
    /// </summary>
    /// <param name="bytesMoved">The number of bytes that were read/written to/from the stream.</param>
    /// <param name="streamLength">The total length of the stream in bytes.</param>
    /// <param name="streamPosition">The current position in the stream.</param>
    /// <param name="wasRead">True if the bytes were read from the stream, false if they were written.</param>
    public ProgressStreamReport(int bytesMoved, long streamLength, long streamPosition, bool wasRead) : this()
    {
        BytesMoved = bytesMoved;
        StreamLength = streamLength;
        StreamPosition = streamPosition;
        WasRead = wasRead;
    }

    /// <summary>
    ///     The number of bytes that were read/written to/from the stream.
    /// </summary>
    public int BytesMoved { get; private set; }

    /// <summary>
    ///     The total length of the stream in bytes, 0 if the stream isn't seekable
    /// </summary>
    public long StreamLength { get; private set; }

    /// <summary>
    ///     The current position in the stream, 0 if the stream isn't seekable
    /// </summary>
    public long StreamPosition { get; private set; }

    /// <summary>
    ///     True if the bytes were read from the stream, false if they were written.
    /// </summary>
    public bool WasRead { get; }
}