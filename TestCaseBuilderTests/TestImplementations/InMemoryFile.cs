using System;
using TestCaseBuilder.ExternalDependencies;

namespace TestCaseBuilderTests.TestImplementations
{
    public class InMemoryFile : IFile
    {
        public Guid Id { get; set; }
    }
}
