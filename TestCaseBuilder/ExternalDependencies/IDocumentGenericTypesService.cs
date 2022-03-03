using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface IDocumentGenericTypesService
    {
        void GetByCode(int documentCode);
    }
}
