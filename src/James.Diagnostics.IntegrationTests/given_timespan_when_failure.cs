using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_timespan_when_failure : MonitoringTestsBase
	{
		private long _executionValueBefore;
		private long _successValueBefore;
		private long _failureValueBefore;

		[TestFixtureSetUp]
		public void Before()
		{
			_successValueBefore = GetValue(SuccessCounter);
			_failureValueBefore = GetValue(FailureCounter);
			_executionValueBefore = GetValue(ExecutionTimeCounter);

			const int expectedMilliseconds = 15;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);

			Monitoring<MonitoringTestsCounters>.Failure(span);

		}

		[Test]
		public void should_not_update_requests_response_time_counter()
		{

			GetValue(ExecutionTimeCounter).Should().Be(_executionValueBefore);
		}

		[Test]
		public void given_timespan_when_failure_should_increment_failure_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore + 1);
		}

		[Test]
		public void given_timespan_when_failure_should_not_increment_success_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore);
		}
	}
}