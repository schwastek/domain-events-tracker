using System;

namespace Domain.Common;

public static class RuntimeHelpers
{
    public static Type GetUnproxiedType(object obj)
    {
        // EF creates runtime proxy types (e.g. for lazy loading),
        // which would otherwise make two equal objects appear to be of different types.
        const string EfCoreProxyPrefix = "Castle.Proxies.";

        var type = obj.GetType();
        var typeString = type.ToString();

        if (typeString.Contains(EfCoreProxyPrefix) && type.BaseType is not null)
        {
            return type.BaseType;
        }

        return type;
    }
}
