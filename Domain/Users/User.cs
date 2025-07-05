using Domain.AccessRights;
using Domain.Authentications;
using Domain.Common;
using Domain.Events.Core;
using Domain.Events.Core.ChangeTracking;
using System;
using System.Collections.Generic;

namespace Domain.Users;

public class User : IIdentity<long>, IHaveDomainEvents
{
    public long Id { get; private set; }
    public Guid ObjectId { get; private set; }

    public long AuthenticationId { get; private set; }
    public Authentication? Authentication { get; private set; }

    private readonly List<AccessRight> _accessRights = [];
    public IReadOnlyCollection<AccessRight> AccessRights => _accessRights.AsReadOnly();

    private readonly DomainEvents _events = new();
    private readonly EntityChangeTracker _tracker = new();

    // EF uses the parameterless constructor to materialize entities.
    public User()
    {
        ObjectId = Guid.NewGuid();

        _events.Add(new UserEntityCreatedEvent(this));
    }

    public void AddAccessRight(AccessRight accessRight)
    {
        _accessRights.Add(accessRight);

        _tracker.Add(new UserAccessRightsCollectionChanged(added: [accessRight]));
    }

    public void RemoveAccessRight(AccessRight accessRight)
    {
        if (_accessRights.Remove(accessRight))
        {
            _tracker.Add(new UserAccessRightsCollectionChanged(removed: [accessRight]));
        }

    }

    public void AddAuthentication(Authentication authentication)
    {
        _tracker.Add(new UserAuthenticationPropertyChanged(oldValue: Authentication, newValue: authentication));

        Authentication = authentication;
        AuthenticationId = authentication.Id;
    }

    public void RemoveAuthentication()
    {
        _tracker.Add(new UserAuthenticationPropertyChanged(oldValue: Authentication, newValue: null));

        Authentication = null;
        AuthenticationId = default;
    }

    public IReadOnlyList<DomainEvent> CollectDomainEvents()
    {
        if (_tracker.HasChanges)
        {
            _events.AddOnce(new UserEntityChangedEvent(this, _tracker.Changes));
        }

        return _events.Collect();
    }

    public void ClearDomainEvents()
    {
        _tracker.Clear();
        _events.Clear();
    }

    public override string ToString()
    {
        return $"User: {ObjectId}";
    }
}
