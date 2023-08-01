namespace LearnIndentityAndAuthorization.Controllers.Dtos.Bases;

public class PaginateDto
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 0; // 0: lấy tất cả dữ liệu
}