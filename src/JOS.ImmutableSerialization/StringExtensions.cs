namespace JOS.ImmutableSerialization;

internal static class StringExtensions
{
    internal static string ToCamelCase(this string str)
    {
        if(string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if(str.Length == 1)
        {
            return str.ToLower();
        }

        return char.ToLower(str[0]) + str[1..];
    }
}
