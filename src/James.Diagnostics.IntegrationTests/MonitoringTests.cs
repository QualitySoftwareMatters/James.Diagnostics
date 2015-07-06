using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using James.Testing;

using Magnum.Extensions;

using NSubstitute;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class MonitoringTests : MonitoringTestsBase
	{
		[Test]
		public void given_boolean_function_returns_true_when_monitoring_should_increment_requests_succeeded_counter()
		{
			var valueBefore = GetValue(SuccessCounter);

			Func<bool> function = () => true;
			Monitoring<MonitoringTestsCounters>.Monitor(function);

			GetValue(SuccessCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_action_does_not_throw_when_monitoring_should_increment_requests_succeeded_counter()
		{
			var valueBefore = GetValue(SuccessCounter);

			Action action = () => Debug.Print("This is a test.");
			Monitoring<MonitoringTestsCounters>.Monitor(action);

			GetValue(SuccessCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_returns_successfully_when_monitoring_a_second_counter_should_increment_requests_succeeded_counter()
		{
			const string categoryName = "AnotherMonitoringTestsCounters";

			var valueBefore = GetValue(SuccessCounter, categoryName);

			Func<bool> function = () => true;
			Monitoring<AnotherMonitoringTestsCounters>.Monitor(function);

			GetValue(SuccessCounter, categoryName).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_returns_successfully_when_monitoring_should_update_requests_response_time_counter()
		{
			Func<bool> function = () => true;

			var stopwatch = Substitute.For<IStopwatch>();
			const int expectedMilliseconds = 1504;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);
			stopwatch.Elapsed.Returns(span);

			Monitoring<MonitoringTestsCounters>.Monitor(function, stopwatch);

			GetValue(ExecutionTimeCounter).Should().Be(expectedMilliseconds);
		}

		[Test]
		public void given_action_returns_successfully_when_monitoring_should_update_requests_response_time_counter()
		{
			Action action = () => Debug.Print("This is a test.");

			var stopwatch = Substitute.For<IStopwatch>();
			const int expectedMilliseconds = 25000;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);
			stopwatch.Elapsed.Returns(span);

			Monitoring<MonitoringTestsCounters>.Monitor(action, stopwatch);

			GetValue(ExecutionTimeCounter).Should().Be(expectedMilliseconds);
		}

		[Test]
		public void given_function_returns_unsuccessfully_when_monitoring_should_increment_requests_failed_counter()
		{
			var valueBefore = GetValue(FailureCounter);

			Func<bool> function = () => false;
			Monitoring<MonitoringTestsCounters>.Monitor(function);

			GetValue(FailureCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_returns_unsuccessfully_when_monitoring_should_not_update_requests_response_time_counter()
		{
			var valueBefore = GetValue(ExecutionTimeCounter);

			Func<bool> function = () => false;
			Monitoring<MonitoringTestsCounters>.Monitor(function);

			GetValue(ExecutionTimeCounter).Should().Be(valueBefore);
		}

		[Test]
		public void given_function_throws_exception_when_monitoring_should_increment_requests_failed_counter()
		{
			var valueBefore = GetValue(FailureCounter);

			Func<bool> function = () => { throw new Exception(); };
			Action action = () => Monitoring<MonitoringTestsCounters>.Monitor(function);
			action.GulpException();

			GetValue(FailureCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_throws_exception_when_monitoring_should_not_update_requests_response_time_counter()
		{
			var valueBefore = GetValue(ExecutionTimeCounter);

			Func<bool> function = () => { throw new Exception(); };
			Action action = () => Monitoring<MonitoringTestsCounters>.Monitor(function);
			action.GulpException();

			GetValue(ExecutionTimeCounter).Should().Be(valueBefore);
		}

		[Test]
		public void given_action_throws_exception_when_monitoring_should_increment_requests_failed_counter()
		{
			var valueBefore = GetValue(FailureCounter);

			Action action = () => { throw new Exception(); };
			Action outerAction = () => Monitoring<MonitoringTestsCounters>.Monitor(action);
			outerAction.GulpException();

			GetValue(FailureCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_throws_exception_when_monitoring_should_bubble_up_exception()
		{
			const string message = "This is the exception message.";
			var expectedException = new ArgumentException(message);
			Func<bool> function = () => { throw expectedException; };
			Action action = () => Monitoring<MonitoringTestsCounters>.Monitor(function);

			action.ShouldThrow<ArgumentException>().WithMessage(message);
		}

		[Test]
		public void given_action_throws_exception_when_monitoring_should_bubble_up_exception()
		{
			const string message = "This is the exception message.";
			var expectedException = new ArgumentException(message);
			Action action = () => { throw expectedException; };
			Action outerAction = () => Monitoring<MonitoringTestsCounters>.Monitor(action);

			outerAction.ShouldThrow<ArgumentException>().WithMessage(message);
		}

		[Test]
		public void given_timespan_when_success_should_update_requests_response_time_counter()
		{
			const int expectedMilliseconds = 1010;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);

			Monitoring<MonitoringTestsCounters>.Success(span);

			GetValue(ExecutionTimeCounter).Should().Be(expectedMilliseconds);
		}

		[Test]
		public void given_timespan_when_success_should_increment_success_counter()
		{
			var valueBefore = GetValue(SuccessCounter);

			const int expectedMilliseconds = 15;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);
			Monitoring<MonitoringTestsCounters>.Success(span);

			GetValue(SuccessCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_timespan_when_failure_should_not_update_requests_response_time_counter()
		{
			var valueBefore = GetValue(ExecutionTimeCounter);

			const int expectedMilliseconds = 15;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);

			Monitoring<MonitoringTestsCounters>.Failure(span);

			GetValue(ExecutionTimeCounter).Should().Be(valueBefore);
		}

		[Test]
		public void given_timespan_when_failure_should_increment_failure_counter()
		{
			var valueBefore = GetValue(FailureCounter);

			const int expectedMilliseconds = 15;
			var span = new TimeSpan(0, 0, 0, 0, expectedMilliseconds);
			Monitoring<MonitoringTestsCounters>.Failure(span);

			GetValue(FailureCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_action_and_multiple_threads_when_monitoring_should_not_throw_dictionary_exception()
		{
			Action action = () =>
			{
				const int maxThreads = 10;
				var taskArray = new Task[maxThreads];

				var generator = new Random();

				for (var taskId = 0; taskId < maxThreads; taskId++)
				{
					int millisecondsTimeout = generator.Next(0, 1000);
					taskArray[taskId] =
						Task.Factory.StartNew(
							() =>
							Monitoring<MonitoringTestsCounters>.Monitor(() => Thread.Sleep(millisecondsTimeout)));
				}

				Task.WaitAll(taskArray);
			};

			action.ShouldNotThrow<ArgumentException>();
		}
	}
}
