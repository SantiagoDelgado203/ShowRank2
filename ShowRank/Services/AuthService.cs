//Identity for hashes
using Microsoft.AspNetCore.Identity;
using ShowRank.Data;
using ShowRank.Models;

namespace ShowRank.Services;

public class AuthService(UserStore userStore)
{
    //built in hasher
    private readonly PasswordHasher<User> _hasher = new();

    public async Task<(bool Success, string? Error, User? User)> RegisterAsync(SignUpModel model)
    {
        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        //Stops to wait for FindByEmailAsync
        if (await userStore.FindByEmailAsync(normalizedEmail) is not null)
        {
            return (false, "An account with that email already exists.", null);
        }

        var user = new User
        {
            DisplayName = model.DisplayName.Trim(),
            Email = normalizedEmail,
        };
        //stores the hash
        user.PasswordHash = _hasher.HashPassword(user, model.Password);

        //Adds persisting user via UserStore
        //UserStore is a data acces layer that talks to a persistence provider
        await userStore.AddAsync(user);
        return (true, null, user);
    }

    public async Task<(bool Success, string? Error, User? User)> ValidateCredentialsAsync(SignInModel model)
    {
        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var user = await userStore.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            return (false, "Invalid email or password.", null);
        }
        //Verify hashes
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return (false, "Invalid email or password.", null);
        }

        return (true, null, user);
    }
}
