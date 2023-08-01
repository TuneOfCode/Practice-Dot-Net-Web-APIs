using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LearnIndentityAndAuthorization.Models;


public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(1000)]
    public string? Avatar { get; set; }
    public string? RefreshToken { get; set; }
}