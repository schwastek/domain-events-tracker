using Core.Dto;
using Data;
using Domain.Users;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Users;

public sealed class CreateUserRequest : IRequest<UserDtoForRead> { }

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, UserDtoForRead>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateUserRequestHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDtoForRead> Handle(CreateUserRequest request, CancellationToken ct)
    {
        var user = new User();
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);
        var domainEventDescription = user.CollectDomainEvents().OfType<UserEntityCreatedEvent>().First().ToString();

        var response = new UserDtoForRead
        {
            UserObjectId = user.ObjectId,
            DomainEvents = [domainEventDescription]
        };

        return response;
    }
}
