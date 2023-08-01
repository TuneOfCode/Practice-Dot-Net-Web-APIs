// Seed/ApplicationDbContextSeeder.cs
using LearnIndentityAndAuthorization.Databases;
using LearnIndentityAndAuthorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        if (!context.Users.Any() && !context.Roles.Any() && context.Posts != null && !context.Posts.Any())
        {
            await SeedUsers(context);
            await SeedRoles(context);
            await SeedPosts(context);
        }
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        if (!context.Users.Any())
        {
            var userManager = context.GetService<UserManager<ApplicationUser>>();

            var superAdmin = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                Name = "Super Admin",
                Avatar = ""
            };

            await userManager.CreateAsync(superAdmin, "Password123!");

            var roleManager = context.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }
    }

    private static async Task SeedRoles(ApplicationDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roleManager = context.GetService<RoleManager<IdentityRole>>();

            var roles = new List<IdentityRole>
            {
                new IdentityRole("Admin"),
                new IdentityRole("User")
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedPosts(ApplicationDbContext context)
    {
        if (context.Posts != null && !context.Posts.Any())
        {
            var userManager = context.GetService<UserManager<ApplicationUser>>();

            var superAdmin = await userManager.FindByEmailAsync("admin@example.com");

            var posts = new List<Post>
            {
                new Post
                {
                    Title = "Post 1",
                    Text = "Lorem ipsum dolor sit amet",
                    ImageURL = "https://example.com/image1.jpg",
                    Author = superAdmin
                },
                new Post
                {
                    Title = "Post 2",
                    Text = "Consectetur adipiscing elit",
                    ImageURL = "https://example.com/image2.jpg",
                    Author = superAdmin
                },
                // Add more posts as needed
            };

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }
    }
}
