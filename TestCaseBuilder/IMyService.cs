using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder
{
    public interface IMyService
    {
        UploadDocumentForCustomerResult UploadDocumentForCustomer(string customerCode, string documentFileName, byte[] document, Guid token);
    }
}
