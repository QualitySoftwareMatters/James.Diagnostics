using System;
using System.Collections.Generic;

namespace James.Diagnostics.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var repository = new InMemoryCustomerRepository();
			var service = new CustomerService(repository);
			var random = new Random();

			Console.WriteLine("Press 'q' and hit ENTER to quit.");
			Console.WriteLine("Press any other key and ENTER to get customers.");
			Console.WriteLine("");
			
			while (Console.ReadKey().Key != ConsoleKey.Q)
			{
				var facilityId = CheckIf.IsEven(random.Next(1, 100)) 
					? 2 
					: 1;

				try
				{
					var customers = service.GetCustomers(facilityId);
					DisplayCustomers(facilityId, customers);
				}
				catch (Exception)
				{
					DisplayFailure(facilityId);
				}
			}

			Console.ReadLine();
		}

		private static void DisplayFailure(int facilityId)
		{
			Console.WriteLine("Facility Id:  {0} - Failed", facilityId);
			DisplayLineBreak();
		}

		private static void DisplayCustomers(int facilityId, IEnumerable<Customer> customers)
		{
			Console.WriteLine("Facility Id:  {0} - Succeeded", facilityId);
			Console.WriteLine("Customers:");

			foreach (var customer in customers)
			{
				Console.WriteLine("	{0}, {1}", customer.LastName, customer.FirstName);
			}
			
			DisplayLineBreak();
		}

		private static void DisplayLineBreak()
		{
			Console.WriteLine("=========================================================================");
		}
	}
}
