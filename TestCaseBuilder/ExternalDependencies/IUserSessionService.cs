using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface IUserSessionService
    {
        (User user, MyServiceErrors error) CheckSession(Guid token);
    }
}
