using Domain.Events.Core;
using Domain.Events.Core.ChangeTracking;
using System.Collections.Generic;
using System.Text;

namespace Domain.Events;

public abstract class EntityChangedEvent<TEntity> : DomainEvent
{
    public TEntity Entity { get; }
    public IReadOnlyCollection<MemberChanged> Changes { get; }

    public EntityChangedEvent(TEntity entity, IReadOnlyCollection<MemberChanged> changes)
    {
        Entity = entity;
        Changes = changes;
    }

    public override string ToString()
    {
        if (Changes.Count == 0) return "No changes";

        var sb = new StringBuilder();
        sb.Append($"{Entity} changed.");

        foreach (var change in Changes)
        {
            sb.Append(' ');
            sb.Append(change);
            sb.Append('.');
        }

        return sb.ToString();
    }
}
