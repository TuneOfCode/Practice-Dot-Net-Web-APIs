using System.Security.Claims;
using LearnIndentityAndAuthorization.Models;
using Microsoft.AspNetCore.Identity;

namespace LearnIndentityAndAuthorization.Repositories;

public interface IUserRepository
{
    Task<List<ApplicationUser>> GetAllUserAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
    Task<ApplicationUser?> GetUserByEmaiOrUserNameAsync(string email, string userName);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser data);
    Task<IdentityResult> CreateUserAsync(ApplicationUser data);
    Task<IdentityResult> DeleteUserAsync(ApplicationUser data);
    Task<bool> CheckPasswordAsync(ApplicationUser data, string password);
    Task<IList<string>> GetRolesOfUserAsync(ApplicationUser data);
    Task<IdentityResult> AddToRoleForUserAsync(ApplicationUser data, string roleName);
    Task<List<Claim>> GetClaimOfUserAsync(string username);
    Task<IdentityResult> AddClaimAsync(ApplicationUser data, string claimnType, string claimName);
    Task<ExternalLoginInfo?> GetExternalLoginInfoAsync();
}
