using Shared;
using System;
using System.Collections.Generic;

namespace Domain.Events.Core.ChangeTracking;

public abstract class PropertyChanged<T> : MemberChanged<T>
{
    public T? OldValue { get; private set; }
    public T? NewValue { get; private set; }

    public PropertyChanged(string propertyName, T? oldValue, T? newValue, IEqualityComparer<T>? comparer)
        : base(propertyName, comparer)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public PropertyChanged<T> MergeWith(PropertyChanged<T> other)
    {
        if (!MemberName.Equals(other.MemberName))
        {
            throw new InvalidOperationException($"Merge failed: properties mismatch. This event tracks '{MemberName}', but attempted to merge with '{other.MemberName}'.");
        }

        // Always keep original old value. Use latest new value.
        NewValue = other.NewValue;

        return this;
    }

    public override bool NoChanges()
    {
        // No change if old and new value are equal by comparer.
        return Comparer.Equals(OldValue, NewValue);
    }

    public override string ToString()
    {
        if (OldValue is null && NewValue is null) return "No changes";

        return $"{MemberName} changed from {OldValue.ToDisplayString()} to {NewValue.ToDisplayString()}";
    }
}
