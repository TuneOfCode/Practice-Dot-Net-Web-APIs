using LearnIndentityAndAuthorization.Databases;
using Microsoft.EntityFrameworkCore;

namespace LearnIndentityAndAuthorization.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Lấy tất cả bài viết
    /// </summary>
    /// <returns></returns>
    public async Task<List<Post>> GetAllPostsAsync()
    {
        var posts = await _context.Posts!.ToListAsync();
        return posts;
    }
    /// <summary>
    /// Lấy bài viết theo id
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        var post =
            await _context.Posts!
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.UUID == id);
        return post;
    }

    public async Task<Post?> GetPostByAuthorIdAsync(Guid id)
    {
        var authorId = new Guid(id.ToString());
        var post = await _context.Posts!.FirstOrDefaultAsync(x => authorId == id);

        return post;
    }
    /// <summary>
    /// Tạo bài viết mới
    /// </summary>
    /// <param name="newPost"></param>
    /// <returns></returns>
    public async Task<int> CreatePostAsync(Post newPost)
    {
        await _context.Posts!.AddAsync(newPost);
        return await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Cập nhật bài viết
    /// </summary>
    /// <param name="editPost"></param>
    /// <returns></returns>
    public async Task<int> UpdatePostAsync(Post editPost)
    {
        _context.Posts!.Update(editPost);
        return await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Xóa bài viết
    /// </summary>
    /// <param name="deletePost"></param>
    /// <returns></returns>
    public async Task<int> DeletePostAsync(Post deletePost)
    {
        _context.Posts!.Remove(deletePost);
        return await _context.SaveChangesAsync();
    }
}