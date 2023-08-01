namespace LearnIndentityAndAuthorization.Controllers.Dtos.Bases;
/// <summary>
/// Lớp định nghĩa các toán tử lọc
/// </summary>
public static class FilterOperators
{
    public const string EQUAL = "eq";
    public const string NOT_EQUAL = "ne";
    public const string GREATER_THAN = "gt";
    public const string GREATER_THAN_OR_EQUAL = "gte";
    public const string LESS_THAN = "lt";
    public const string LESS_THAN_OR_EQUAL = "lte";
    public const string CONTAINS = "like";
}
/// <summary>
/// Lớp cơ sở cho các lớp DTO dùng để lọc dữ liệu
/// </summary>
public class FilterDto
{
    public string? FilterField { get; set; }
    public string? FilterOperator { get; set; } = FilterOperators.EQUAL;
    public string? FilterValue { get; set; }
}