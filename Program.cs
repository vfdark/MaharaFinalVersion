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
app.Urls.Add("https://localhost:7054");
app.Urls.Add("http://localhost:5246");
// ===============================
// Middleware
// ===============================
app.UseHttpsRedirection(); // must be HTTPS
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

    // Admin role
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Admin user
    var adminEmail = "admin@mahara.com";
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
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Dummy students
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

            await userManager.CreateAsync(user, "Password123!");
        }
    }

    await CreateDummyUser("dummy1", "فاطمة الزهراء", "dummy1@example.com", 2450, 24, new List<string>{"Python","ML","Data Analysis"});
    await CreateDummyUser("dummy2", "عبدالله العتيبي", "dummy2@example.com", 2180, 22, new List<string>{"JavaScript","React","Node.js"});
    await CreateDummyUser("dummy3", "مريم الشمري", "dummy3@example.com", 1950, 20, new List<string>{"UI/UX Design","Figma","Adobe XD"});
}

app.Run();
