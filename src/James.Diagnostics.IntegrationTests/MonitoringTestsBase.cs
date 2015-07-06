using System;
using System.Diagnostics;
using System.Threading;

using Magnum.Extensions;
using Magnum.PerformanceCounters;

namespace James.Diagnostics.IntegrationTests
{
	public abstract class MonitoringTestsBase
	{
		protected const string ExecutionTimeCounter = "ExecutionTime";
		protected const string SuccessCounter = "Succeeded";
		protected const string FailureCounter = "Failed";

		protected long GetValue(string counterName, string categoryName = "MonitoringTestsCounters")
		{
			const string machineName = ".";
			const string instanceName = Monitoring<MonitoringTestsCounters>.DefaultInstance;
			long returnValue = 0;

			if (!CounterExists(counterName, categoryName, instanceName, machineName)) return returnValue;

			using (var counter = new PerformanceCounter(categoryName, counterName, instanceName, machineName))
			{
				returnValue = counter.RawValue;
			}

			return returnValue;
		}

		private bool CounterExists(string counterName, string categoryName, string instanceName, string machineName)
		{
			return PerformanceCounterCategory.Exists(categoryName)
			       && PerformanceCounterCategory.InstanceExists(instanceName, categoryName, machineName)
			       && PerformanceCounterCategory.CounterExists(counterName, categoryName, machineName);
		}

		public class MonitoringTestsCounters : CounterCategory, IMonitorableCounterCategory
		{
			public Counter Succeeded { get; set; }
			public Counter Failed { get; set; }
			public Counter ExecutionTime { get; set; }
		}

		public class AnotherMonitoringTestsCounters : CounterCategory, IMonitorableCounterCategory
		{
			public Counter Succeeded { get; set; }
			public Counter Failed { get; set; }
			public Counter ExecutionTime { get; set; }
		}
	}
}