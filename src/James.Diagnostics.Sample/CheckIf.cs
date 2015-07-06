namespace James.Diagnostics.Sample
{
	public static class CheckIf
	{
		public static bool IsEven(int number)
		{
			return IsMultipleOf(2, number);
		}

		public static bool IsMultipleOf(int factor, int number)
		{
			return number%factor == 0;
		}
	}
}