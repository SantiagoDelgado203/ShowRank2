using System.Text.Json;
using ShowRank.Models;

namespace ShowRank.Services;

public class TvMazeService(HttpClient httpClient)
{
    public async Task<List<SearchResult>> SearchAsync(string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        var url = $"https://api.tvmaze.com/search/shows?q={Uri.EscapeDataString(query)}";
        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var results = new List<SearchResult>();
        foreach (var entry in doc.RootElement.EnumerateArray().Take(limit))
        {
            var show = entry.GetProperty("show");
            var name = show.GetProperty("name").GetString() ?? "Untitled";

            var summary = show.TryGetProperty("summary", out var summaryEl) && summaryEl.ValueKind == JsonValueKind.String
                ? TextUtils.StripHtml(summaryEl.GetString())
                : null;

            double? rating = show.TryGetProperty("rating", out var ratingEl)
                && ratingEl.ValueKind == JsonValueKind.Object
                && ratingEl.TryGetProperty("average", out var avgEl)
                && avgEl.ValueKind == JsonValueKind.Number
                ? avgEl.GetDouble()
                : null;

            var genres = show.TryGetProperty("genres", out var genresEl) && genresEl.ValueKind == JsonValueKind.Array
                ? string.Join(", ", genresEl.EnumerateArray().Select(g => g.GetString()))
                : string.Empty;
            if (string.IsNullOrWhiteSpace(genres))
            {
                genres = "TV Show";
            }

            var imageUrl = show.TryGetProperty("image", out var imageEl) && imageEl.ValueKind == JsonValueKind.Object
                && imageEl.TryGetProperty("medium", out var mediumEl)
                ? mediumEl.GetString()
                : null;

            var siteUrl = show.TryGetProperty("url", out var urlEl) ? urlEl.GetString() ?? "https://www.tvmaze.com" : "https://www.tvmaze.com";

            results.Add(new SearchResult(name, imageUrl, summary, rating, genres, MediaKind.Show, siteUrl));
        }

        return results;
    }
}
