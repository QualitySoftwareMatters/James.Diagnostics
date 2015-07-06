using System;

using Magnum.PerformanceCounters;

namespace James.Diagnostics
{
	public static class Monitoring<TCounter>
		where TCounter : class, IMonitorableCounterCategory, CounterCategory, new()
	{
		private static readonly ThreadSafeCounterRepository _repository;
		public const string DefaultInstance = "_default";

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
			var counter = _repository.GetCounter<TCounter>(DefaultInstance);

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
		}

		public static T Monitor<T>(Func<T> function)
		{
			return Monitor(function, null);
		}

		internal static T Monitor<T>(Func<T> function, IStopwatch stopwatch)
		{
			T result = default(T);
			Action action = () => result = function();
			Monitor(action);
			return result;
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
			var counter = _repository.GetCounter<TCounter>(DefaultInstance);
			counter.Succeeded.Increment();
			counter.ExecutionTime.Set(elapsed.TotalMilliseconds.AsLong());
		}

		public static void Failure(TimeSpan elapsed)
		{
			var counter = _repository.GetCounter<TCounter>(DefaultInstance);
			counter.Failed.Increment();
		}
	}
}
