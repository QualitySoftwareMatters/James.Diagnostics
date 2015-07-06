using System;

namespace James.Diagnostics
{
	internal interface IStopwatch
	{
		void Start();
		void Stop();
		TimeSpan Elapsed { get; }
	}
}
