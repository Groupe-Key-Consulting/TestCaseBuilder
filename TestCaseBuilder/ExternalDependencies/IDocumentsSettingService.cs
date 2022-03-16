namespace TestCaseBuilder.ExternalDependencies
{
    public interface IDocumentsSettingService
    {
        int GetMaxFileSize();
        string[] GetAllowedFileExtensions();
    }
}
