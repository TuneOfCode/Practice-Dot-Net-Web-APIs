using System.Net.Http.Headers;
using System.Security.Claims;
using AutoMapper;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Controllers.Dtos;
using LearnIndentityAndAuthorization.Controllers.Filters;
using LearnIndentityAndAuthorization.Controllers.Responses;
using LearnIndentityAndAuthorization.Controllers.Responses.Auth;
using LearnIndentityAndAuthorization.Controllers.Responses.Users;
using LearnIndentityAndAuthorization.Models;
using LearnIndentityAndAuthorization.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LearnIndentityAndAuthorization.Controllers;

[ApiController]
[JwtAuthorize]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, SignInManager<ApplicationUser> _signInManager, IMapper mapper, IConfiguration configuration)
    {
        _authService = authService;
        this._signInManager = _signInManager;
        _mapper = mapper;
        _configuration = configuration;
    }
    /// <summary>
    /// Đăng ký người dùng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto request)
    {
        try
        {
            var data = _mapper.Map<ApplicationUser>(request);
            var result = await _authService.RegisterUser(data);

            if (!result)
            {
                return BadRequest("Có lỗi trong quá trình đăng ký");
            }

            return CreatedAtAction(nameof(RegisterUser), new SuccessReponse()
            {
                Status = StatusCodes.Status201Created,
                Message = "Đăng ký thành công",
                Data = new
                {
                    userId = data.Id,
                }
            });
        }
        catch (Exception ex)
        {
            return Problem(
                type: ex.GetType().ToString(),
                title: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }

    }
    /// <summary>
    /// Đăng nhập người dùng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginUserDto request)
    {
        try
        {
            var data = _mapper.Map<ApplicationUser>(request);
            var result = await _authService.LoginUser(data);

            if (result is null)
            {
                return BadRequest("Đăng nhập thất bại");
            }

            // Hiển thị kết quả
            var currentUser = _mapper.Map<UserResponse>(result.Data ?? null!);
            currentUser.Roles = await _authService.GetRolesOfUser(result.Data ?? null!);

            return Ok(new SuccessReponse()
            {
                Message = "Đăng nhập thành công",
                Data = currentUser,
                Meta = new AuthToken()
                {
                    AccessToken = result.AccessToken,
                    IssuedTime = result.IssuedTime,
                    ExpiredTime = result.ExpiredTime,
                    RefreshToken = result.RefreshToken,
                }
            });
        }
        catch (Exception ex)
        {
            return Problem(
                type: ex.GetType().ToString(),
                title: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }
    /// <summary>
    /// Đăng nhập bằng tài khoản Google
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        // Tạo URL để chuyển hướng đến giao diện đăng nhập của Google
        var redirectUrl = _configuration["Google:RedirectUrl"] ?? null!;
        if (redirectUrl is null)
        {
            return BadRequest("Chưa cấu hình đường dẫn chuyển hướng tới Google");
        }
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        // Chuyển hướng đến giao diện đăng nhập của Google
        var clientId = _configuration["Google:ClientId"] ?? null!;
        var scope = _configuration["Google:Scope"] ?? null!;
        if (clientId is null || scope is null)
        {
            return BadRequest("Chưa cấu hình ClientId hoặc Scope của Google");
        }

        // // Chuyển hướng đến giao diện đăng nhập của Google
        properties.AllowRefresh = true;
        return new ChallengeResult("Google", properties);
    }

    /// <summary>
    /// Callback khi đăng nhập bằng tài khoản Google
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("signin-google")]
    public async Task<IActionResult> GoogleCallback()
    {
        try
        {
            // var info = await _signInManager.GetExternalLoginInfoAsync();
            // if (info == null)
            // {
            //     // Xử lý lỗi khi không nhận được thông tin từ Google
            //     return BadRequest("Không nhận được thông tin từ Google");
            // }

            // // Kiểm tra tài khoản người đùng đăng nhập bằng Google đã tồn tại với email mà người dùng đã đăng ký trong hệ thống trước đó chưa
            // var email = info.Principal.FindFirst(ClaimTypes.Email)!.Value;
            // if (email == null)
            // {
            //     // Xử lý lỗi khi không nhận được email từ Google
            //     return BadRequest("Không nhận được email từ Google");
            // }

            // Xác thực thành công, tạo hoặc đăng nhập người dùng với thông tin nhận được từ Google
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                // Nếu người dùng chưa tồn tại, bạn có thể tạo tài khoản mới ở đây
                // và sau đó đăng nhập với thông tin nhận được từ Google

                // Xử lý lỗi khi xác thực thất bại
                return BadRequest(new ErrorResponse()
                {
                    Title = "Xác thực thất bại",
                });
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)!.Value;
            if (email is null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Title = "Không nhận được email từ Google",
                });
            }

            // Lấy access token và refresh token
            var accessTokenGoogle = result.Properties.GetTokenValue("access_token") ?? null!;
            if (accessTokenGoogle is null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Title = "Không nhận được token từ Google",
                });
            }

            // Kiểm tra tài khoản người đùng đăng nhập bằng Google đã tồn tại với email mà người dùng đã đăng ký trong hệ thống trước đó chưa
            var isEmailInDb = await _authService.CheckUserViaEmail(email);

            // Trường hợp chưa tồn tại tài khoản người dùng trong hệ thống
            if (!isEmailInDb)
            {
                // Tạo mới tài khoản người dùng
                var userInfoGoogle = await _authService.GetGoogleUserInfo(accessTokenGoogle);
                var registerData = _mapper.Map<ApplicationUser>(userInfoGoogle);
                var isRegistered = await _authService.RegisterUser(registerData);
                if (!isRegistered)
                {
                    return BadRequest("Đăng ký tài khoản thất bại");
                }
            }

            // Đăng nhập với tài khoản vừa tạo
            var resultLogin = await _authService.LoginGoogle(email, accessTokenGoogle);
            if (resultLogin is null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Title = "Đăng nhập thất bại với tài khoản Google"
                });
            }
            var resultData = _mapper.Map<UserResponse>(resultLogin.Data ?? null!);

            // Xác thực thành công, redirect đến trang ReturnUrl
            return Ok(new SuccessReponse()
            {
                Message = "Đăng nhập thành công với tài khoản Google",
                Data = resultData,
                Meta = new AuthToken()
                {
                    AccessToken = resultLogin.AccessToken,
                    IssuedTime = resultLogin.IssuedTime,
                    ExpiredTime = resultLogin.ExpiredTime,
                    RefreshToken = resultLogin.RefreshToken,
                }
            });
        }
        catch (System.Exception ex)
        {
            return Problem(
                type: ex.GetType().ToString(),
                title: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var result = await _authService.GetCurrentUser();

            // Hiển thị kết quả
            var currentUser = _mapper.Map<UserResponse>(result);
            currentUser.Roles = await _authService.GetRolesOfUser(result);

            return Ok(new SuccessReponse()
            {
                Message = "Lấy thông tin người dùng thành công",
                Data = currentUser
            });
        }
        catch (Exception ex)
        {
            return Problem(
                type: ex.GetType().ToString(),
                title: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }
    /// <summary>
    /// Lấy thông tin của tất cả người dùng
    /// </summary>
    /// <returns></returns>
    [JwtAuthorize(Roles = $"{Roles.SUPPER_ADMIN},{Roles.ADMIN}")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUser()
    {
        try
        {
            var result = await _authService.GetAllUser();
            var users = _mapper.Map<List<UserResponse>>(result);
            users.ForEach(u => u.Roles = _authService.GetRolesOfUser(result.FirstOrDefault(r => r.Id == u.Id) ?? null!).Result);

            return Ok(new SuccessReponse()
            {
                Message = "Lấy danh sách người dùng thành công",
                Data = users,
                Meta = new Paginate()
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    /// <summary>
    /// Đăng xuất người dùng
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutUser()
    {
        try
        {
            await _authService.LogoutUser();
            return Ok(new SuccessReponse()
            {
                Message = "Đăng xuất thành công"
            });
        }
        catch (Exception ex)
        {
            return Problem(
                type: ex.GetType().ToString(),
                title: ex.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }
}