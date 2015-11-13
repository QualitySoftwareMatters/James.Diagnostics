using Magnum.PerformanceCounters;

namespace James.Diagnostics
{
    public abstract class MonitorableCounterCategory : CounterCategory, IMonitorableCounterCategory
    {
        public Counter Succeeded { get; set; }
        public Counter Failed { get; set; }
        public Counter ExecutionTime { get; set; }
    }
}