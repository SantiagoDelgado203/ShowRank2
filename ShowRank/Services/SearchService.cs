using ShowRank.Models;

namespace ShowRank.Services;

public class SearchService(AniListService aniListService, TvMazeService tvMazeService, ILogger<SearchService> logger)
{
    public async Task<List<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var animeTask = SafeSearchAsync(() => aniListService.SearchAsync(query, cancellationToken: cancellationToken), "AniList");
        var showTask = SafeSearchAsync(() => tvMazeService.SearchAsync(query, cancellationToken: cancellationToken), "TVMaze");

        var resultSets = await Task.WhenAll(animeTask, showTask);
        return resultSets
            .SelectMany(set => set)
            .OrderByDescending(r => r.Rating ?? 0)
            .ToList();
    }

    private async Task<List<SearchResult>> SafeSearchAsync(Func<Task<List<SearchResult>>> search, string sourceName)
    {
        try
        {
            return await search();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Search against {Source} failed", sourceName);
            return [];
        }
    }
}
