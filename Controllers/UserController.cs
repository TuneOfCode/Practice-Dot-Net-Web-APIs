using AutoMapper;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Controllers.Dtos;
using LearnIndentityAndAuthorization.Controllers.Filters;
using LearnIndentityAndAuthorization.Controllers.Responses;
using LearnIndentityAndAuthorization.Helpers;
using LearnIndentityAndAuthorization.Models;
using LearnIndentityAndAuthorization.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnIndentityAndAuthorization.Controllers;

[ApiController]
[JwtAuthorize(Roles = $"{Roles.SUPPER_ADMIN},{Roles.ADMIN}")]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new SuccessReponse()
            {
                Message = "Lấy danh sách người dùng thành công",
                Data = users,
                Meta = new Paginate()
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new ErrorResponse()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(new SuccessReponse()
            {
                Message = "Lấy người dùng thành công",
                Data = user
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new ErrorResponse()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ex.Message
            });
        }
    }

    [JwtAuthorize(Roles = $"{Roles.SUPPER_ADMIN}")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromForm] RegisterUserDto data)
    {
        try
        {
            var newUser = _mapper.Map<ApplicationUser>(data);
            var result = await _userService.CreateUserAsync(newUser);
            return Ok(new SuccessReponse()
            {
                Message = "Tạo người dùng thành công",
                Data = new
                {
                    userId = newUser.Id
                }
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new ErrorResponse()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ex.Message
            });
        }
    }

    [JwtAuthorize(Roles = $"{Roles.SUPPER_ADMIN}")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto data, string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
            {
                return NotFound(new ErrorResponse()
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Không tìm thấy người dùng"
                });
            }
            data.Id = id;
            user.Name = data.Name ?? user.Name;
            user.Email = data.Email ?? user.Email;

            var result = await _userService.UpdateUserAsync(user, data.Avatar);
            return Ok(new SuccessReponse()
            {
                Message = "Cập nhật người dùng thành công",
                Data = user
            });

        }
        catch (System.Exception ex)
        {
            return BadRequest(new ErrorResponse()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ex.Message
            });
        }
    }

    [JwtAuthorize(Roles = $"{Roles.SUPPER_ADMIN}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            var result = await _userService.DeleteUserAsync(user ?? null!);
            return Ok(new SuccessReponse()
            {
                Message = "Xóa người dùng thành công",
                Data = new
                {
                    userId = user!.Id
                }
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new ErrorResponse()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ex.Message
            });
        }
    }
}