using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Controllers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace LearnIndentityAndAuthorization.Controllers.Filters;

/// <summary>
/// Lớp thuộc tính uỷ quyền thông qua JWT
/// </summary>
public class JwtAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter, IAllowAnonymous
{

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Kiểm tra nếu route hiện tại có thuộc tính [AllowAnonymous] thì bỏ qua
        if (context.ActionDescriptor.EndpointMetadata.Any(metadata => metadata.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return;
        }

        // Lấy token từ header Authorization hoặc trong cookie
        string? accessToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? context.HttpContext.Request.Cookies[Constants.AUTH_COOKIE_NAME];

        // Kiểm tra token có tồn tại hay không
        if (string.IsNullOrEmpty(accessToken))
        {
            context.Result = new ErrorResponse()
            {
                Title = "Không được uỷ quyền",
                Status = StatusCodes.Status401Unauthorized,
                Type = ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)
            };
            return;
        }

        // Kiểm tra và xác thực token
        var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = configuration!.GetSection("Jwt:Key").Value;
        if (string.IsNullOrEmpty(key))
        {
            throw new Exception("Khoá bảo mật không tìm thấy");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            // kiểm tra token có hết hạn hay không
            var token = tokenHandler.ReadJwtToken(accessToken);
            var exp = token.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
            if (string.IsNullOrEmpty(exp))
            {
                context.Result = new ErrorResponse()
                {
                    Title = "Không tìm thấy thời gian hết hạn",
                    Status = StatusCodes.Status401Unauthorized,
                    Type = ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)
                };
                return;
            }

            long.TryParse(exp, out long expiryTime);
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (currentUnixTime >= expiryTime)
            {
                context.Result = new ErrorResponse()
                {
                    Title = "Token đã hết hạn sử dụng",
                    Status = StatusCodes.Status401Unauthorized,
                    Type = ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)
                };
                return;
            }

            // Xác thực token
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken validatedToken);
            if (claimsPrincipal is null || validatedToken is null)
            {
                context.Result = new ErrorResponse()
                {
                    Title = "Token không hợp lệ",
                    Status = StatusCodes.Status401Unauthorized,
                    Type = ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)
                };
                return;
            }

            // Kiểm tra và áp dụng vai trò
            if (!string.IsNullOrEmpty(Roles))
            {
                var roles = Roles.Split(",") ?? Roles.Split(", ");
                if (roles.Length > 0)
                {
                    ref string roleActive = ref roles[0];
                    foreach (var role in roles)
                    {
                        // kiểm tra xem role có tồn tại trong token hay không
                        var _role = role.Trim();
                        var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;

                        if (roleClaim == _role)
                        {
                            roleActive = _role;
                            break;
                        }
                    }
                    var roleDb = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
                    if (roleActive != roleDb)
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Result = new ErrorResponse()
                        {
                            Title = $"Không có quyền truy cập vào tài nguyên này với vai trò {roleActive} vì vai trò của bạn là {roleDb}",
                            Status = StatusCodes.Status403Forbidden,
                            Type = ReasonPhrases.GetReasonPhrase(StatusCodes.Status403Forbidden)
                        };
                        return;
                    }
                }
            }

            // kiểm tra xác thực identity
            if (!claimsPrincipal.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Thiết lập người dùng xác thực thành công
            var userName = claimsPrincipal.Identity.Name;
            context.HttpContext.Items["UserName"] = userName;
        }
        catch (Exception ex)
        {
            // Xác thực token không thành công
            context.Result = new ErrorResponse()
            {
                Title = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            };
            return;
        }
    }
}
