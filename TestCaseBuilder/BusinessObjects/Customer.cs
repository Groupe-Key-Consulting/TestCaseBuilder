using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseBuilder.BusinessObjects
{
    public class Customer
    {
        public Guid AreaId { get; set; }
        public string CustomerCodeWithoutSig { get; set; }
        public string CustomerSig { get; set; }
        public bool ContractIsClosed { get; set; }
        public List<CustomerDocument> Documents { get; set; } = new List<CustomerDocument>();
    }
}
