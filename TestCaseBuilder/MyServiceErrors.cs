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
