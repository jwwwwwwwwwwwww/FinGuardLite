using FinGuardLite.Data;
using FinGuardLite.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<FinGuardDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FinGuardDb")));

builder.Services.AddScoped<RiskScoringService>();

// Required for session-based login
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

await FinGuardDbSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

// Role-based page protection
app.Use(async (context, next) =>
{
    string path = context.Request.Path.Value?.ToLower() ?? "";

    bool isPublicPage =
        path.StartsWith("/login") ||
        path.StartsWith("/accessdenied") ||
        path.StartsWith("/css") ||
        path.StartsWith("/js") ||
        path.StartsWith("/lib") ||
        path.StartsWith("/favicon");

    string? username = context.Session.GetString("Username");
    string? role = context.Session.GetString("Role");

    bool isLoggedIn = !string.IsNullOrEmpty(username);

    if (!isPublicPage && !isLoggedIn)
    {
        context.Response.Redirect("/Login");
        return;
    }

    if (isLoggedIn)
    {
        bool isAllowed = role switch
        {
            "Admin" => true,

            "Risk Analyst" =>
                path == "/" ||
                path.StartsWith("/index") ||
                path.StartsWith("/about") ||
                path.StartsWith("/customers") ||
                path.StartsWith("/transactions") ||
                path.StartsWith("/riskalerts") ||
                path.StartsWith("/logout"),

            "Compliance Officer" =>
                path == "/" ||
                path.StartsWith("/index") ||
                path.StartsWith("/about") ||
                path.StartsWith("/auditlogs") ||
                path.StartsWith("/logout"),

            _ => false
        };

        if (!isAllowed && !isPublicPage)
        {
            context.Response.Redirect("/AccessDenied");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();