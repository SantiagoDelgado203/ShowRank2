using System.Security.Claims;
using ShowRank.Data;
using ShowRank.Models;

namespace ShowRank.Endpoints;

// Minimal API endpoints (not Blazor components) for the "Add to Watched List" /
// "Remove" show functionality in Search.razor and Profile.razor.

// They're plain HTTP POST endpoints rather than Blazor EditForm handlers to avoid the
// problem of opening a live connection for a button click. A live connection must be
// opened because Blazor is a server held connection, meaning you would have to open
// a websocket connection on the browser. 

// Giving each one a unique Blazor FormName would be far more boilerplate than a couple of hidden
// <input> fields posted to a normal <form>. MapWatchedEndpoints() is called once from Program.cs to
// register both routes so it's much easier. 

// Search's GET doesn't need a separate file because the Razor page is the handler.

// /watched/add and /watched/remove don't render anything — they're pure actions.
public static class WatchedEndpoints
{
    public static void MapWatchedEndpoints(this WebApplication app)
    {
        app.MapPost("/watched/add", async (HttpContext context, WatchedListStore store) =>
        {
            // Defensive check: the UI only renders this form for signed-in users, but the
            // endpoint itself can still be hit directly, so it must not trust the caller.
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect("/account");
            }

            // error checking the title and source url. 
            var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var form = context.Request.Form;
            var title = form["title"].ToString();
            var sourceUrl = form["sourceUrl"].ToString();
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(sourceUrl))
            {
                // Malformed request (e.g. hand-crafted, missing hidden fields) — nothing sane to save.
                return Results.BadRequest();
            }

            // Everything else is optional/best-effort: fall back to sane defaults rather
            // than failing the whole request over a missing rating or genre.
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

            // Send the user back to wherever they clicked "Add" from (Search.razor sets
            // returnUrl to the current search query string); fall back to the profile page
            // if that's missing.
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
