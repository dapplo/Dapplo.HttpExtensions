﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     Wraps another stream and provides reporting for when bytes are read or written to the stream.
/// </summary>
public class ProgressStream : Stream
{
    private readonly Stream _innerStream;

    /// <summary>
    ///     Creates a new ProgressStream supplying the stream for it to report on.
    /// </summary>
    /// <param name="streamToReportOn">The underlying stream that will be reported on when bytes are read or written.</param>
    public ProgressStream(Stream streamToReportOn)
    {
        if (streamToReportOn != null)
        {
            _innerStream = streamToReportOn;
        }
        else
        {
            throw new ArgumentNullException(nameof(streamToReportOn));
        }
    }

    /// <summary>
    ///     Called when bytes are read from the stream.
    /// </summary>
    public Action<object, ProgressStreamReport> BytesRead { get; set; }

    /// <summary>
    ///     RaisedCalled when bytes are written to the stream.
    /// </summary>
    public Action<object, ProgressStreamReport> BytesWritten { get; set; }

    /// <summary>
    ///     RaisedCalled when bytes are either read or written to the stream.
    /// </summary>
    public Action<object, ProgressStreamReport> BytesMoved { get; set; }

    /// <summary>
    ///     Called when bytes are read.
    /// </summary>
    /// <param name="bytesMoved">int with the number of bytes</param>
    protected virtual void OnBytesRead(int bytesMoved)
    {
        if (bytesMoved == 0 || BytesRead is null)
        {
            return;
        }

        long length = 0;
        long position = 0;
        if (_innerStream.CanSeek)
        {
            length = _innerStream.Length;
            position = _innerStream.Position;
        }
        var args = new ProgressStreamReport(bytesMoved, length, position, true);
        BytesRead?.Invoke(this, args);
    }

    /// <summary>
    ///     Called when bytes are written
    /// </summary>
    /// <param name="bytesMoved">int with the number of bytes</param>
    protected virtual void OnBytesWritten(int bytesMoved)
    {
        if (bytesMoved == 0 || BytesWritten is null)
        {
            return;
        }

        long length = 0;
        long position = 0;
        if (_innerStream.CanSeek)
        {
            length = _innerStream.Length;
            position = _innerStream.Position;
        }
        var args = new ProgressStreamReport(bytesMoved, length, position, false);
        BytesWritten?.Invoke(this, args);
    }

    /// <summary>
    ///     Called when bytes are moved
    /// </summary>
    /// <param name="bytesMoved">int with the number of bytes which are moved</param>
    /// <param name="isRead">true if the bytes were read, false if written</param>
    protected virtual void OnBytesMoved(int bytesMoved, bool isRead)
    {
        if (bytesMoved == 0 || BytesMoved is null)
        {
            return;
        }

        long length = 0;
        long position = 0;
        if (_innerStream.CanSeek)
        {
            length = _innerStream.Length;
            position = _innerStream.Position;
        }
        var args = new ProgressStreamReport(bytesMoved, length, position, isRead);
        BytesMoved?.Invoke(this, args);
    }

    /// <inheritdoc />
    public override bool CanRead => _innerStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => _innerStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => _innerStream.CanWrite;

    /// <inheritdoc />
    public override void Flush()
    {
        _innerStream.Flush();
    }

    /// <inheritdoc />
    public override long Length => _innerStream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get { return _innerStream.Position; }
        set { _innerStream.Position = value; }
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = _innerStream.Read(buffer, offset, count);

        OnBytesRead(bytesRead);
        OnBytesMoved(bytesRead, true);

        return bytesRead;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);

        OnBytesWritten(count);
        OnBytesMoved(count, false);
    }

#if NETFRAMEWORK
        /// <inheritdoc />
        public override void Close()
        {
            _innerStream.Close();
            base.Close();
        }
#endif
}