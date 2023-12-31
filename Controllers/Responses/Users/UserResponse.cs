namespace LearnIndentityAndAuthorization.Controllers.Responses.Users;
public class UserResponse
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? Avatar { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberConfirmed { get; set; }
    public Object? Roles { get; set; }
}