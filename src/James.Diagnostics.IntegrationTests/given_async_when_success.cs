using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_async_when_success : MonitoringTestsBase
	{
		private const int ExpectedMilliseconds = 1010;
		private long _successValueBefore;
		private long _failureValueBefore;

		[TestFixtureSetUp]
		public void Before()
		{
			_successValueBefore = GetValue(SuccessCounter);
			_failureValueBefore = GetValue(FailureCounter);

			var span = new TimeSpan(0, 0, 0, 0, ExpectedMilliseconds);
			Monitoring<MonitoringTestsCounters>.Success(span);
		}

		[Test]
		public void given_timespan_when_success_should_update_requests_response_time_counter()
		{
			GetValue(ExecutionTimeCounter).Should().Be(ExpectedMilliseconds);
		}

		[Test]
		public void given_timespan_when_success_should_increment_success_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore + 1);
		}

		[Test]
		public void given_timespan_when_success_should_not_increment_failure_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore);
		}
	}
}