using Domain.AccessRights;
using Domain.Common;
using System.Collections.Generic;

namespace Domain.Applications;

public class Application : IIdentity<long>
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

    public override string ToString()
    {
        return $"Application: {Code}";
    }
}
