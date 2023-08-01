using LearnIndentityAndAuthorization.Models;

namespace LearnIndentityAndAuthorization.Controllers.Responses.Users;
public class LoginResponse
{
    public ApplicationUser? Data { get; set; }
    public string? AccessToken { get; set; }
    public string? IssuedTime { get; set; }
    public string? ExpiredTime { get; set; }
    public string? RefreshToken { get; set; }
}