using System.Text.Json;
using ShowRank.Models;

namespace ShowRank.Data;

// Called by search.razor, profile.razor and watchedendpoints

// File-backed watched-list store (App_Data/watched.json) — all users' items in one flat
// file, filtered by UserId 
public class WatchedListStore
{
    private readonly string _filePath;

    // another lock to prevent race conditions.
    private readonly SemaphoreSlim _lock = new(1, 1);

    public WatchedListStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "watched.json");
    }

    public async Task<List<WatchedItem>> GetForUserAsync(int userId)
    {
        var all = await ReadAllAsync();
        return all
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.AddedAtUtc)
            .ToList();
    }

    public async Task AddAsync(WatchedItem item)
    {
        await _lock.WaitAsync();
        try
        {
            var all = await ReadAllAsync();
            // Same show already saved for this user — no-op instead of a duplicate entry.
            if (all.Any(w => w.UserId == item.UserId && w.SourceUrl == item.SourceUrl))
            {
                return;
            }

            all.Add(item);
            await WriteAllAsync(all);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveAsync(int userId, string sourceUrl)
    {
        await _lock.WaitAsync();
        try
        {
            var all = await ReadAllAsync();
            all.RemoveAll(w => w.UserId == userId && w.SourceUrl == sourceUrl);
            await WriteAllAsync(all);
        }
        finally
        {
            _lock.Release();
        }
    }

    // Reads skip the lock (only writes need to serialize)
    private async Task<List<WatchedItem>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<WatchedItem>>(stream) ?? [];
    }

    private async Task WriteAllAsync(List<WatchedItem> items)
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, new JsonSerializerOptions { WriteIndented = true });
    }
}
