using System.Collections.Generic;

namespace Shared;

public static class CollectionExtensions
{
    public static string ToJoinedString<T>(this IEnumerable<T> items, string separator = ", ")
    {
        return items is null
            ? string.Empty
            : string.Join(separator, items);
    }
}
