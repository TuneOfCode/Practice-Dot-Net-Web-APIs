using System.IdentityModel.Tokens.Jwt;
using LearnIndentityAndAuthorization.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Repositories;
using Microsoft.AspNetCore.Authorization;
using LearnIndentityAndAuthorization.Controllers.Responses.Users;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using LearnIndentityAndAuthorization.Controllers.Responses.Auth;

namespace LearnIndentityAndAuthorization.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContext;


    public AuthService
    (
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        IHttpContextAccessor httpContext
    )
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
        _httpContext = httpContext;
    }
    /// <summary>
    /// Lấy tất cả tài khoản
    /// </summary>
    /// <returns></returns>
    public async Task<List<ApplicationUser>> GetAllUser()
    {
        var users = await _userRepository.GetAllUserAsync();
        if (users is null)
        {
            throw new Exception("Không tìm thấy dữ liệu");
        }
        return users;
    }
    /// <summary>
    /// Đăng ký tài khoản
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<bool> RegisterUser(ApplicationUser data)
    {
        // kiểm tra đầu vào dữ liệu
        if (data is null)
        {
            throw new Exception("Không tìm thấy dữ liệu");
        }

        // kiểm tra email đã xuất hiện trước đó
        // var checkEmail = await _userManager!.FindByEmailAsync(data.Email ?? null!);
        var checkEmail = await _userRepository.GetUserByEmailAsync(data.Email ?? null!);
        if (checkEmail != null)
        {
            throw new Exception("Email đã tồn tại");
        }

        // kiểm tra username đã xuất hiện trước đó
        // var checkUserName = await _userManager!.Users.FirstOrDefaultAsync(u => u.UserName == data.UserName);
        var checkUserName = await _userRepository.GetUserByUserNameAsync(data.UserName ?? null!);
        if (checkUserName != null)
        {
            throw new Exception("Tên đăng nhập đã tồn tại");
        }

        // thực hiện quá trình tạo tài khoản
        // var newAccount = await _userManager.CreateAsync(data, data.PasswordHash ?? null!);
        var newAccount = await _userRepository.CreateUserAsync(data);
        if (!newAccount.Succeeded)
        {
            throw new Exception(newAccount.Errors.FirstOrDefault()?.Description);
        }

        // Thêm vai trò và quyền cho tài khoản
        // var role = await _roleManager.FindByNameAsync(Roles.USER);
        var role = await _roleRepository.GetRoleByNameAsync(Roles.USER);
        if (role is null)
        {
            var newRole = await _roleRepository.CreateRoleAsync(Roles.USER);

            if (!newRole.Succeeded)
            {
                throw new Exception(newRole.Errors.FirstOrDefault()?.Description);
            }
        }
        // thêm vai trò cho tài khoản
        // await _userManager.AddToRoleAsync(data, Roles.USER);
        await _userRepository.AddToRoleForUserAsync(data, Roles.USER);

        // thêm quyền cho tài khoản
        // await _userManager.AddClaimAsync(data, new Claim(ClaimTypes.Role, Roles.USER));
        await _userRepository.AddClaimAsync(data, ClaimTypes.Role, Roles.USER);

        return newAccount.Succeeded;
    }
    /// <summary>
    /// Đăng nhập tài khoản
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<LoginResponse> LoginUser(ApplicationUser data)
    {
        // kiểm tra đầu vào dữ liệu
        if (data is null)
        {
            throw new Exception("Không tìm thấy dữ liệu");
        }

        // kiểm tra username và mật khẩu
        // cho phép đăng nhập bằng email hoặc username
        // var userDb = await _userManager!.Users.FirstOrDefaultAsync(u => u.UserName == data.UserName || u.Email == data.UserName);
        var userDb = await _userRepository.GetUserByEmaiOrUserNameAsync(data.UserName ?? null!, data.UserName ?? null!);

        if (userDb is null || string.IsNullOrEmpty(userDb.PasswordHash))
        {
            throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác");
        }
        // kiểm tra mật khẩu
        // var checkPassword = await _userManager.CheckPasswordAsync(userDb, data.PasswordHash ?? null!);
        var checkPassword = await _userRepository.CheckPasswordAsync(userDb, data.PasswordHash ?? null!);
        if (userDb is null || !checkPassword)
        {
            throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác");
        }

        // Tạo 1 access token và refresh token
        var authToken = await GetToken(userDb);

        return new LoginResponse
        {
            Data = userDb,
            AccessToken = authToken.AccessToken,
            IssuedTime = authToken.IssuedTime,
            ExpiredTime = authToken.ExpiredTime,
            RefreshToken = authToken.RefreshToken,
        };
    }
    /// <summary>
    /// Tạo token
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<string> GenerateToken(ApplicationUser data, string secretKey, DateTime expiredTime)
    {
        // kiểm tra đầu vào dữ liệu
        if (data is null
            || string.IsNullOrEmpty(data.UserName))
        {
            throw new Exception("Không tìm thấy dữ liệu");
        }

        // kiểm tra khoá bảo mật
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("Khoá bảo mật không tìm thấy");
        }

        // tạo ra quyền
        // var rolesOfUser = await _userManager!.GetRolesAsync(data);
        var rolesOfUser = await _userRepository.GetRolesOfUserAsync(data);

        IEnumerable<Claim> claims = new List<Claim>(){
            new Claim(ClaimTypes.Email, data.UserName),
            new Claim(ClaimTypes.Name, data.UserName),
            new Claim(ClaimTypes.Role, rolesOfUser.FirstOrDefault() ?? Roles.USER)
        };

        // tạo ra khoá bảo mật
        if (_configuration is null)
        {
            throw new Exception("Có lỗi trong quá trình tạo token");
        }
        // var key = _configuration.GetSection("Jwt:Key").Value;
        // if (string.IsNullOrEmpty(secretKey))
        // {
        //     throw new Exception("Khoá bảo mật không tìm thấy");
        // }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        // tạo ra chữ ký xác thực
        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256Signature
        );
        var securityToken = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt:Issuer").Value,
            audience: _configuration.GetSection("Jwt:Audience").Value,
            claims: claims,
            expires: expiredTime,
            signingCredentials: signingCredentials
        );
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }
    /// <summary>
    /// Tạo ra token mới gồm access token và refresh token
    /// </summary>
    /// <param name="userDb"></param>
    /// <returns></returns>
    public async Task<AuthToken> GetToken(ApplicationUser userDb)
    {
        var secretKeyOfAccessToken = _configuration.GetSection("Jwt:Key").Value ?? null!;
        var secretKeyOfRefreshToken = _configuration.GetSection("Jwt:RefreshKey").Value ?? null!;

        var dateTimeNow = DateTime.Now;

        var expiredTimeOfAccessToken = dateTimeNow.AddMinutes(Constants.EXPRIED_TIME_OF_ACCESS_TOKEN);
        var accessToken = await GenerateToken(userDb, secretKeyOfAccessToken, expiredTimeOfAccessToken);

        var expiredTimeOfRefreshToken = dateTimeNow.AddMonths(Constants.EXPRIED_TIME_OF_REFRESH_TOKEN);
        var refreshToken = await GenerateToken(userDb, secretKeyOfRefreshToken, expiredTimeOfRefreshToken);

        // Lưu access token vào cookie 
        await SaveToken(accessToken);

        // Lưu refresh token vào database
        var isSaveRefreshToken = await SaveRefreshToken(userDb, refreshToken);
        if (!isSaveRefreshToken)
        {
            throw new Exception("Có lỗi trong quá trình đăng nhập");
        }

        return new AuthToken()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IssuedTime = dateTimeNow.ToString(Constants.FORMAT_DATETIME),
            ExpiredTime = expiredTimeOfAccessToken.ToString(Constants.FORMAT_DATETIME)
        };
    }

    public Task<string> CheckRefreshToken()
    {

        return null!;
    }
    /// <summary>
    /// Lưu token vào cookie
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task SaveToken(string token)
    {
        await Task.Run(() =>
        {
            _httpContext.HttpContext!.Response.Cookies.Append(Constants.AUTH_COOKIE_NAME, token, new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(Constants.EXPRIED_TIME_OF_ACCESS_TOKEN)
            });
        });
    }
    /// <summary>
    /// Đăng xuất tài khoản
    /// </summary>
    /// <returns></returns>
    public Task LogoutUser()
    {
        var accessToken = _httpContext.HttpContext!.Request.Headers["Authorization"].ToString().Split(" ").Last();
        return Task.Run(() =>
        {
            _httpContext.HttpContext!.Response.Cookies.Delete(Constants.AUTH_COOKIE_NAME);
            _httpContext.HttpContext!.Response.Headers.Remove("Authorization");
        });
    }

    public async Task<ApplicationUser> GetCurrentUser()
    {
        // lấy thông tin người dùng hiện tại ở trong HttpContext
        var claimsIdentity = _httpContext.HttpContext!.User.Identities.FirstOrDefault();
        var userName = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userName))
        {
            return null!;
        }

        // lấy thông tin tài khoản
        // var currentUser = await _userManager!.FindByNameAsync(userName);
        // if (currentUser is null)
        // {
        //     return null!;
        // }

        // // loại bỏ thông tin mật khẩu
        // currentUser.PasswordHash = null!;
        var currentUser = await _userRepository.GetUserByUserNameAsync(userName);
        if (currentUser is null)
        {
            throw new Exception("Không tìm thấy tài khoản");
        }

        return currentUser;
    }

    public async Task<bool> SaveRefreshToken(ApplicationUser data, string token)
    {
        // lấy thông tin người dùng hiện tại 
        var userName = data.UserName;
        if (string.IsNullOrEmpty(userName))
        {
            return false;
        }

        var currentUser = await _userRepository.GetUserByUserNameAsync(userName);
        if (currentUser is null)
        {
            throw new Exception("Không tìm thấy tài khoản");
        }

        // lưu token vào database
        currentUser.RefreshToken = token;
        var result = await _userRepository.UpdateUserAsync(currentUser);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.FirstOrDefault()?.Description);
        }

        return result.Succeeded;
    }

    // /// <summary>
    // /// Cập nhật tài khoản
    // /// </summary>
    // /// <param name="data"></param>
    // /// <returns></returns>
    // public async Task<bool> UpdateUser(ApplicationUser data)
    // {
    //     // kiểm tra đầu vào dữ liệu
    //     if (data is null)
    //     {
    //         throw new Exception("Không tìm thấy dữ liệu");
    //     }

    //     // kiểm tra tài khoản có tồn tại hay không
    //     var checkUser = await _userRepository.GetUserByIdAsync(data.Id);
    //     if (checkUser is null)
    //     {
    //         throw new Exception("Không tìm thấy tài khoản");
    //     }

    //     // thực hiện quá trình cập nhật tài khoản
    //     // var newAccount = await _userManager.UpdateAsync(data);
    //     var updateAccount = await _userRepository.UpdateUserAsync(data);
    //     if (!updateAccount.Succeeded)
    //     {
    //         throw new Exception(updateAccount.Errors.FirstOrDefault()?.Description);
    //     }

    //     return updateAccount.Succeeded;
    // }

    /// <summary>
    /// Tạo ra policy cho các vai trò
    /// </summary>
    /// <param name="policyName"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    public AuthorizationPolicy CreatePolicy(string policyName, string[] roles)
    {
        var policy = new AuthorizationPolicyBuilder();

        if (roles != null && roles.Length > 0)
        {
            policy.RequireRole(roles);
        }

        return policy.Build();
    }
    /// <summary>
    /// Lấy tất cả vai trò của người dùng hiện tại
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<IList<string>> GetRolesOfUser(ApplicationUser data)
    {
        return await _userRepository.GetRolesOfUserAsync(data);
    }
    /// <summary>
    /// Đăng nhập bằng google
    /// </summary>
    /// <returns></returns>
    public async Task<LoginResponse> LoginGoogle(string email, string accessTokenGoogle)
    {
        // Tạo access token và refresh token mới cho người dùng hệ thống
        var userDb = await _userRepository.GetUserByEmailAsync(email);
        if (userDb is null)
        {
            throw new Exception("Không tìm thấy tài khoản");
        }

        // Tạo 1 access token và refresh token
        var authToken = await GetToken(userDb);

        return new LoginResponse
        {
            Data = userDb,
            AccessToken = authToken.AccessToken,
            IssuedTime = authToken.IssuedTime,
            ExpiredTime = authToken.ExpiredTime,
            RefreshToken = authToken.RefreshToken,
        };
    }
    /// <summary>
    /// Lấy thông tin người dùng từ google thông qua access token nhận từ google
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<GoogleUserInfo?> GetGoogleUserInfo(string accessTokenGoogle)
    {
        // Lấy thông tin chi tiết của người dùng
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenGoogle);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();
        var userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(data);
        return userInfo;
    }
    /// <summary>
    /// Kiểm tra tài khoản đã tồn tại hay chưa
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<bool> CheckUserViaEmail(string email)
    {
        var userDb = await _userRepository.GetUserByEmailAsync(email);
        if (userDb is null)
        {
            return false;
        }

        return true;
    }
}