using System.ComponentModel.DataAnnotations;

namespace LearnIndentityAndAuthorization.Controllers.Dtos;

public class UpdateUserDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    public string? Email { get; set; }

    [MaxLength(5000)]
    public List<IFormFile>? Avatar { get; set; }
}