# James.Diagnostics

[![NuGet version (James.Diagnostics)](https://img.shields.io/nuget/v/James.Diagnostics.svg?style=flat)](https://www.nuget.org/packages/James.Diagnostics/)
[![NuGet version (James.Diagnostics)](https://img.shields.io/nuget/dt/James.Diagnostics.svg?style=flat)](https://www.nuget.org/packages/James.Diagnostics/)

## Overview ##
James.Diagnostics is a convention-based library built on top [Magnum](https://www.nuget.org/packages/Magnum/) to help you to add custom performance counters to your existing applications.  It is based on the assumption that in most cases you will want to instrument your application logic for:

* Successes
* Failures
* Average Execution Times

If you need other types of custom counters, libraries such as [Magnum](https://www.nuget.org/packages/Magnum/), written by Chris Patterson and Dru Sellers, can help you.

## Creating Performance Counters ##
Let's say you have a service that allows clients to get and update customer information.

<pre lang="csharp">
public IEnumerable<Customer> GetCustomers()
{
	return _repository.GetAll();
}

public void UpdateCustomer(Customer customer)
{
	_repository.Update(customer);
}
</pre>

Creating custom performance counters for these two methods is as simple as inheriting CounterCategory and implementing the IMonitorableCounterCategory for each as shown below.

<pre lang="csharp">
public class CustomerService_GetCustomersCounters 
	: CounterCategory, IMonitorableCounterCategory
{
  public Counter Succeeded { get; set; }
  public Counter Failed { get; set; }
  public Counter ExecutionTime { get; set; }
}

public class CustomerService_UpdateCustomerCounters 
	: CounterCategory, IMonitorableCounterCategory
{
  public Counter Succeeded { get; set; }
  public Counter Failed { get; set; }
  public Counter ExecutionTime { get; set; }
}
</pre>

Once you have these in place, you can begin to use those to <code lang="csharp">Monitor()</code> your methods.

<pre lang="csharp">
public IEnumerable<Customer> GetCustomers()
{
	return Monitoring&lt;CustomerService_GetCustomerCounters&gt;.Monitor(() => _repository.GetAll());
}

public void UpdateCustomer(Customer customer)
{
	Monitoring&lt;CustomerService_UpdateCustomerCounters&gt;.Monitor(() => _repository.Update(customer));
}
</pre>

Below is a sample screenshot of Performance Monitor showing one of these performance counter categories in action.  In the graph there are three colored lines representing the three counters.

* Blue Line - represents the execution times in milliseconds.  
* Green Line - represents the number of successes.
* Red Line - represents the number of failures.

![capture-perfmon-james-diagnostics](https://cloud.githubusercontent.com/assets/177508/8517979/76eab4fe-238b-11e5-9c2a-febcaaca00cd.PNG)

If these metrics are brought into tools such as SCOM, they can be further manipulated to derive computed metrics such as the percentage of successes or average execution times aggregated by time periods.

## Synchronous Monitoring Options ##

There are multiple options for monitoring the execution of your synchronous code with custom performance counters.

### <code lang="csharp">void Monitor(Action action)</code> ###

Takes an action and increments the success/failure counters depending on whether or not the action succeeds (ie. - does not throw).  If an exception does occur, it will bubble this up.

### <code lang="csharp">T Monitor&lt;T&gt;(Func&lt;T&gt; function)</code> ###

Takes a function and increments the success/failure counters depending on whether or not the action succeeds (ie. -does not throw).  If an exception does occur, it will bubble this up.  If it succeeds, it will also return the result of the function call.

### <code lang="csharp">void Monitor(Func&lt;bool&gt; function)</code> ###

Takes a function that returns a true/false and increments the success/failure counters depending on whether or not the function returns a true or false.  It will not return the boolean value to the client, however.  So, only use this method if are instrumenting something that gracefully handles exceptions by not bubbling them up. 

## Asynchronous Monitoring Options ##

Sometimes, the distance between beginning and ending your monitoring is not just in time but also by process or machine.  This often occurs in distributed systems (especially messaging) where the process that you would like to instrument occurs on one machine or process and ends in another.  James.Diagnostics has you covered in these scenarios.

### Process 1:  Sending Message ###
<pre lang="csharp">
public void SendMessage(Customer customer)
{
	var message = new CustomerUpdate
		{  
			Customer = Customer, 
			Success = true,
			Start = DateTime.Now.ToUniversalTime()
		};

	_bus.Publish(message);
}
</pre>

### Process 2:  Consuming Message ###
<pre lang="csharp">
public void Consume(CustomerUpdate message)
{
	var elapsed = DateTime.Now.ToUniversalTime().Subtract(message.Start);
	message.Success 
		? Monitoring&lt;CustomerService_CustomerUpdatedCounters&gt;.Success(elapsed)
		: Monitoring&lt;CustomerService_CustomerUpdatedCounters&gt;.Failure();
}

</pre>

You will notice that in the case of the <code lang="csharp">Failure()</code> method, there is no <code lang="csharp">TimeSpan</code> to be provided.  This is because a failure will either happen immediately or due to timeout, may be very lengthy.  In either situation, you will not want to skew your average execution time metrics.
