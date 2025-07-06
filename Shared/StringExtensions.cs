namespace Shared;

public static class StringExtensions
{
    public static string ToDisplayString(this object? value)
    {
        return value?.ToString() ?? "null";
    }
}
