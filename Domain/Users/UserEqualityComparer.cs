using Domain.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Users;

public class UserEqualityComparer : IEqualityComparer<User>
{
    public bool Equals(User? x, User? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (RuntimeHelpers.GetUnproxiedType(x) != RuntimeHelpers.GetUnproxiedType(y)) return false;

        return x.ObjectId.Equals(y.ObjectId);
    }

    public int GetHashCode([DisallowNull] User obj)
    {
        return obj.GetHashCode();
    }
}
