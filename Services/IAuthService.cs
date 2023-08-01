using LearnIndentityAndAuthorization.Controllers.Responses.Auth;
using LearnIndentityAndAuthorization.Controllers.Responses.Users;
using LearnIndentityAndAuthorization.Models;

namespace LearnIndentityAndAuthorization.Services;

public interface IAuthService
{
    Task<bool> RegisterUser(ApplicationUser data);
    Task<List<ApplicationUser>> GetAllUser();
    Task<ApplicationUser> GetCurrentUser();
    Task<LoginResponse> LoginUser(ApplicationUser data);
    Task<string> GenerateToken(ApplicationUser data, string secretKey, DateTime expiredTime);
    Task<AuthToken> GetToken(ApplicationUser userDb);
    Task<bool> SaveRefreshToken(ApplicationUser data, string token);
    Task<string> CheckRefreshToken();
    Task SaveToken(string token);
    Task LogoutUser();
    Task<IList<string>> GetRolesOfUser(ApplicationUser data);
    Task<LoginResponse> LoginGoogle(string email, string accessTokenGoogle);
    Task<GoogleUserInfo?> GetGoogleUserInfo(string accessToken);
    Task<bool> CheckUserViaEmail(string email);
}
