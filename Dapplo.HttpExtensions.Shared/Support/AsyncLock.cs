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
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// A simple class to make it possible to lock a resource while waiting
	/// </summary>
	public class AsyncLock : IDisposable
	{
		private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

		public async Task<IDisposable> LockAsync()
		{
			await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
			return new Releaser(_semaphoreSlim);
		}

		internal struct Releaser : IDisposable
		{
			private readonly SemaphoreSlim _semaphoreSlim;

			public Releaser(SemaphoreSlim semaphoreSlim)
			{
				_semaphoreSlim = semaphoreSlim;
			}
			public void Dispose()
			{
				_semaphoreSlim.Release();
			}
		}

		#region IDisposable Support
		// To detect redundant calls
		private bool _disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					_semaphoreSlim.Dispose();
				}

				_disposedValue = true;
			}
		}

		~AsyncLock()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
