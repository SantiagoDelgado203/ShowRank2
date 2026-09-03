using System.Text.RegularExpressions;

namespace ShowRank.Services;

public static partial class TextUtils
{
    public static string? StripHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var withoutTags = HtmlTagRegex().Replace(input, " ");
        return System.Net.WebUtility.HtmlDecode(withoutTags).Trim();
    }

    public static string Truncate(string text, int maxLength)
    {
        return text.Length <= maxLength ? text : text[..maxLength].TrimEnd() + "…";
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
