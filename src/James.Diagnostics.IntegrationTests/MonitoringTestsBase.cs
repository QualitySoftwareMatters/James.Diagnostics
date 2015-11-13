using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace James.Diagnostics.IntegrationTests
{
	public abstract class MonitoringTestsBase
	{
		protected const string ExecutionTimeCounter = "ExecutionTime";
		protected const string SuccessCounter = "Succeeded";
		protected const string FailureCounter = "Failed";
		private static readonly Dictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>(); 

		protected long GetValue(string counterName, string categoryName = "MonitoringTestsCounters")
		{
			const string machineName = ".";
			const string instanceName = Monitoring<MonitoringTestsCounters>.DefaultInstance;

			if (!CounterExists(counterName, categoryName, instanceName, machineName)) return 0;

			var key = String.Format("{0}-{1}-{2}-{3}", categoryName, counterName, instanceName, machineName);

			if (!Counters.ContainsKey(key))
			{
				var counter = new PerformanceCounter(categoryName, counterName, instanceName, machineName);
				Counters.Add(key, counter);
			}
			
			return Counters[key].RawValue;
		}

		private bool CounterExists(string counterName, string categoryName, string instanceName, string machineName)
		{
			return PerformanceCounterCategory.Exists(categoryName)
			       && PerformanceCounterCategory.InstanceExists(instanceName, categoryName, machineName)
			       && PerformanceCounterCategory.CounterExists(counterName, categoryName, machineName);
		}

		public class MonitoringTestsCounters : MonitorableCounterCategory
		{}

		public class AnotherMonitoringTestsCounters : MonitorableCounterCategory
		{}
	}
}