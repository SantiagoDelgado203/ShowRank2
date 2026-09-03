namespace ShowRank.Models;

public enum MediaKind
{
    Anime,
    Show,
}

public record SearchResult(
    string Title,
    string? ImageUrl,
    string? Description,
    double? Rating,
    string Genre,
    MediaKind Kind,
    string SourceUrl);
