using Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Applications;

public class ApplicationEqualityComparer : IEqualityComparer<Application>
{
    public bool Equals(Application? x, Application? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (RuntimeHelpers.GetUnproxiedType(x) != RuntimeHelpers.GetUnproxiedType(y)) return false;

        return string.Equals(x.Code, y.Code, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode([DisallowNull] Application obj)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Code);
    }
}
