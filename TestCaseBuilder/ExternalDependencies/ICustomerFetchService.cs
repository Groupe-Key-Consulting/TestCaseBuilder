using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface ICustomerFetchService
    {
        Customer GetCustomerByCustomerNumber(string customerNumber);
        IQueryable<Customer> GetCustomersWithoutSigByCustomerNumber(string customerNumber);
    }
}
