//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

namespace Dapplo.HttpExtensions.Support
{
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
}