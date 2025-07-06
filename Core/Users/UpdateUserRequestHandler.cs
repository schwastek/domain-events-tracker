using Core.Dto;
using Data;
using Data.Extensions;
using Domain.AccessRights;
using Domain.Authentications;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Users;

public sealed class UpdateUserRequest : IRequest<UserDtoForRead>
{
    public Guid UserObjectId { get; set; }
}

public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, UserDtoForRead>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateUserRequestHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDtoForRead> Handle(UpdateUserRequest request, CancellationToken ct)
    {
        // Modify the User entity multiple times to trigger domain events for testing.
        var user = await GetUser(request.UserObjectId, ct);
        await ReplaceAccessRight(user, ct);
        ReplaceAuthentication(user);

        // EF nulls AccessRight.User when it's removed from User.AccessRights during SaveChanges.
        // UserAccessRightsCollectionChanged.ToString() accesses User properties via each AccessRight.
        // To avoid null references in ToString(), domain events are collected before SaveChanges while navigation properties are still available.
        // Alternatively, evaluate ToString() immediately when raising the event (don't defer calls to later).
        // Or keep ToString() independent of navigation properties.
        var domainEventDescription = user.CollectDomainEvents().OfType<UserEntityChangedEvent>().First().ToString();

        await _dbContext.SaveChangesAsync(ct);

        // Map.
        var response = Map(user, domainEventDescription);

        return response;
    }

    private async Task<User> GetUser(Guid objectId, CancellationToken ct)
    {
        var user = await _dbContext.Users
            .Include(u => u.Authentication)
            .Include(u => u.AccessRights)
                // We eager-load AccessRights along with their related Applications,
                // because UserAccessRightsCollectionChanged.ToString() references Application properties.
                // Including Application here ensures ToString() won't hit null.
                .ThenInclude(ar => ar.Application)
            .Include(u => u.AccessRights)
            .Where(u => u.ObjectId.Equals(objectId))
            .SingleAsync(ct);

        return user;
    }

    private static void ReplaceAuthentication(User user)
    {
        if (user.Authentication is null)
        {
            var authentication = new Authentication(username: RandomStringGenerator.Generate());
            user.AddAuthentication(authentication);
        }
        else
        {
            // The following method performs the action, but it does not emit the change event.
            //user.Authentication.SetUsername(username: StringExtensions.Generate());

            user.RemoveAuthentication();
            var authentication = new Authentication(username: RandomStringGenerator.Generate());
            user.AddAuthentication(authentication);
        }
    }

    private async Task ReplaceAccessRight(User user, CancellationToken ct)
    {
        var existingAccessRight = user.AccessRights.FirstOrDefault();
        if (existingAccessRight is not null)
        {
            user.RemoveAccessRight(existingAccessRight);
        }

        var application = await _dbContext.Applications.GetRandomAsync(ct);

        var accessRight = new AccessRight(user, application!, applicationUserId: RandomStringGenerator.Generate());
        user.AddAccessRight(accessRight);
    }

    private static UserDtoForRead Map(User user, string domainEventDescription)
    {
        AuthenticationDtoForRead? authenticationDto = null;
        if (user.Authentication is not null)
        {
            authenticationDto = new AuthenticationDtoForRead
            {
                Username = user.Authentication.Username,
                UserObjectId = user.ObjectId
            };
        }

        var accessRightsDto = user.AccessRights.Select(ar => new AccessRightDtoForRead
        {
            UserObjectId = ar.User.ObjectId,
            ApplicationCode = ar.Application.Code,
            ApplicationUserId = ar.ApplicationUserId
        }).ToList();

        var dto = new UserDtoForRead
        {
            UserObjectId = user.ObjectId,
            Authentication = authenticationDto,
            AccessRights = accessRightsDto,
            DomainEvents = [domainEventDescription]
        };

        return dto;
    }
}
