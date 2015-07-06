using System.Collections.Generic;

namespace James.Diagnostics.Sample
{
	public interface ICustomerRepository
	{
		IEnumerable<Customer> GetAll();
	}
}