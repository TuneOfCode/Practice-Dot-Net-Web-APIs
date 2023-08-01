using Microsoft.AspNetCore.Identity;

namespace LearnIndentityAndAuthorization.Repositories;

public interface IRoleRepository
{
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> DeleteRoleAsync(string roleName);
    Task<List<IdentityRole>> GetAllRoleAsync();
    Task<IdentityRole?> GetRoleByNameAsync(string roleName);
    Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
}
