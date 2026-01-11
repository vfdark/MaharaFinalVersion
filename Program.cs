using MaharaFinalVersion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaharaFinalVersion.Data;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Database
// ===============================
builder.Services.AddDbContext<Mahara2DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===============================
// Identity
// ===============================
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<Mahara2DbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ===============================
// Middleware
// ===============================
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ===============================
// ROUTES
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===============================
// ✅ SEED ADMIN USER & ROLE
// ===============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // 1️⃣ Create Admin role if not exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2️⃣ Admin user data
    var adminEmail = "admin@mahara.com";
    var adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = "AdminRahaf",
            Email = adminEmail,
            FullName = "مشرف النظام",
            AccessFailedCount = 0,   // Required
            IsInstructor = false,    // Required
            EmailConfirmed = true     
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            // Optional: log errors if creation fails
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin: {error.Description}");
            }
        }
    }
}

app.Run();
