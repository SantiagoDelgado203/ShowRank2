using ShowRank.Models;

namespace ShowRank.Services;

public class SearchService(AniListService aniListService, TvMazeService tvMazeService, ILogger<SearchService> logger)
{
    public async Task<List<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        //Check search validation
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }
        //Calls services for show apis,etc
        var animeTask = SafeSearchAsync(() => aniListService.SearchAsync(query, cancellationToken: cancellationToken), "AniList");
        var showTask = SafeSearchAsync(() => tvMazeService.SearchAsync(query, cancellationToken: cancellationToken), "TVMaze");
        
        //waits until both tasks are done
        var resultSets = await Task.WhenAll(animeTask, showTask);
        //select many to make one big list
        return resultSets
            .SelectMany(set => set)
            .OrderByDescending(r => r.Rating ?? 0)
            .ToList();
    }
    //Resiliency lambda
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
