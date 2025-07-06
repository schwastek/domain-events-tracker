using System;

namespace Core.Dto;

public sealed class AuthenticationDtoForRead
{
    public required Guid UserObjectId { get; set; }
    public required string Username { get; set; } = string.Empty;
}
