using System.Collections.Generic;

namespace Domain;

public class Application
{
    public long Id { get; private set; }
    public string Code { get; private set; }

    private readonly List<AccessRight> _accessRights = [];
    public IReadOnlyCollection<AccessRight> AccessRights => _accessRights.AsReadOnly();

    private Application() { }

    public Application(string code)
    {
        Code = code;
    }
}
