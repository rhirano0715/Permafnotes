namespace PermafnotesDomain.Extensions;

using System.Collections.Generic;
using System.Linq;


internal static class StringExtensions
{
    internal static string Join(string delimiter, IEnumerable<string> values)
        => string.Join(delimiter, values.Select(x => $"\"{x}\""));
    internal static string EscapeDoubleQuote(this string target, string escaper = "\"")
        => target.Replace("\"", $"{escaper}\"");
}
