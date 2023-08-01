using LearnIndentityAndAuthorization.Configs;

namespace LearnIndentityAndAuthorization.Controllers.Responses;

/// <summary>
/// Thông tin phân trang
/// </summary>
public class Paginate
{
    public int CurrentPage { get; set; } = 0;
    public int PerPage { get; set; } = 0;
    public int TotalPages { get; set; } = 0;
    public int TotalRows { get; set; } = 0;
    public int From { get; set; } = 0;
    public int To { get; set; } = 0;
}
/// <summary>
/// Thông tin mã xác thực
/// </summary>
public class Token
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
/// <summary>
/// Định nghĩa lớp trả về khi thành công
/// </summary>
public class SuccessReponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; } = null!;
    public Object? Meta { get; set; } = null!;
    public string Time { get; set; } = DateTime.Now.ToString(Constants.FORMAT_DATETIME);

    public SuccessReponse(string message = null!, object data = null!, Object? meta = null!, int status = StatusCodes.Status200OK)
    {
        Message = message;
        Data = data;
        Meta = meta;
        Status = status;
    }
}