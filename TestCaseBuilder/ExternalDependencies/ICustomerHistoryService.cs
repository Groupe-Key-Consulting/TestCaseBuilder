namespace TestCaseBuilder.ExternalDependencies
{
    public interface ICustomerHistoryService
    {
        void UpdateCustomer(string customerNumber, string userLogin, object customerUpdate);
    }
}
