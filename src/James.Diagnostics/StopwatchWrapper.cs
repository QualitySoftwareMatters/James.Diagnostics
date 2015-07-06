using System;
using System.Diagnostics;

namespace James.Diagnostics
{
	internal class StopwatchWrapper : IStopwatch
	{
		private readonly Stopwatch _stopwatch;

		public StopwatchWrapper()
			: this(new Stopwatch())
		{}

		public StopwatchWrapper(Stopwatch stopwatch)
		{
			_stopwatch = stopwatch;
		}

		public void Start()
		{
			_stopwatch.Start();
		}

		public void Stop()
		{
			_stopwatch.Stop();
		}

		public TimeSpan Elapsed
		{
			get { return _stopwatch.Elapsed; }
		}
	}
}
