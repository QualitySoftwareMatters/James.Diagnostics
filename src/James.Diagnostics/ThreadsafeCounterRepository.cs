using System;

using Magnum.PerformanceCounters;

namespace James.Diagnostics
{
	public class ThreadSafeCounterRepository
	{
		private readonly CounterRepository _repository;
		private static readonly object SyncRoot = new Object();

		public ThreadSafeCounterRepository(CounterRepository repository)
		{
			_repository = repository;
		}

		public T GetCounter<T>(string instance) where T : class, CounterCategory, new()
		{
			lock (SyncRoot)
			{
				return _repository.GetCounter<T>(instance);
			}
		}
	}
}
