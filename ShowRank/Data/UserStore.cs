using System.Text.Json;
using ShowRank.Models;

namespace ShowRank.Data;

// Called by AuthService.
// File-backed user store (App_Data/users.json) — no database
public class UserStore
{
    private readonly string _filePath;

    // Lock that ensures multiple sign-ups can't overwrite each other's changes.
    private readonly SemaphoreSlim _lock = new(1, 1);

    // Initialize folder path.
    public UserStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "users.json");
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var users = await ReadAllAsync();
        return users.FirstOrDefault(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        // Lock to prevent race conditions. 
        await _lock.WaitAsync();
        try
        {
            // Read file to see if user exists. 
            var users = await ReadAllAsync();
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            await WriteAllAsync(users);
            return user;
        }
        finally
        {
            _lock.Release();
        }
    }

    // Reads skip the lock (only writes need to serialize)
    private async Task<List<User>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<User>>(stream) ?? [];
    }

    private async Task WriteAllAsync(List<User> users)
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, users, new JsonSerializerOptions { WriteIndented = true });
    }
}
