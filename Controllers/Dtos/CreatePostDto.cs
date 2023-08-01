using System.ComponentModel.DataAnnotations;

namespace LearnIndentityAndAuthorization.Controllers.Dtos;

public class CreatePostDto
{
    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [MinLength(3, ErrorMessage = "Tiêu đề không được ít hơn 3 ký tự")]
    [MaxLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Nội dung không được để trống")]
    [MaxLength(1000, ErrorMessage = "Nội dung không được quá 1000 ký tự")]
    public string? Text { get; set; }

    [MaxLength(1000)]
    public string? ImageURL { get; set; }

    public int Upvote { get; set; } = 0;

    public int Downvote { get; set; } = 0;

    public Guid AuthorId { get; set; }
}