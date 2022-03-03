using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface IFileStorageService
    {
        IFile Create(string filename);
        void Upload(IFile file, Stream stream);
    }
}
