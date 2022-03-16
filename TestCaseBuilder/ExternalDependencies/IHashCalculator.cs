namespace TestCaseBuilder.ExternalDependencies
{
    public interface IHashCalculator
    {
        string CalculateHash(Stream stream);
    }
}
