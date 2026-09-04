namespace ShowRank.Models;

// A show/anime a user has saved to their watched list, persisted via WatchedListStore.
public record WatchedItem(
    int UserId,
    string Title,
    string? ImageUrl,
    string Genre,
    double? Rating,
    MediaKind Kind,
    string SourceUrl,
    DateTime AddedAtUtc);
