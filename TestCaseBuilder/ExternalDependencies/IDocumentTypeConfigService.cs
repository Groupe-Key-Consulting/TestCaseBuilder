using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface IDocumentTypeConfigService
    {
        DocumentTypeConfig GetByArea(Guid guid);
        DocumentTypeConfig GetByArea(object areaId, Func<object, bool> p1, object p2);
    }
}
