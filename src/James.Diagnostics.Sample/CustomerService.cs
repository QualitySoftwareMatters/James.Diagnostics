using System.Collections.Generic;
using System.Linq;

namespace James.Diagnostics.Sample
{
	public class CustomerService
	{
		private readonly ICustomerRepository _repository;

		public CustomerService(ICustomerRepository repository)
		{
			_repository = repository;
		}

		public IEnumerable<Customer> GetCustomers(int facilityId)
		{
			return Monitoring<CustomerServiceCounters>.Monitor(() => _repository.GetAll().Where(x => x.FacilityId == facilityId));
		}
	}
}