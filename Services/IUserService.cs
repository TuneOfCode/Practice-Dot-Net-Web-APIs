using LearnIndentityAndAuthorization.Models;
using Microsoft.AspNetCore.Identity;

namespace LearnIndentityAndAuthorization.Services;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<bool> CreateUserAsync(ApplicationUser newUser);
    Task<bool> UpdateUserAsync(ApplicationUser data, List<IFormFile>? avatar);
    Task<bool> DeleteUserAsync(ApplicationUser deleteUser);
}
