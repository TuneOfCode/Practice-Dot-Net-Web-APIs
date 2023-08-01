using AutoMapper;
using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Controllers.Dtos;
using LearnIndentityAndAuthorization.Controllers.Dtos.Bases;
using LearnIndentityAndAuthorization.Controllers.Filters;
using LearnIndentityAndAuthorization.Controllers.Responses;
using LearnIndentityAndAuthorization.Helpers;
using LearnIndentityAndAuthorization.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnIndentityAndAuthorization.Controllers;

[ApiController]
[JwtAuthorize]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    private readonly IUserService _userService;

    private readonly IMapper _mapper;

    public PostController(IPostService postService, IUserService userService, IMapper mapper)
    {
        _postService = postService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts
    (
        [FromQuery] PaginateDto paginateDto = null!,
        [FromQuery] FilterDto filterDto = null!,
        [FromQuery] SortDto sortDto = null!)
    {
        try
        {
            List<Post> posts = await _postService.GetAllPostsAsync();
            var paginate = Collection<Post>.Query(posts, filterDto, sortDto, paginateDto);
            return Ok(new SuccessReponse()
            {
                Message = "Lấy danh sách bài viết thành công",
                Data = paginate.Data,
                Meta = new Paginate()
                {
                    CurrentPage = paginate.CurrentPage,
                    PerPage = paginate.PerPage,
                    TotalPages = paginate.TotalPages,
                    TotalRows = paginate.TotalRows,
                    From = paginate.From,
                    To = paginate.To
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            return Ok(new SuccessReponse()
            {
                Message = "Lấy bài viết thành công",
                Data = post
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

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto newPost)
    {
        try
        {
            var request = _mapper.Map<Post>(newPost);

            var result = await _postService.CreatePostAsync(request);
            return Ok(new SuccessReponse()
            {
                Message = "Tạo bài viết thành công",
                Data = newPost
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] Post editPost)
    {
        try
        {
            editPost.UUID = id;
            var request = _mapper.Map<Post>(editPost);
            var author = await _userService.GetUserByIdAsync(editPost.Author!.Id);
            editPost.Author = author;

            var result = await _postService.UpdatePostAsync(request);
            return Ok(new SuccessReponse()
            {
                Message = "Cập nhật bài viết thành công",
                Data = result
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            var result = await _postService.DeletePostAsync(post!);
            return Ok(new SuccessReponse()
            {
                Message = "Xóa bài viết thành công",
                Data = result
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