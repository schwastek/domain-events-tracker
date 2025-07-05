using System.Collections.Generic;

namespace Domain.Events.Core;

public interface IHaveDomainEvents
{
    IReadOnlyList<DomainEvent> CollectDomainEvents();
    void ClearDomainEvents();
}
