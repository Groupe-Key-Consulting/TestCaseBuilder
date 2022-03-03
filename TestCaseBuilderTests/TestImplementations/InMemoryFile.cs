using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.ExternalDependencies;

namespace TestCaseBuilderTests.TestImplementations
{
    public class InMemoryFile : IFile
    {
        public Guid Id { get; set; }
    }
}
