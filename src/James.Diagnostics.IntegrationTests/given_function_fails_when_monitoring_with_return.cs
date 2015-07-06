using System;

using FluentAssertions;

using James.Testing;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture(typeof(bool), true)]
	[TestFixture(typeof(bool), false)]
	[TestFixture(typeof(object), null)]
	[TestFixture(typeof(string), "Todd Meinershagen")]
	public class given_function_fails_when_monitoring_with_return<T> : MonitoringTestsBase
	{
		private readonly T _returnValue;
		private long _successValueBefore;
		private long _failureValueBefore;
		private Exception _exceptionResult;
		private const string ExpectedMessage = "This is for demo purposes.";

		public given_function_fails_when_monitoring_with_return(T returnValue)
		{
			_returnValue = returnValue;
		}

		[TestFixtureSetUp]
		public void Before()
		{
			_successValueBefore = GetValue(SuccessCounter);
			_failureValueBefore = GetValue(FailureCounter);

			Func<T> function = () =>
			{
				throw new ArgumentException(ExpectedMessage);
			};

			try
			{
				Monitoring<MonitoringTestsCounters>.Monitor(function);
				Assert.Fail("An exception should have been thrown.");
			}
			catch (Exception ex)
			{
				_exceptionResult = ex;
			}
		}

		[Test]
		public void should_not_increment_succeeded_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore);
		}

		[Test]
		public void should_increment_failed_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore + 1);
		}

		[Test]
		public void should_throw_exception()
		{
			_exceptionResult.Should().BeOfType<ArgumentException>();
			_exceptionResult.Message.Should().Be(ExpectedMessage);
		}

		private void GulpException<T>(Func<T> function)
		{
			Action action = () => function();
			action.GulpException();
		}
	}
}