using Magnum.PerformanceCounters;

namespace James.Diagnostics
{
	public interface IMonitorableCounterCategory
	{
		Counter Succeeded { get; set; }
		Counter Failed { get; set; }
		Counter ExecutionTime { get; set; }
	}
}
