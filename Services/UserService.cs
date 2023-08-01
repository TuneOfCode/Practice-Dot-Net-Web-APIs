using AutoMapper;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Controllers.Dtos;
using LearnIndentityAndAuthorization.Helpers;
using LearnIndentityAndAuthorization.Models;
using LearnIndentityAndAuthorization.Repositories;

namespace LearnIndentityAndAuthorization.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    /// <summary>
    /// Lấy tất cả người dùng
    /// </summary>
    /// <returns></returns>
    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUserAsync();
        if (users is null)
        {
            throw new Exception("Không có người dùng nào");
        }
        return users;
    }
    /// <summary>
    /// Lấy người dùng theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user is null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }
        return user;
    }
    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns></returns>
    public async Task<bool> CreateUserAsync(ApplicationUser newUser)
    {
        var result = await _userRepository.CreateUserAsync(newUser);
        if (!result.Succeeded)
        {
            throw new Exception($"Tạo người dùng thất bại: {result.Errors.First().Description}");
        }
        return result.Succeeded;
    }
    /// <summary>
    /// Cập nhật người dùng
    /// </summary>
    /// <param name="editUser"></param>
    /// <returns></returns>
    public async Task<bool> UpdateUserAsync(ApplicationUser user, List<IFormFile>? avatar = null!)
    {
        // upload ảnh đại diện lên server
        string avatarUploaded = null!;
        if (avatar != null && avatar.Count > 0)
        {
            avatarUploaded = Helper.UploadFiles(avatar, 1, Constants.ALLOWED_EXTENSIONS_IMAGE).FirstOrDefault() ?? null!;
        }
        user.Avatar = avatarUploaded;

        // cập nhật người dùng
        var result = await _userRepository.UpdateUserAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception($"Cập nhật người dùng thất bại: {result.Errors.First().Description}");
        }
        return result.Succeeded;
    }
    /// <summary>
    /// Xóa người dùng
    /// </summary>
    /// <param name="deleteUser"></param>
    /// <returns></returns>
    public async Task<bool> DeleteUserAsync(ApplicationUser deleteUser)
    {
        var result = await _userRepository.DeleteUserAsync(deleteUser);
        if (!result.Succeeded)
        {
            throw new Exception($"Xóa người dùng thất bại: {result.Errors.First().Description}");
        }
        return result.Succeeded;
    }
}