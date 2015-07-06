using System;

namespace James.Diagnostics
{
	internal static class DoubleExtensions
	{
		public static long AsLong(this double value)
		{
			return Convert.ToInt64(value);
		}
	}
}
