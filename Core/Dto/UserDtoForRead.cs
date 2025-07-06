using System;
using System.Collections.Generic;

namespace Core.Dto;

public sealed class UserDtoForRead
{
    public required Guid UserObjectId { get; set; }
    public AuthenticationDtoForRead? Authentication { get; set; }
    public IReadOnlyCollection<AccessRightDtoForRead> AccessRights { get; set; } = [];
    public IReadOnlyCollection<string> DomainEvents { get; set; } = [];
}
