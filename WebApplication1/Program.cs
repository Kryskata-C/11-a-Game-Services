using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Services;
using Microsoft.Extensions.DependencyInjection; // For CreateScope, GetRequiredService
using Microsoft.Extensions.Logging; // For ILogger
using System; // For TimeSpan, InvalidOperationException, Exception
using System.Threading.Tasks; // For Task
using WebApplication1.Data;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed roles and admin user START
// This section should be placed after app has been built (var app = builder.Build();)
// and before the request pipeline configuration (app.UseExceptionHandler, etc.) or app.Run().
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>(); // Using ILogger<Program> for context

        // Call your seeding method
        // Ensure the SeedRolesAndAdminUser method is defined (see definition below or in a separate class)
        await SeedRolesAndAdminUser(userManager, roleManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        // Optionally, rethrow or handle critical seeding failure
    }
}
// Seed roles and admin user END

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Definition of the SeedRolesAndAdminUser method
// You can place this at the end of your Program.cs file or in a separate static helper class.
async Task SeedRolesAndAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<Program> logger)
{
    string adminRoleName = "Admin";
    // IMPORTANT: Set your desired admin username/email and password here
    string adminEmail = "admin@gmail.com";
    string adminPassword = "Admin1"; // Change this to a strong, unique password!

    logger.LogInformation("Attempting to seed roles and admin user.");

    // Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
        logger.LogInformation($"Role '{adminRoleName}' created.");
    }
    else
    {
        logger.LogInformation($"Role '{adminRoleName}' already exists.");
    }

    // Ensure a default admin user exists
    var adminUser = await userManager.FindByNameAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            logger.LogInformation($"Admin user '{adminEmail}' created successfully.");
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
            logger.LogInformation($"Admin user '{adminEmail}' assigned to '{adminRoleName}' role.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                logger.LogError($"Error creating admin user '{adminEmail}': {error.Description}");
            }
        }
    }
    else
    {
        logger.LogInformation($"Admin user '{adminEmail}' already exists.");
        // Optionally, ensure the existing user is in the Admin role
        if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
            logger.LogInformation($"Existing admin user '{adminEmail}' assigned to '{adminRoleName}' role.");
        }
    }
}