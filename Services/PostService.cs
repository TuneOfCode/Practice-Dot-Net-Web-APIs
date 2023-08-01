using LearnIndentityAndAuthorization.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LearnIndentityAndAuthorization.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    private readonly IUserRepository _userRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(
        IPostRepository postRepository,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Lấy tất cả bài viết
    /// </summary>
    /// <returns></returns>
    public async Task<List<Post>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllPostsAsync();
        if (posts is null)
        {
            throw new Exception("Không có bài viết nào");
        }
        return posts;
    }
    /// <summary>
    /// Lấy bài viết theo id
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post is null)
        {
            throw new Exception("Không tìm thấy bài viết");
        }
        return post;
    }
    /// <summary>
    /// Lấy bài viết theo id tác giả
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Post?> GetPostByAuthorIdAsync(Guid id)
    {
        var post = _postRepository.GetPostByAuthorIdAsync(id);
        if (post is null)
        {
            throw new Exception("Không tìm thấy bài viết");
        }
        return post;
    }
    /// <summary>
    /// Tạo bài viết mới
    /// </summary>
    /// <param name="newPost"></param>
    /// <returns></returns>
    public async Task<int> CreatePostAsync(Post newPost)
    {
        var userNameCurrent = _httpContextAccessor.HttpContext!.Items["UserName"]?.ToString() ?? null!;
        var currentUser = await _userRepository.GetUserByUserNameAsync(userNameCurrent);

        if (currentUser is null)
        {
            throw new Exception("Không tìm thấy người tạo bài viết");
        }

        newPost.Author = currentUser;
        var result = await _postRepository.CreatePostAsync(newPost);
        if (result == 0)
        {
            throw new Exception("Tạo bài viết thất bại");
        }
        return result;
    }
    /// <summary>
    /// Cập nhật bài viết
    /// </summary>
    /// <param name="editPost"></param>
    /// <returns></returns>
    public async Task<int> UpdatePostAsync(Post editPost)
    {
        // kiểm tra người dùng hiện tại có phải là tác giả của bài viết không
        var userNameCurrent = _httpContextAccessor.HttpContext!.Items["UserName"]?.ToString() ?? null!;
        var currentUser = _userRepository.GetUserByUserNameAsync(userNameCurrent);
        var authorId = int.Parse(editPost.Author!.Id);

        if (authorId != currentUser.Id)
        {
            throw new Exception("Bạn không có quyền cập nhật bài viết này");
        }
        var result = await _postRepository.UpdatePostAsync(editPost);
        if (result == 0)
        {
            throw new Exception("Cập nhật bài viết thất bại");
        }
        return result;
    }
    /// <summary>
    /// Xóa bài viết
    /// </summary>
    /// <param name="deletePost"></param>
    /// <returns></returns>
    public async Task<int> DeletePostAsync(Post deletePost)
    {
        // kiểm tra người dùng hiện tại có phải là tác giả của bài viết không
        var userNameCurrent = _httpContextAccessor.HttpContext!.Items["UserName"]?.ToString() ?? null!;
        var currentUser = await _userRepository.GetUserByUserNameAsync(userNameCurrent);
        if (currentUser is null)
        {
            throw new Exception("Không tìm thấy người tạo bài viết");
        }
        var authorId = deletePost.Author!.Id;

        if (authorId != currentUser.Id)
        {
            throw new Exception("Bạn không có quyền xoá bài viết này");
        }
        var result = await _postRepository.DeletePostAsync(deletePost);
        if (result == 0)
        {
            throw new Exception("Xóa bài viết thất bại");
        }
        return result;
    }
}