using Domain.AccessRights;
using Domain.Applications;
using Domain.Authentications;
using Domain.Users;
using Shared;
using System.Linq;
using Xunit;

namespace UnitTests.Domain.Users;

public sealed class UserDomainEventsTests
{
    [Fact]
    public void Creating_user_entity_emits_expected_event()
    {
        // Arrange
        var user = User.Create();

        // Act
        var events = user.CollectDomainEvents();

        // Assert
        Assert.Single(events.OfType<UserEntityCreatedEvent>());
    }

    [Fact]
    public void Changing_authentication_property_emits_expected_event()
    {
        // Arrange
        var user = User.Create();

        var authentication1 = new Authentication(username: RandomStringGenerator.Generate());
        var authentication2 = new Authentication(username: RandomStringGenerator.Generate());
        var authentication3 = new Authentication(username: RandomStringGenerator.Generate());

        user.AddAuthentication(authentication1);
        user.AddAuthentication(authentication2);
        user.AddAuthentication(authentication3);

        // Act
        var events = user.CollectDomainEvents();

        // Assert
        var expectedEvent = Assert.Single(events.OfType<UserEntityChangedEvent>());
        var change = Assert.IsType<UserAuthenticationPropertyChanged>(expectedEvent.Changes.First());

        Assert.Null(change.OldValue);
        Assert.Equal(authentication3, change.NewValue);
    }

    [Fact]
    public void Changing_access_rights_collection_emits_expected_event()
    {
        // Arrange
        var user = User.Create();
        var application = new Application(code: RandomStringGenerator.Generate());

        var accessRight1 = new AccessRight(user, application, applicationUserId: RandomStringGenerator.Generate());
        var accessRight2 = new AccessRight(user, application, applicationUserId: RandomStringGenerator.Generate());

        user.AddAccessRight(accessRight1);
        user.AddAccessRight(accessRight2);
        user.RemoveAccessRight(accessRight1);

        // Act
        var events = user.CollectDomainEvents();

        // Assert
        var expectedEvent = Assert.Single(events.OfType<UserEntityChangedEvent>());

        Assert.Single(expectedEvent.Changes);
        var change = Assert.IsType<UserAccessRightsCollectionChanged>(expectedEvent.Changes.First());

        Assert.Contains(accessRight2, change.AddedItems);
        Assert.Empty(change.RemovedItems);
    }

    [Fact]
    public void Clear_domain_events_removes_all_events()
    {
        // Arrange
        var user = User.Create();
        var application = new Application(code: RandomStringGenerator.Generate());

        var accessRight = new AccessRight(user, application, applicationUserId: RandomStringGenerator.Generate());
        var authentication = new Authentication(username: RandomStringGenerator.Generate());

        // Act
        user.AddAuthentication(authentication);
        user.AddAccessRight(accessRight);

        user.ClearDomainEvents();
        var events = user.CollectDomainEvents();

        // Assert
        Assert.Empty(events);
    }

    [Fact]
    public void No_events_emitted_when_mutually_exclusive_changes()
    {
        // Arrange
        var user = User.Create();
        var application = new Application(code: RandomStringGenerator.Generate());

        var accessRight = new AccessRight(user, application, applicationUserId: RandomStringGenerator.Generate());
        var authentication = new Authentication(username: RandomStringGenerator.Generate());

        user.AddAccessRight(accessRight);
        user.RemoveAccessRight(accessRight);

        user.AddAuthentication(authentication);
        user.RemoveAuthentication();

        // Act
        var events = user.CollectDomainEvents();

        // Assert
        Assert.Empty(events.OfType<UserEntityChangedEvent>());
    }
}
