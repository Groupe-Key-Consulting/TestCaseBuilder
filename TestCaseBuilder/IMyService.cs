using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder
{
    public interface IMyService
    {
        UploadDocumentForCustomerResult UploadDocumentForCustomer(string customerCode, string documentFileName, byte[] document, Guid token);
    }
}
