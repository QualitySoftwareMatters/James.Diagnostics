using System;

using Magnum.PerformanceCounters;

namespace James.Diagnostics
{
	public static class Monitoring<TCounter>
		where TCounter : class, IMonitorableCounterCategory, CounterCategory, new()
	{
		private static readonly ThreadSafeCounterRepository _repository;

		static Monitoring()
		{
			_repository = new ThreadSafeCounterRepository(CounterRepositoryConfigurator.New(cfg => cfg.Register<TCounter>()));
		}

		public static void Monitor(Func<bool> function)
		{
			Monitor(function, null);
		}

		internal static void Monitor(Func<bool> function, IStopwatch stopwatch)
		{
			stopwatch = stopwatch ?? new StopwatchWrapper();
			var counter = _repository.GetCounter<TCounter>("_default");

			try
			{
				stopwatch.Start();

				if (function())
				{
					counter.Succeeded.Increment();
					stopwatch.Stop();
					counter.ExecutionTime.Set(stopwatch.Elapsed.TotalMilliseconds.AsLong());
				}
				else
				{
					counter.Failed.Increment();
				}
			}
			catch (Exception)
			{
				counter.Failed.Increment();
				throw;
			}
			finally
			{

			}
		}

		public static void Monitor(Action action)
		{
			Monitor(action, null);
		}

		internal static void Monitor(Action action, IStopwatch stopwatch)
		{
			Func<bool> function = () =>
			{
				action();
				return true;
			};

			Monitor(function, stopwatch);
		}

		public static void Success(TimeSpan elapsed)
		{
			var counter = _repository.GetCounter<TCounter>("_default");
			counter.Succeeded.Increment();
			counter.ExecutionTime.Set(elapsed.TotalMilliseconds.AsLong());
		}

		public static void Failure(TimeSpan elapsed)
		{
			var counter = _repository.GetCounter<TCounter>("_default");
			counter.Failed.Increment();
		}
	}
}
