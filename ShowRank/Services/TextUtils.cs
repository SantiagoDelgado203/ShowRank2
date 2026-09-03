using System.Text.RegularExpressions;

namespace ShowRank.Services;

//
public static partial class TextUtils
{
    //used to turn summaries from AniList and TvMaze services to plain text
    public static string? StripHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }
        //regular expression
        var withoutTags = HtmlTagRegex().Replace(input, " ");
        //to plain text
        return System.Net.WebUtility.HtmlDecode(withoutTags).Trim();
    }

    public static string Truncate(string text, int maxLength)
    {
        return text.Length <= maxLength ? text : text[..maxLength].TrimEnd() + "…";
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex HtmlTagRegex();
}
