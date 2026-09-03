using Microsoft.AspNetCore.Authentication.Cookies;
using ShowRank.Components;
using ShowRank.Data;
using ShowRank.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Lets components run on server
//UI updates pushed to browser(Over SignalR Circuit not static rendering)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<UserStore>();
//Each user gets their own(scope is per circuit(per connecetd client)
builder.Services.AddScoped<AuthService>();

//Adding our services
builder.Services.AddHttpClient<AniListService>();
builder.Services.AddHttpClient<TvMazeService>();
builder.Services.AddScoped<SearchService>();

//Cookie authentication
//Redirects failures to /account not /account/login
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

//Reads cookie and enforces checks
app.UseAuthentication();
app.UseAuthorization();

//CSRF protection
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
