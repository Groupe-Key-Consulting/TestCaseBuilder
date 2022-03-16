namespace TestCaseBuilder.BusinessObjects
{
    public class DocumentTypeConfig
    {
        public Guid AreaId { get; set; }
        public bool ExternalAccess { get; set; }
        public bool CanBeAddedAfterContractClosed { get; set; }
        public bool ApiDocumentsUploadAllowed { get; set; }
    }
}
