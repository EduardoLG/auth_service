using AuthService.Domain.Entitis;
namespace AuthService.Domain.Interface;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<int> CountUsersInRoleAsync(String roleId);
    Task<IReadOnlyList<User>> GetUsersByRoleAsync(String roleId);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string userId); 
}