using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnIndentityAndAuthorization.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    /// <summary>
    /// Lấy tất cả role
    /// </summary>
    /// <returns></returns>
    public async Task<List<IdentityRole>> GetAllRoleAsync()
    {
        var roles = await _roleManager!.Roles.ToListAsync();
        return roles;
    }
    /// <summary>
    /// Lấy role theo tên
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityRole?> GetRoleByNameAsync(string roleName)
    {
        // return await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        return await _roleManager.FindByNameAsync(roleName);
    }
    /// <summary>
    /// Tạo mới role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        return result;
    }
    /// <summary>
    /// Cập nhật role
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
    {
        var result = await _roleManager.UpdateAsync(role);
        return result;
    }
    /// <summary>
    /// Xóa role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        var result = await _roleManager.DeleteAsync(role ?? null!);
        return result;
    }
}