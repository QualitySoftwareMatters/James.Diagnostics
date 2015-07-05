using FluentAssertions;

using NUnit.Framework;

namespace James.Diagnostics.IntegrationTests
{
	[TestFixture]
	public class DoubleExtensionsTests
	{
		[TestCase(1.0, 1)]
		[TestCase(-1.0, -1)]
		[TestCase(0.0, 0)]
		public void given_double_value_when_casting_as_long_should_be_long_value(double input, long expected)
		{
			input.AsLong().Should().Be(expected);
		}
	}
}
