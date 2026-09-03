using Microsoft.AspNetCore.Authentication.Cookies;
using ShowRank.Components;
using ShowRank.Data;
using ShowRank.Endpoints;
using ShowRank.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<UserStore>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<WatchedListStore>();

builder.Services.AddHttpClient<AniListService>();
builder.Services.AddHttpClient<TvMazeService>();
builder.Services.AddScoped<SearchService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account";
        options.AccessDeniedPath = "/account";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapWatchedEndpoints();

app.Run();
