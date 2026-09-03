using ShowRank.Models;

namespace ShowRank.Data;

public static class SampleShows
{
    public static readonly List<Show> TopRankings = new()
    {
        new Show(1, "Nightfall Station", "Sci-Fi Thriller", "A crew aboard a derelict orbital station race to uncover who—or what—is picking them off one by one.", 4.8, 12430, "#5b3cc4", "N"),
        new Show(2, "The Last Harvest", "Drama", "Three siblings return to their family farm and confront the secret that split them apart a decade ago.", 4.7, 9810, "#c4573c", "L"),
        new Show(3, "Paper Crowns", "Historical Drama", "A sweeping saga of rival merchant families vying for power in a fractured medieval kingdom.", 4.6, 15220, "#3c8dc4", "P"),
        new Show(4, "Static Heart", "Romance", "Two rival radio hosts fall for each other on air while pretending they can't stand one another.", 4.5, 7640, "#c43c8d", "S"),
        new Show(5, "Blacktop Legends", "Sports Comedy", "A washed-up street racer mentors a ragtag crew for one last shot at the underground championship.", 4.4, 6120, "#3cc46b", "B"),
    };

    public static readonly List<Show> Suggestions = new()
    {
        new Show(0, "Hollow Pines", "Mystery", "A detective returns to her hometown to solve a disappearance eerily similar to her sister's decades earlier.", 4.6, 5310, "#8d3cc4", "H"),
        new Show(0, "The Quiet Machine", "Sci-Fi", "An engineer discovers the AI she built to help her father might be rewriting more than his memories.", 4.5, 4120, "#c4a03c", "Q"),
        new Show(0, "Salt & Neon", "Crime Drama", "A smuggler and a rookie customs agent form an uneasy alliance in a coastal city run by cartels.", 4.7, 8890, "#3cc4a0", "S"),
        new Show(0, "Ordinary Wonders", "Comedy", "A chaotic found-family sitcom about the tenants of the world's worst-run apartment building.", 4.3, 3980, "#c43c3c", "O"),
        new Show(0, "Ember & Ash", "Fantasy", "Two rival fire-wielders must combine their forbidden magic to stop a kingdom from freezing over.", 4.6, 6740, "#c4823c", "E"),
        new Show(0, "The Long Way Round", "Documentary", "A decade-spanning look at four friends who set out to walk the coastline of an entire continent.", 4.4, 2210, "#3c6bc4", "L"),
    };
}
