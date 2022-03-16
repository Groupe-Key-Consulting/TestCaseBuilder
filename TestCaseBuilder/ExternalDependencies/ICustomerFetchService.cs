using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface ICustomerFetchService
    {
        Customer GetCustomerByCustomerNumber(string customerNumber);
        IQueryable<Customer> GetCustomersWithoutSigByCustomerNumber(string customerNumber);
    }
}
