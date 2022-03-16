namespace TestCaseBuilder.BusinessObjects
{
    public class CustomerDocument
    {
        public Guid Id { get; set; }
        public string Md5Signature { get; set; }
        public Customer Customer { get; set; }
        public Guid FileId { get; set; }
        public DateTime Date { get; set; }
        public bool ExternalAccess { get; set; }
    }
}
