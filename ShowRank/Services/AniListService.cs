using System.Net.Http.Json;
using System.Text.Json;
using ShowRank.Models;

namespace ShowRank.Services;


public class AniListService(HttpClient httpClient)
{
    private const string Endpoint = "https://graphql.anilist.co";

    private const string Query = """
        query ($search: String, $perPage: Int) {
          Page(perPage: $perPage) {
            media(search: $search, type: ANIME, sort: SEARCH_MATCH) {
              title { romaji english }
              description(asHtml: false)
              averageScore
              genres
              coverImage { medium }
              siteUrl
            }
          }
        }
        """;

    //API call, 
    public async Task<List<SearchResult>> SearchAsync(string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        //query + api
        var payload = new { query = Query, variables = new { search = query, perPage = limit } };
        using var response = await httpClient.PostAsJsonAsync(Endpoint, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        //parse
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var mediaList = doc.RootElement.GetProperty("data").GetProperty("Page").GetProperty("media");

        var results = new List<SearchResult>();
        //loop through json
        foreach (var media in mediaList.EnumerateArray())
        {
            var title = media.GetProperty("title");
            var name = title.TryGetProperty("english", out var en) && en.ValueKind == JsonValueKind.String
                ? en.GetString()
                : title.GetProperty("romaji").GetString();

            var description = media.TryGetProperty("description", out var desc) && desc.ValueKind == JsonValueKind.String
                ? TextUtils.StripHtml(desc.GetString())
                : null;

            double? rating = media.TryGetProperty("averageScore", out var score) && score.ValueKind == JsonValueKind.Number
                ? score.GetDouble() / 10.0
                : null;

            var genres = media.TryGetProperty("genres", out var genresEl) && genresEl.ValueKind == JsonValueKind.Array
                ? string.Join(", ", genresEl.EnumerateArray().Select(g => g.GetString()))
                : string.Empty;
            if (string.IsNullOrWhiteSpace(genres))
            {
                genres = "Anime";
            }

            var imageUrl = media.TryGetProperty("coverImage", out var cover) && cover.TryGetProperty("medium", out var medium)
                ? medium.GetString()
                : null;

            var siteUrl = media.TryGetProperty("siteUrl", out var site) ? site.GetString() ?? "https://anilist.co" : "https://anilist.co";

            results.Add(new SearchResult(name ?? "Untitled", imageUrl, description, rating, genres, MediaKind.Anime, siteUrl));
        }
        //return list of shows
        return results;
    }
}
