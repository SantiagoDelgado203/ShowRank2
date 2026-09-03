namespace ShowRank.Models;

public record WatchedItem(
    int UserId,
    string Title,
    string? ImageUrl,
    string Genre,
    double? Rating,
    MediaKind Kind,
    string SourceUrl,
    DateTime AddedAtUtc);
