using System;

namespace Core.Dto;

public sealed class AccessRightDtoForRead
{
    public required Guid UserObjectId { get; set; }
    public required string ApplicationCode { get; set; } = string.Empty;
    public required string ApplicationUserId { get; set; } = string.Empty;
}
