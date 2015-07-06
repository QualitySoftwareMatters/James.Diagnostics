using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class given_action_throws_when_monitoring : MonitoringTestsBase
	{
		private const string ExpectedMessage = "This is the exception message.";
		private long _failureValueBefore;
		private Exception _resultException;
		private long _successValueBefore;

		[TestFixtureSetUp]
		public void Before()
		{
			_failureValueBefore = GetValue(FailureCounter);
			_successValueBefore = GetValue(SuccessCounter);

			var expectedException = new ArgumentException(ExpectedMessage);
			Action action = () => { throw expectedException; };

			try
			{
				Monitoring<MonitoringTestsCounters>.Monitor(action);
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
		public void should_bubble_up_exception()
		{
			_resultException.Should().BeOfType<ArgumentException>();
			_resultException.Message.Should().Be(ExpectedMessage);
		}
	}
}