namespace ShowRank.Models;

// Which external source a result came from: AniList (Anime) or TVMaze (Show).
public enum MediaKind
{
    Anime,
    Show,
}

// One search hit, merged into a common shape from either AniList or TVMaze.
public record SearchResult(
    string Title,
    string? ImageUrl,
    string? Description,
    double? Rating,
    string Genre,
    MediaKind Kind,
    string SourceUrl);
