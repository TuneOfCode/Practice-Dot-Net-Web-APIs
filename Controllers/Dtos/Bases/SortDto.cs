namespace LearnIndentityAndAuthorization.Controllers.Dtos.Bases;
/// <summary>
/// Lớp định nghĩa loại sắp xếp
/// ASC: Sắp xếp tăng dần
/// DESC: Sắp xếp giảm dần
/// </summary>
public static class SortTypes
{
    public const string ASC = "asc";
    public const string DESC = "desc";
}
/// <summary>
/// Lớp cơ sở cho các lớp DTO dùng để sắp xếp dữ liệu
/// </summary>
public class SortDto
{
    public string? SortField { get; set; }
    public string? SortType { get; set; } = SortTypes.ASC;
}