using System.ComponentModel.DataAnnotations;

namespace LearnIndentityAndAuthorization.Controllers.Dtos;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Họ tên phải có ít nhất 5 ký tự")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Địa chỉ email không được để trống")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Mật khẩu cần được xác nhận lại")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận lại không chính xác")]
    public string? ConfirmPassword { get; set; }
    [MaxLength(5000)]

    public List<IFormFile>? Avatar { get; set; }
}