using System.Text.Json.Serialization;
using LearnIndentityAndAuthorization.Configs;

namespace LearnIndentityAndAuthorization.Models;

public class AuthToken
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }
    [JsonPropertyName("issuedTime")]
    public string? IssuedTime { get; set; }
    [JsonPropertyName("expiredTime")]
    public string? ExpiredTime { get; set; }
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}