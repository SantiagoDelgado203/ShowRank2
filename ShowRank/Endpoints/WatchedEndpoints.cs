using System.Security.Claims;
using ShowRank.Data;
using ShowRank.Models;

namespace ShowRank.Endpoints;

public static class WatchedEndpoints
{
    public static void MapWatchedEndpoints(this WebApplication app)
    {
        app.MapPost("/watched/add", async (HttpContext context, WatchedListStore store) =>
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect("/account");
            }

            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var form = context.Request.Form;

            var title = form["title"].ToString();
            var sourceUrl = form["sourceUrl"].ToString();
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(sourceUrl))
            {
                return Results.BadRequest();
            }

            var kind = Enum.TryParse<MediaKind>(form["kind"].ToString(), out var parsedKind) ? parsedKind : MediaKind.Show;
            var rating = double.TryParse(form["rating"].ToString(), out var parsedRating) ? parsedRating : (double?)null;
            var genre = form["genre"].ToString();
            var imageUrl = form["imageUrl"].ToString();

            await store.AddAsync(new WatchedItem(
                userId,
                title,
                string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
                string.IsNullOrWhiteSpace(genre) ? "Unknown" : genre,
                rating,
                kind,
                sourceUrl,
                DateTime.UtcNow));

            var returnUrl = form["returnUrl"].ToString();
            return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/profile" : returnUrl);
        });

        app.MapPost("/watched/remove", async (HttpContext context, WatchedListStore store) =>
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect("/account");
            }

            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sourceUrl = context.Request.Form["sourceUrl"].ToString();
            await store.RemoveAsync(userId, sourceUrl);

            var returnUrl = context.Request.Form["returnUrl"].ToString();
            return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/profile" : returnUrl);
        });
    }
}
