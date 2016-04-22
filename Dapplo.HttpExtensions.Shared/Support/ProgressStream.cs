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
using System.IO;

#endregion

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	///     Wraps another stream and provides reporting for when bytes are read or written to the stream.
	/// </summary>
	public class ProgressStream : Stream
	{
		#region Private Data Members

		private readonly Stream innerStream;

		#endregion

		#region Constructor

		/// <summary>
		///     Creates a new ProgressStream supplying the stream for it to report on.
		/// </summary>
		/// <param name="streamToReportOn">The underlying stream that will be reported on when bytes are read or written.</param>
		public ProgressStream(Stream streamToReportOn)
		{
			if (streamToReportOn != null)
			{
				innerStream = streamToReportOn;
			}
			else
			{
				throw new ArgumentNullException(nameof(streamToReportOn));
			}
		}

		#endregion

		#region Events

		/// <summary>
		///     Raised when bytes are read from the stream.
		/// </summary>
		public event ProgressStreamReportDelegate BytesRead;

		/// <summary>
		///     Raised when bytes are written to the stream.
		/// </summary>
		public event ProgressStreamReportDelegate BytesWritten;

		/// <summary>
		///     Raised when bytes are either read or written to the stream.
		/// </summary>
		public event ProgressStreamReportDelegate BytesMoved;

		/// <summary>
		///     Called when bytes are read.
		/// </summary>
		/// <param name="bytesMoved">int with the number of bytes</param>
		protected virtual void OnBytesRead(int bytesMoved)
		{
			if (bytesMoved != 0 && BytesRead != null)
			{
				ProgressStreamReportEventArgs args;
				if (innerStream.CanSeek)
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, innerStream.Length, innerStream.Position, true);
				}
				else
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, 0, 0, true);
				}
				BytesRead(this, args);
			}
		}

		/// <summary>
		///     Called when bytes are written
		/// </summary>
		/// <param name="bytesMoved">int with the number of bytes</param>
		protected virtual void OnBytesWritten(int bytesMoved)
		{
			if (bytesMoved != 0 && BytesWritten != null)
			{
				ProgressStreamReportEventArgs args;
				if (innerStream.CanSeek)
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, innerStream.Length, innerStream.Position, false);
				}
				else
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, 0, 0, false);
				}
				BytesWritten(this, args);
			}
		}

		/// <summary>
		///     Called when bytes are moved
		/// </summary>
		/// <param name="bytesMoved">int with the number of bytes which are moved</param>
		/// <param name="isRead">true if the bytes were read, false if written</param>
		protected virtual void OnBytesMoved(int bytesMoved, bool isRead)
		{
			if (bytesMoved != 0 && BytesMoved != null)
			{
				ProgressStreamReportEventArgs args;
				if (innerStream.CanSeek)
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, innerStream.Length, innerStream.Position, isRead);
				}
				else
				{
					args = new ProgressStreamReportEventArgs(bytesMoved, 0, 0, isRead);
				}
				BytesMoved(this, args);
			}
		}

		#endregion

		#region Stream Members

		/// <inheritdoc />
		public override bool CanRead
		{
			get { return innerStream.CanRead; }
		}

		/// <inheritdoc />
		public override bool CanSeek
		{
			get { return innerStream.CanSeek; }
		}

		/// <inheritdoc />
		public override bool CanWrite
		{
			get { return innerStream.CanWrite; }
		}

		/// <inheritdoc />
		public override void Flush()
		{
			innerStream.Flush();
		}

		/// <inheritdoc />
		public override long Length
		{
			get { return innerStream.Length; }
		}

		/// <inheritdoc />
		public override long Position
		{
			get { return innerStream.Position; }
			set { innerStream.Position = value; }
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			var bytesRead = innerStream.Read(buffer, offset, count);

			OnBytesRead(bytesRead);
			OnBytesMoved(bytesRead, true);

			return bytesRead;
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			return innerStream.Seek(offset, origin);
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			innerStream.SetLength(value);
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			innerStream.Write(buffer, offset, count);

			OnBytesWritten(count);
			OnBytesMoved(count, false);
		}

#if !_PCL_
	/// <inheritdoc />
		public override void Close()
		{
			innerStream.Close();
			base.Close();
		}
#endif

		#endregion
	}

	/// <summary>
	///     Contains the pertinent data for a ProgressStream Report event.
	/// </summary>
	public class ProgressStreamReportEventArgs : EventArgs
	{
		/// <summary>
		///     Default constructor for ProgressStreamReportEventArgs.
		/// </summary>
		public ProgressStreamReportEventArgs()
		{
		}

		/// <summary>
		///     Creates a new ProgressStreamReportEventArgs initializing its members.
		/// </summary>
		/// <param name="bytesMoved">The number of bytes that were read/written to/from the stream.</param>
		/// <param name="streamLength">The total length of the stream in bytes.</param>
		/// <param name="streamPosition">The current position in the stream.</param>
		/// <param name="wasRead">True if the bytes were read from the stream, false if they were written.</param>
		public ProgressStreamReportEventArgs(int bytesMoved, long streamLength, long streamPosition, bool wasRead) : this()
		{
			BytesMoved = bytesMoved;
			StreamLength = streamLength;
			StreamPosition = streamPosition;
			WasRead = WasRead;
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

	/// <summary>
	///     The delegate for handling a ProgressStream Report event.
	/// </summary>
	/// <param name="sender">The object that raised the event, should be a ProgressStream.</param>
	/// <param name="args">The arguments raised with the event.</param>
	public delegate void ProgressStreamReportDelegate(object sender, ProgressStreamReportEventArgs args);
}