using ASPCoreFirstApp.Models;

namespace ASPCoreFirstApp.Models.VM;

public class MovieCustomerViewModel
{
    public Movie Movie { get; set; } = new();

    public List<Customer> Customers { get; set; } = new();
}