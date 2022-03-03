using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseBuilder
{
    public enum MyServiceErrors
    {
        None,
        SessionIsNotValid,
        InvoiceFormatNotAllowed,
        DocumentTypeNotAllowedAfterContractClosing,
        DocumentSizeExceedsTheMaxAllowed,
        CustomerNotFound,
        UploadNotAllowedThroughAPI,
        DocumentAlreadyExists,
    }
}
