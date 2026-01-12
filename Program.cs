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
    builder.WebHost.UseUrls("http://0.0.0.0:5246"); 

// ===============================
// Identity
// ===============================
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
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
            AccessFailedCount = 0,
            IsInstructor = false,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin: {error.Description}");
            }
        }
    }

    // ===============================
    // ✅ SEED DUMMY STUDENTS
    // ===============================
    async Task CreateDummyUser(string id, string fullName, string email, int points, int completedSessions, List<string> skills)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var user = new User
            {
                Id = id,
                UserName = email,
                Email = email,
                FullName = fullName,
                Points = points,
                CompletedSessions = completedSessions,
                Skills = skills,
                IsInstructor = false,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Password123!"); // default password
        }
    }

    await CreateDummyUser("dummy1", "فاطمة الزهراء", "dummy1@example.com", 2450, 24, new List<string>{"Python","ML","Data Analysis"});
    await CreateDummyUser("dummy2", "عبدالله العتيبي", "dummy2@example.com", 2180, 22, new List<string>{"JavaScript","React","Node.js"});
    await CreateDummyUser("dummy3", "مريم الشمري", "dummy3@example.com", 1950, 20, new List<string>{"UI/UX Design","Figma","Adobe XD"});
}

app.Run();
