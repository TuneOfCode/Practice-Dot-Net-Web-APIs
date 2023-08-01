namespace LearnIndentityAndAuthorization.Controllers.Responses.Bases;
/// <summary>
/// Lớp trả về dữ liệu phân trang
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginateResponse<T> : Paginate where T : class
{
    public IEnumerable<T> Data { get; set; } = null!;
}