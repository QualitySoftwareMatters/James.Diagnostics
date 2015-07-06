using System;
using System.Diagnostics;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_action_does_not_throw_when_monitoring : MonitoringTestsBase
	{
		private long _successValueBefore;
		private long _failureValueBefore;
		private const int ExpectedMilliseconds = 25000;

		[TestFixtureSetUp]
		public void Before()
		{
			_successValueBefore = GetValue(SuccessCounter);
			_failureValueBefore = GetValue(FailureCounter);

			Action action = () => Debug.Print("This is a test.");

			var stopwatch = Substitute.For<IStopwatch>();
			var span = new TimeSpan(0, 0, 0, 0, ExpectedMilliseconds);
			stopwatch.Elapsed.Returns(span);

			Monitoring<MonitoringTestsCounters>.Monitor(action, stopwatch);
		}

		[Test]
		public void should_increment_success_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore + 1);
		}

		[Test]
		public void should_not_increment_failure_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore);
		}

		[Test]
		public void given_action_does_not_throw_when_monitoring_should_update_response_time_counter()
		{
			GetValue(ExecutionTimeCounter).Should().Be(ExpectedMilliseconds);
		}
	}
}