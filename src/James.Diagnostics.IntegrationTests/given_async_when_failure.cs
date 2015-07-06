using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_async_when_failure : MonitoringTestsBase
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

			Monitoring<MonitoringTestsCounters>.Failure();
		}

		[Test]
		public void should_not_update_execution_time_counter()
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