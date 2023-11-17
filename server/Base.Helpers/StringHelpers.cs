namespace Base.Helpers;

public static class StringHelpers
{
    public static string? NullIfEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str) ? null : str;
    }
}