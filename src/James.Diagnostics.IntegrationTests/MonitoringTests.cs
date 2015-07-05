using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Magnum.PerformanceCounters;

using NSubstitute;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class MonitoringTests
	{
		private const string ExecutionTimeCounter = "ExecutionTime";
		private const string SuccessCounter = "Succeeded";
		private const string FailureCounter = "Failed";

		[Test]
		public void given_function_returns_successfully_when_monitoring_should_increment_requests_succeeded_counter()
		{
			var valueBefore = GetValue(SuccessCounter);

			Func<bool> function = () => true;
			Monitoring<MonitoringTestsCounters>.Monitor(function);

			GetValue(SuccessCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_action_does_not_throw_when_monitoring_should_increment_requests_succeeded_counter()
		{
			var counter = GetCounter(SuccessCounter);
			var valueBefore = counter == null ? 0 : counter.RawValue;

			Action action = () => Debug.Print("This is a test.");
			Monitoring<MonitoringTestsCounters>.Monitor(action);

			counter = counter ?? GetCounter(SuccessCounter);
			counter.RawValue.Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_returns_successfully_when_monitoring_a_second_counter_should_increment_requests_succeeded_counter()
		{
			const string categoryName = "AnotherMonitoringTestsCounters";

			var counter = GetCounter(SuccessCounter, categoryName);
			var valueBefore = counter == null ? 0 : counter.RawValue;

			Func<bool> function = () => true;
			Monitoring<AnotherMonitoringTestsCounters>.Monitor(function);

			counter = counter ?? GetCounter(SuccessCounter, categoryName);
			counter.RawValue.Should().Be(valueBefore + 1);
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
			GulpException(action);

			GetValue(FailureCounter).Should().Be(valueBefore + 1);
		}

		[Test]
		public void given_function_throws_exception_when_monitoring_should_not_update_requests_response_time_counter()
		{
			var valueBefore = GetValue(ExecutionTimeCounter);

			Func<bool> function = () => { throw new Exception(); };
			Action action = () => Monitoring<MonitoringTestsCounters>.Monitor(function);
			GulpException(action);

			GetValue(ExecutionTimeCounter).Should().Be(valueBefore);
		}

		[Test]
		public void given_action_throws_exception_when_monitoring_should_increment_requests_failed_counter()
		{
			var valueBefore = GetValue(FailureCounter);

			Action action = () => { throw new Exception(); };
			Action outerAction = () => Monitoring<MonitoringTestsCounters>.Monitor(action);
			GulpException(outerAction);

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

			GetCounter(FailureCounter).RawValue.Should().Be(valueBefore + 1);
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

		private long GetValue(string counterName)
		{
			var counter = GetCounter(counterName);
			return counter == null ? 0 : counter.RawValue;
		}

		public void GulpException(Action action)
		{
			try
			{
				action();
			}
			catch (Exception)
			{
			}
		}

		private PerformanceCounter GetCounter(string counterName, string categoryName = "MonitoringTestsCounters")
		{
			PerformanceCounter counter = null;
			const string machineName = ".";
			const string instanceName = "_default";

			if (CounterExists(counterName, categoryName, instanceName, machineName))
			{
				counter = new PerformanceCounter
				{
					CategoryName = categoryName,
					CounterName = counterName,
					InstanceName = instanceName,
					MachineName = machineName
				};
			}

			return counter;
		}

		private static bool CounterExists(string counterName, string categoryName, string instanceName, string machineName)
		{
			return PerformanceCounterCategory.Exists(categoryName)
				&& PerformanceCounterCategory.InstanceExists(instanceName, categoryName, machineName)
				&& PerformanceCounterCategory.CounterExists(counterName, categoryName, machineName);
		}
	}

	public class MonitoringTestsCounters : CounterCategory, IMonitorableCounterCategory
	{
		public Counter Succeeded { get; set; }
		public Counter Failed { get; set; }
		public Counter ExecutionTime { get; set; }
	}

	public class AnotherMonitoringTestsCounters : CounterCategory, IMonitorableCounterCategory
	{
		public Counter Succeeded { get; set; }
		public Counter Failed { get; set; }
		public Counter ExecutionTime { get; set; }
	}
}
