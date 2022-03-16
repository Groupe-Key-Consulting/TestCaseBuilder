namespace TestCaseBuilder.ExternalDependencies
{
    public interface IFileStorageService
    {
        IFile Create(string filename);
        void Upload(IFile file, Stream stream);
    }
}
