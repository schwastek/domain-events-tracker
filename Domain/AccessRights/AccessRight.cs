using Domain.Applications;
using Domain.Common;
using Domain.Users;

namespace Domain.AccessRights;

public class AccessRight : IIdentity<long>
{
    public long Id { get; private set; }
    public string ApplicationUserId { get; private set; }

    public long UserId { get; private set; }
    public User User { get; private set; }

    public long ApplicationId { get; private set; }
    public Application Application { get; private set; }

    private AccessRight() { }

    public AccessRight(User user, Application application, string applicationUserId)
    {
        User = user;
        UserId = user.Id;
        Application = application;
        ApplicationId = application.Id;
        ApplicationUserId = applicationUserId;
    }

    public override string ToString()
    {
        return $"Access Right: [{Application}, {User}, Application User ID: {ApplicationUserId}]";
    }
}
