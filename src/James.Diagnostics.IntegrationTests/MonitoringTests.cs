using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class MonitoringTests : MonitoringTestsBase
	{
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
						Task.Factory.StartNew(() => Monitoring<MonitoringTestsCounters>.Monitor(() => Thread.Sleep(millisecondsTimeout)));
				}

				Task.WaitAll(taskArray);
			};

			action.ShouldNotThrow<ArgumentException>();
		}
	}
}
