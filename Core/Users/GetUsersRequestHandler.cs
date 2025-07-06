using Core.Dto;
using Data;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Users;

public sealed class GetUsersRequest : IRequest<GetUsersResponse> { }

public sealed class GetUsersResponse
{
    public required IReadOnlyCollection<UserDtoForRead> Users { get; set; } = [];
}

public class GetUsersRequestHandler : IRequestHandler<GetUsersRequest, GetUsersResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetUsersRequestHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken ct)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Authentication)
            .Include(u => u.AccessRights)
                .ThenInclude(ar => ar.Application)
            .ToListAsync(ct);

        var response = Map(users);

        return response;
    }

    private static GetUsersResponse Map(List<User> users)
    {
        var dto = users.Select(user =>
        {
            var authenticationDto = MapAuthentication(user);
            var accessRightsDto = MapAccessRights(user);

            return new UserDtoForRead
            {
                UserObjectId = user.ObjectId,
                Authentication = authenticationDto,
                AccessRights = accessRightsDto,
                DomainEvents = []
            };
        }).ToList();

        var response = new GetUsersResponse { Users = dto };

        return response;
    }

    private static AuthenticationDtoForRead? MapAuthentication(User user)
    {
        AuthenticationDtoForRead? dto = null;

        if (user.Authentication is not null)
        {
            dto = new AuthenticationDtoForRead
            {
                Username = user.Authentication.Username,
                UserObjectId = user.ObjectId
            };
        }

        return dto;
    }

    private static List<AccessRightDtoForRead> MapAccessRights(User user)
    {
        var dto = user.AccessRights.Select(static ar =>
        {
            return new AccessRightDtoForRead
            {
                UserObjectId = ar.User.ObjectId,
                ApplicationCode = ar.Application.Code,
                ApplicationUserId = ar.ApplicationUserId
            };
        }).ToList();

        return dto;
    }
}
