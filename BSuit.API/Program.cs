using BSuit.API.Areas.Admin.Services;
using BSuit.API.Infrastructure.Constants;
using BSuit.API.Infrastructure.Services;
using BSuit.API.Models;
using BSuit.Contracts.Services;
using BSuit.Core;
using BSuit.Core.Data;
using BSuit.Core.Seed;
using BSuit.HR;
using BSuit.Identity;
using BSuit.Identity.Seed;
using BSuit.Infrastructure;
using BSuit.SalesCRM;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();


// ✅ REGISTER HERE (BEFORE Build)
builder.Services.AddBSuitsIdentity(builder.Configuration);
builder.Services.AddBSuitsCore(builder.Configuration);
builder.Services.AddBSuitsHR(builder.Configuration);
builder.Services.AddBSuitsSalesCRM(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddHostedService<LeadAssignmentBackgroundService>();


//Enable Razor Pages
builder.Services.AddRazorPages();

//cookie configuration for session-based login:
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/NoAccess";

    // Important
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    // Cookie removed when browser closes
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;

    // VERY IMPORTANT
    options.Cookie.MaxAge = null;

    options.SlidingExpiration = false;

    //Disable redirectURL fix
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(POLICIES.Superadmin_Admin_TenantsOnly, policy =>
        policy.RequireRole(
            nameof(BSuit.Identity.AppRoles.ADMIN),
            nameof(BSuit.Identity.AppRoles.SUPERADMIN),
            nameof(BSuit.Identity.AppRoles.TENANT)
        ));

    options.AddPolicy(POLICIES.SuperadminOnly, policy =>
       policy.RequireRole(
           nameof(BSuit.Identity.AppRoles.SUPERADMIN)
       ));


});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<CoreDbContext>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            // ✅ Seed Core Data
            await CoreDataSeeder.SeedAsync(dbContext);

            // ✅ Get Tenant safely
            var tenant = await dbContext.Tenants
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync();

            if (tenant != null)
            {
                // ✅ Seed Identity
                await IdentitySeeder.SeedAsync(services, tenant.Id);
            }
        }
    }
    catch (Exception ex)
    {
        // ❗ Do NOT crash app
        Console.WriteLine("⚠️ Database not available. Skipping seeding.");
        Console.WriteLine(ex.Message);
    }
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/Admin");
    return Task.CompletedTask;
});

app.UseSession();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ Order is correct
app.UseAuthentication();
app.UseAuthorization();

//Execute here so that we can get values once iser is logged-in
app.UseMiddleware<TenantMiddleware>();



app.MapRazorPages(); // Identity UI-Razor

app.MapStaticAssets();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();