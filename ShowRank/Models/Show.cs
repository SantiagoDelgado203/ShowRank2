namespace ShowRank.Models;

// Static/sample show data used for Home and Community Rankings (see Data/SampleShows.cs).
public record Show(
    int Rank,
    string Title,
    string Genre,
    string Description,
    double Rating,
    int ReviewCount,
    string AccentColor,
    string Initial);
