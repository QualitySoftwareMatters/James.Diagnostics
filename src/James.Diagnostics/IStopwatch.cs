using System;

namespace James.Diagnostics
{
	public interface IStopwatch
	{
		void Start();
		void Stop();
		TimeSpan Elapsed { get; }
	}
}
