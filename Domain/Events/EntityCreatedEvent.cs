using Domain.Events.Core;

namespace Domain.Events;

public abstract class EntityCreatedEvent<TEntity> : DomainEvent
{
    public TEntity Entity { get; }

    public EntityCreatedEvent(TEntity entity)
    {
        Entity = entity;
    }

    public override string ToString()
    {
        return $"Entity created: [{Entity}]";
    }
}
