using GraduationQRSystem.Data;
using GraduationQRSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure port for Render deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Get connection string from appsettings.json or environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If not found, try DATABASE_URL environment variable (Render format)
if (string.IsNullOrEmpty(connectionString))
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Convert DATABASE_URL (postgresql://user:password@host:port/dbname) to Npgsql format
        var uri = new Uri(databaseUrl);
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
    else
    {
        // Fallback for local development
        connectionString = "Host=localhost;Port=5432;Database=graduation;Username=postgres;Password=password";
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add simple session for login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Seed data if database is empty
    if (!db.Seniors.Any())
    {
        var testSenior = new Senior
        {
            Name = "Senior Test",
            NumberOfGuests = 1,
            PhoneNumber = "01012345678",
            Guests = new List<Guest>
            {
                new Guest { Name = "Guest Test", PhoneNumber = "01098765432" }
            }
        };
        db.Seniors.Add(testSenior);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "seniors",
    pattern: "Seniors",
    defaults: new { controller = "Seniors", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();


