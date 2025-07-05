namespace Domain.Common;

public interface IIdentity<TId>
{
    TId Id { get; }
}
