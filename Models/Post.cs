using System.ComponentModel.DataAnnotations;
using LearnIndentityAndAuthorization.Models;

public class Post
{
    [Key]
    public Guid UUID { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Text { get; set; }

    [MaxLength(1000)]
    public string? ImageURL { get; set; }

    public int Upvote { get; set; } = 0;

    public int Downvote { get; set; } = 0;

    [Required]
    public bool isPublic { get; set; } = true;

    public ApplicationUser? Author { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime PublishedAt { get; set; } = DateTime.Now;
}