using Magnum.PerformanceCounters;

namespace James.Diagnostics.Sample
{
	public class CustomerServiceCounters : CounterCategory, IMonitorableCounterCategory
	{
		public Counter Succeeded { get; set; }
		public Counter Failed { get; set; }
		public Counter ExecutionTime { get; set; }
	}
}