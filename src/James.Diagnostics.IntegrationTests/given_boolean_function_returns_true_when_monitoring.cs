using System;

using FluentAssertions;

using Magnum.PerformanceCounters;

using NSubstitute;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture(typeof(MonitoringTestsCounters))]
	[TestFixture(typeof(AnotherMonitoringTestsCounters))]
	public class given_boolean_function_returns_true_when_monitoring<TCounters> : MonitoringTestsBase
		where TCounters : class, IMonitorableCounterCategory, CounterCategory, new()
	{
		private long _successValueBefore;
		private long _failureValueBefore;
		private string _categoryName;
		private const int ExpectedMilliseconds = 1504;

		[TestFixtureSetUp]
		public void Before()
		{
			_categoryName = typeof (TCounters).Name;
			_successValueBefore = GetValue(SuccessCounter, _categoryName);
			_failureValueBefore = GetValue(FailureCounter, _categoryName);

			Func<bool> function = () => true;

			var stopwatch = Substitute.For<IStopwatch>();
			var span = new TimeSpan(0, 0, 0, 0, ExpectedMilliseconds);
			stopwatch.Elapsed.Returns(span);

			Monitoring<TCounters>.Monitor(function, stopwatch);
		}

		[Test]
		public void should_increment_requests_succeeded_counter()
		{
			GetValue(SuccessCounter, _categoryName).Should().Be(_successValueBefore + 1);
		}

		[Test]
		public void should_not_increment_requests_failed_counter()
		{
			GetValue(FailureCounter, _categoryName).Should().Be(_failureValueBefore);
		}

		[Test]
		public void should_update_requests_response_time_counter()
		{
			GetValue(ExecutionTimeCounter, _categoryName).Should().Be(ExpectedMilliseconds);
		}
	}
}