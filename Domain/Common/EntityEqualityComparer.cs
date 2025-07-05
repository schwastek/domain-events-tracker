using System.Collections.Generic;

namespace Domain.Common;

public abstract class EntityEqualityComparer<TEntity, TId> : IEqualityComparer<TEntity>
    where TEntity : class, IIdentity<TId>
    where TId : notnull
{
    public bool Equals(TEntity? x, TEntity? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (RuntimeHelpers.GetUnproxiedType(x) != RuntimeHelpers.GetUnproxiedType(y)) return false;
        if (IsTransient(x) || IsTransient(y)) return false;

        // EqualityComparer<T>.Default is smart: if T implements IEquatable<T>, it uses that implementation.
        return EqualityComparer<TId>.Default.Equals(x.Id, y.Id);
    }

    public int GetHashCode(TEntity obj)
    {
        // Notes:
        // - GetHashCode must return the same value throughout an object's lifetime.
        // - EF Core entities may change identity (e.g. ID is assigned after SaveChanges), so their hash code can change if it depends on the ID (Primary Key).
        // - This creates a problem when adding transient entities (no ID yet) to hash-based collections like HashSet or Dictionary before saving them.
        // - Always avoid changing data used in GetHashCode once an entity is placed in a hash-based collection.

        // Important:
        // - Equal hash codes do NOT mean entities are equal.
        // - HashCode is used to group objects into buckets; actual equality is checked using Equals().

        // Best practices:
        // - Use immutable, unique identifiers (e.g., database ID or natural key) in GetHashCode.
        // - Do NOT base GetHashCode on mutable fields or fields that change after SaveChanges.
        if (IsTransient(obj))
        {
            return base.GetHashCode();
        }

        return EqualityComparer<TId>.Default.GetHashCode(obj.Id);
    }

    private static bool IsTransient(TEntity entity)
    {
        // Checks whether the entity is new (no PK assigned).
        return EqualityComparer<TId>.Default.Equals(entity.Id, default);
    }
}