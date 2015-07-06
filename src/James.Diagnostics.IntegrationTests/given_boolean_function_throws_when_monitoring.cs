using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_boolean_function_throws_when_monitoring : MonitoringTestsBase
	{
		private const string ExpectedMessage = "This is the exception message.";
		private long _failureValueBefore;
		private long _executionValueBefore;
		private long _successValueBefore;
		private Exception _resultException;

		[TestFixtureSetUp]
		public void Before()
		{
			_executionValueBefore = GetValue(ExecutionTimeCounter);
			_failureValueBefore = GetValue(FailureCounter);
			_successValueBefore = GetValue(SuccessCounter);

			var expectedException = new ArgumentException(ExpectedMessage);
			Func<bool> function = () => { throw expectedException; };

			try
			{
				Monitoring<MonitoringTestsCounters>.Monitor(function);
				Assert.Fail("An exception should have been thrown.");
			}
			catch (Exception ex)
			{
				_resultException = ex;
			}
		}

		[Test]
		public void should_increment_failure_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore + 1);
		}

		[Test]
		public void should_not_increment_success_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore);
		}

		[Test]
		public void should_not_update_requests_response_time_counter()
		{
			GetValue(ExecutionTimeCounter).Should().Be(_executionValueBefore);
		}

		[Test]
		public void given_function_throws_exception_when_monitoring_should_bubble_up_exception()
		{
			_resultException.Should().BeOfType<ArgumentException>();
			_resultException.Message.Should().Be(ExpectedMessage);
		}
	}
}