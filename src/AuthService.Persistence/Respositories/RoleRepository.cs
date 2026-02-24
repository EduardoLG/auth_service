using AuthService.Domain.Entitis;
using AuthService.Domain.Interface;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    // Cambiado 'ByName' a 'BYName' para que coincida con tu interfaz
    public async Task<Role?> GetBYNameAsync(string roleName)
    {
        return await context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task<int> CountUsersInRoleAsync(string roleId)
    {
        return await context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .CountAsync();
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName)
    {
        var users = await context.UserRoles
            .Where(ur => ur.Role.Name == roleName)
            .Select(ur => ur.User)
                .Include(u => u.UserProfile)
                .Include(u => u.UserEmail)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.PasswordReset)
            .ToListAsync();

        return users.AsReadOnly();
    }

    // Cambiado 'Names' (plural) a 'Name' (singular) para que coincida con tu interfaz
    public async Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId)
    {
        var roles = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        return roles.AsReadOnly();
    }
}