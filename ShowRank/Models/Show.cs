namespace ShowRank.Models;

public record Show(
    int Rank,
    string Title,
    string Genre,
    string Description,
    double Rating,
    int ReviewCount,
    string AccentColor,
    string Initial);
