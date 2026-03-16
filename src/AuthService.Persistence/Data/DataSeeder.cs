using AuthService.Domain.Entitis;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // 1. SEED DE ROLES
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Id = Guid.NewGuid().ToString("N").Substring(0, 16), Name = "ADMIN", Description = "Full system access" },
                new() { Id = Guid.NewGuid().ToString("N").Substring(0, 16), Name = "USER", Description = "Standard user access" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 2. SEED DE USUARIO ADMIN
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "ADMIN");
            if (adminRole != null)
            {
                // Generamos un solo ID de string (16 caracteres max)
                var userId = Guid.NewGuid().ToString("N").Substring(0, 16);
                
                var adminUser = new User
                {
                    Id = userId,
                    Name = "Admin",
                    Surname = "User",
                    Username = "admin",
                    Email = "admin@ksports.local",
                    Password = new string('x', 255), // Cumple MaxLength(255)
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 16), // CORREGIDO: Id Mayúscula
                        UserId = userId,
                        ProfilePictureUrl = "https://cdn.com/admin.jpg",
                        Bio = "System Administrator",
                        DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    UserEmail = new UserEmail
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 16), // CORREGIDO: Id Mayúscula
                        UserId = userId,
                        EmailVerified = true,
                        EmailVerificationToken = "initial-token",
                        EmailVerifiedTokenExpire = DateTime.UtcNow.AddYears(1) 
                    }
                };

                adminUser.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 16),
                        UserId = userId,
                        RoleId = adminRole.Id
                    }
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}