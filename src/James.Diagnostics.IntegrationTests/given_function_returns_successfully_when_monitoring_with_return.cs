using System;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture(typeof(bool), true)]
	[TestFixture(typeof(bool), false)]
	[TestFixture(typeof(object), null)]
	[TestFixture(typeof(string), "Todd Meinershagen")]
	public class given_function_returns_successfully_when_monitoring_with_return<T> : MonitoringTestsBase
	{
		private readonly T _returnValue;
		private T _result;
		private long _successValueBefore;
		private long _failureValueBefore;

		public given_function_returns_successfully_when_monitoring_with_return(T returnValue)
		{
			_returnValue = returnValue;
		}

		[TestFixtureSetUp]
		public void Before()
		{
			_successValueBefore = GetValue(SuccessCounter);
			_failureValueBefore = GetValue(FailureCounter);

			Func<T> function = () => _returnValue;
			_result = Monitoring<MonitoringTestsCounters>.Monitor(function);
		}

		[Test]
		public void should_increment_succeeded_counter()
		{
			GetValue(SuccessCounter).Should().Be(_successValueBefore + 1);
		}

		[Test]
		public void should_not_increment_failed_counter()
		{
			GetValue(FailureCounter).Should().Be(_failureValueBefore);
		}

		[Test]
		public void should_return_value()
		{
			_result.Should().Be(_returnValue);
		}
	}
}