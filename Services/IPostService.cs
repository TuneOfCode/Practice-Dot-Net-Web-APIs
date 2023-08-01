namespace LearnIndentityAndAuthorization.Services;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post?> GetPostByAuthorIdAsync(Guid id);
    Task<int> CreatePostAsync(Post newPost);
    Task<int> UpdatePostAsync(Post editPost);
    Task<int> DeletePostAsync(Post deletePost);
}
