using System;
using System.Collections.Generic;
using System.Threading;

namespace James.Diagnostics.Sample
{
	public class InMemoryCustomerRepository : ICustomerRepository
	{
		private readonly Random _random = new Random();

		private readonly List<Customer> _customers = new List<Customer>
		{
			new Customer {FirstName = "Todd", LastName = "Meinershagen", FacilityId = 1},
			new Customer {FirstName = "Tammy", LastName = "Meinershagen", FacilityId = 2},
			new Customer {FirstName = "Elysa", LastName = "Meinershagen", FacilityId = 1},
		};

		public IEnumerable<Customer> GetAll()
		{
			var failed = CheckIf.IsMultipleOf(5, _random.Next(1, 100));
			Thread.Sleep(_random.Next(0, 1000));

			if (failed)
			{
				throw new Exception("Random exception occurred.");
			}

			return _customers;
		}
	}
}