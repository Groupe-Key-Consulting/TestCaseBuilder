using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseBuilder.BusinessObjects
{
    public class UploadDocumentForCustomerResult
    {
        public MyServiceErrors Error { get; set; }
        public string UrlDocumentFile { get; set; }
        public User User { get; internal set; }
        public bool HasError => Error != MyServiceErrors.None;
        public Customer Customer { get; internal set; }
        public CustomerDocument NewDocument { get; internal set; }
        public List<Customer> CustomersWithoutSig { get; internal set; }
    }
}
