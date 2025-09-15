using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Services; // <-- ensure this is present

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProfileDetailsService, ProfileDetailsService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IStudyService, StudyService>();
builder.Services.AddScoped<ISosService, SosService>();

// Register DbContext using connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VConnect")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.LogoutPath = "/Account/Logout";
        o.AccessDeniedPath = "/Account/Login";
        o.Cookie.HttpOnly = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
