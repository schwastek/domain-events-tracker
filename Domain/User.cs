using System;
using System.Collections.Generic;

namespace Domain;

public class User
{
    public long Id { get; private set; }
    public Guid ObjectId { get; private set; }

    public long AuthenticationId { get; private set; }
    public Authentication? Authentication { get; private set; }

    private readonly List<AccessRight> _accessRights = [];
    public IReadOnlyCollection<AccessRight> AccessRights => _accessRights.AsReadOnly();

    // EF uses the parameterless constructor to materialize entities.
    public User()
    {
        ObjectId = Guid.NewGuid();
    }

    public void AddAccessRight(AccessRight accessRight)
    {
        _accessRights.Add(accessRight);
    }

    public void SetAuthentication(Authentication authentication)
    {
        Authentication = authentication;
        AuthenticationId = authentication.Id;
    }
}
