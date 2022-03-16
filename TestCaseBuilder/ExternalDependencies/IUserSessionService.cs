using TestCaseBuilder.BusinessObjects;

namespace TestCaseBuilder.ExternalDependencies
{
    public interface IUserSessionService
    {
        (User user, MyServiceErrors error) CheckSession(Guid token);
    }
}
