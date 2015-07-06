using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_boolean_function_returns_false_when_monitoring : MonitoringTestsBase
	{
		private long _executionValueBefore;
		private long _failureValueBefore;
		private long _successValueBefore;

		[TestFixtureSetUp]
		public void Before()
		{
			_executionValueBefore = GetValue(ExecutionTimeCounter);
			_failureValueBefore = GetValue(FailureCounter);
			_successValueBefore = GetValue(SuccessCounter);

			Func<bool> function = () => false;
			Monitoring<MonitoringTestsCounters>.Monitor(function);
		}

		[Test]
		public void should_increment_failure_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore + 1);
		}

		[Test]
		public void should_not_increment_success_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore + 1);
		}

		[Test]
		public void should_not_update_execution_time_counter()
		{
			GetValue(ExecutionTimeCounter).Should().Be(_executionValueBefore);
		}
	}
}