using System.Security.Claims;
using LearnIndentityAndAuthorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnIndentityAndAuthorization.Repositories;
/// <summary>
/// Lớp kho lưu trữ của người dùng
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserRepository
    (
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    /// <summary>
    /// Lấy tất cả người dùng
    /// </summary>
    /// <returns></returns>
    public async Task<List<ApplicationUser>> GetAllUserAsync()
    {
        var users = await _userManager!.Users.ToListAsync();
        // // loại bỏ thông tin mật khẩu
        // users.ForEach(u => u.PasswordHash = null!);
        return users;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        var user = await _userManager!.FindByIdAsync(id);

        return user;
    }
    /// <summary>
    /// Lấy thông tin người dùng theo tên đăng nhập
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName)
    {
        var user = await _userManager!.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        return user;
    }
    /// <summary>
    /// Lấy thông tin người dùng theo email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager!.Users.FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }
    /// <summary>
    /// Lấy thông tin người dùng theo email hoặc tên đăng nhập
    /// </summary>
    /// <param name="email"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByEmaiOrUserNameAsync(string email, string userName)
    {
        var user = await _userManager!.Users.FirstOrDefaultAsync(u => u.Email == email || u.UserName == userName);

        return user;
    }
    /// <summary>
    /// Tạo mới người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateUserAsync(ApplicationUser data)
    {
        return await _userManager.CreateAsync(data, data.PasswordHash ?? null!);
    }
    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser data)
    {
        return await _userManager.UpdateAsync(data);
    }
    /// <summary>
    /// Xóa người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser data)
    {
        return await _userManager.DeleteAsync(data);
    }
    /// <summary>
    /// Kiểm tra mật khẩu người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<bool> CheckPasswordAsync(ApplicationUser data, string password)
    {
        return await _userManager.CheckPasswordAsync(data, password);
    }
    /// <summary>
    /// Lấy tất cả các vai trò của người dùng
    /// </summary>
    /// <param name="data"></param>
    /// /// <returns></returns>
    public async Task<IList<string>> GetRolesOfUserAsync(ApplicationUser data)
    {
        return await _userManager.GetRolesAsync(data);
    }
    /// <summary>
    /// Thêm vai trò cho người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddToRoleForUserAsync(ApplicationUser data, string roleName)
    {
        return await _userManager.AddToRoleAsync(data, roleName);
    }
    /// <summary>
    /// Thêm quyền cho người dùng
    /// </summary>
    /// <param name="data"></param>
    /// <param name="permissionType"></param>
    /// <param name="permissionName"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddClaimAsync(ApplicationUser data, string claimnType, string claimName)
    {
        return await _userManager.AddClaimAsync(data, new Claim(claimnType, claimName));
    }
    /// <summary>
    /// Lấy tất cả các quyền của người dùng
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<List<Claim>> GetClaimOfUserAsync(string username)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user is null)
        {
            return new List<Claim>();
        }

        var claims = await _userManager.GetClaimsAsync(user);
        return claims.ToList();
    }
    /// <summary>
    /// Lấy thông tin đăng nhập bên ngoài như google, facebook
    /// </summary>
    /// <returns></returns>
    public async Task<ExternalLoginInfo?> GetExternalLoginInfoAsync()
    {
        return await _signInManager.GetExternalLoginInfoAsync();
    }
}