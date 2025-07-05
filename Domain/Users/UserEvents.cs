using Domain.AccessRights;
using Domain.Authentications;
using Domain.Events;
using Domain.Events.Core.ChangeTracking;
using System.Collections.Generic;

namespace Domain.Users;

public sealed class UserEntityCreatedEvent : EntityCreatedEvent<User>
{
    public UserEntityCreatedEvent(User user) : base(user) { }
}

public sealed class UserEntityChangedEvent : EntityChangedEvent<User>
{
    public UserEntityChangedEvent(User user, IReadOnlyCollection<MemberChanged> changes) : base(user, changes) { }
}

public sealed class UserAuthenticationPropertyChanged : PropertyChanged<Authentication>
{
    public UserAuthenticationPropertyChanged(Authentication? oldValue = null, Authentication? newValue = null)
        : base(propertyName: nameof(User.Authentication), oldValue, newValue, comparer: new AuthenticationEqualityComparer()) { }
}

public sealed class UserAccessRightsCollectionChanged : CollectionChanged<AccessRight>
{
    public UserAccessRightsCollectionChanged(IEnumerable<AccessRight>? added = null, IEnumerable<AccessRight>? removed = null)
        : base(collectionName: nameof(User.AccessRights), added, removed, comparer: new AccessRightEqualityComparer()) { }
}