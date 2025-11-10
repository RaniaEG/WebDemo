using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDemo;
using WebDemo.Data;
using WebDemo.Middleware;

var builder = WebApplication.CreateBuilder(args);

// hide server header
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});


// MVC and CSRF configurations
builder.Services.AddControllersWithViews();
builder.Services.Configure<MvcOptions>(StartupConfig.ConfigureMvcOptions);
builder.Services.AddAntiforgery(StartupConfig.ConfigureAntiforgery);

// DbContext 
builder.Services.AddDbContext<AppDbContext>(StartupConfig.ConfigureDbContext);

var app = builder.Build();

// Create DB on first run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Security headers via dedicated middleware 
app.UseMiddleware<SecureHeadersMiddleware>();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Messages}/{action=Index}/{id?}"
);

app.Run();

namespace WebDemo
{
    public static class StartupConfig
    {
        public static void ConfigureMvcOptions(MvcOptions options)
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        }

        public static void ConfigureAntiforgery(AntiforgeryOptions options)
        {
            options.HeaderName = "X-XSRF-TOKEN";
        }

        public static void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            // Using SQLite in the local app directory
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }
}
