using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface ICustomerHistoryService
    {
        void UpdateCustomer(string customerNumber, string userLogin, object customerUpdate);
    }
}
