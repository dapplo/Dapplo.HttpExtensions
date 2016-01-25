/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// The ProgressStreamContent makes it possible to track the progress of a http-post for a stream
	/// </summary>
	public class ProgressStreamContent : HttpContent
	{
		public enum UploadStates { PendingUpload, Uploading, PendingResponse }

		private readonly Stream _content;
		private readonly int _bufferSize;
		private bool _contentConsumed;

		/// <summary>
		/// Specify the IProgress, which will be informed of progress
		/// </summary>
		public IProgress<float> ProgressHandler { get; set; }

		/// <summary>
		/// the current upload state
		/// </summary>
		public UploadStates State
		{
			get;
			private set;
		} = UploadStates.PendingUpload;

		/// <summary>
		/// See the amount of bytes that were uploaded
		/// </summary>
		public long UploadedBytes
		{
			get;
			private set;
		}

		/// <summary>
		/// Create a StreamContent which supports progress reporting
		/// </summary>
		/// <param name="content">The stream for uploading</param>
		/// <param name="progressHandler">IProgress with float</param>
		/// <param name="bufferSize">Size of the upload buffer, 4Kb is default</param>
		public ProgressStreamContent(Stream content, IProgress<float> progressHandler = null, int bufferSize = 4 * 1024)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bufferSize));
			}
			_content = content;
			_bufferSize = bufferSize;
			ProgressHandler = progressHandler;
		}

		/// <summary>
		/// This actually writes (serializes) the content-stream to the remote
		/// </summary>
		/// <param name="stream">Stream to write to</param>
		/// <param name="context"></param>
		/// <returns>Task</returns>
		protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			if (_contentConsumed)
			{
				// If the content needs to be written to a target stream a 2nd time, then the stream must support
				// seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
				// stream (e.g. a NetworkStream).
				if (_content.CanSeek)
				{
					_content.Position = 0;
				}
				else
				{
					throw new InvalidOperationException("SR.net_http_content_stream_already_read");
				}
			}

			_contentConsumed = true;

			var buffer = new byte[_bufferSize];
			var size = _content.Length;
			var uploaded = 0;

			while (true)
			{
				var length = await _content.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
				if (length <= 0)
				{
					break;
				}

				UploadedBytes = uploaded += length;

				// Report the progress
				ProgressHandler?.Report(UploadedBytes * 100 / (float)size);

				await stream.WriteAsync(buffer, 0, length).ConfigureAwait(false);

				State = UploadStates.Uploading;
			}
			State = UploadStates.PendingResponse;
		}

		/// <summary>
		/// Return the length of the stream
		/// </summary>
		/// <param name="length"></param>
		/// <returns>true if it worked, in this case always</returns>
		protected override bool TryComputeLength(out long length)
		{
			length = _content.Length;
			return true;
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_content.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
