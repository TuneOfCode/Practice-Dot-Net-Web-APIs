using System.Reflection;
using LearnIndentityAndAuthorization.Controllers.Dtos.Bases;
using LearnIndentityAndAuthorization.Controllers.Responses.Bases;

namespace LearnIndentityAndAuthorization.Helpers;
/// <summary>
/// Lớp hỗ trợ cho việc lọc, sắp xếp, phân trang dữ liệu
/// </summary>
/// <typeparam name="T"></typeparam>
public static class Collection<T> where T : class
{
    private static List<string> _filters = null!;
    /// <summary>
    /// Hàm kiểm tra xem trường lọc hoặc sắp xếp có tồn tại trong đối tượng không
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private static bool CheckField(string field)
    {
        _filters = new List<string>();
        Type type = typeof(T);
        bool isExist = false;
        foreach (PropertyInfo propertyInfo in type.GetProperties())
        {
            if (propertyInfo.Name == field)
            {
                isExist = true;
                continue;
            }
            _filters.Add(propertyInfo.Name);
        }

        return isExist;
    }
    /// <summary>
    /// Hàm lọc dữ liệu
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static PaginateResponse<T> Filter(IEnumerable<T> collections, FilterDto query, PaginateDto queryPaginate)
    {
        var result = new PaginateResponse<T>();
        // kiểm tra dữ liệu đầu vào
        if (query.FilterField is null || query.FilterOperator is null || query.FilterValue is null)
        {
            result.Data = collections.ToList();
            return Paginate(result.Data, queryPaginate);
        }

        query.FilterField = query.FilterField.Trim();
        query.FilterOperator = query.FilterOperator.Trim();
        query.FilterValue = query.FilterValue.Trim().ToLower();

        // kiểm tra xem trường lọc có tồn tại trong đối tượng không
        if (!CheckField(query.FilterField))
        {
            throw new Exception($"Trường lọc không tồn tại trong đối tượng. Trường lọc phải là một trong các trường sau: {string.Join(", ", _filters)}");
        }

        // thực hiện lọc dữ liệu
        switch (query.FilterOperator)
        {
            case FilterOperators.EQUAL: // so sánh bằng
                result.Data = collections.Where(item => item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString() == query.FilterValue).ToList();
                break;

            case FilterOperators.NOT_EQUAL: // so sánh khác
                result.Data = collections.Where(item => item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()! != query.FilterValue).ToList();
                break;

            case FilterOperators.GREATER_THAN: // so sánh lớn hơn
                result.Data = collections.Where(item => Double.Parse(item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()!) > Double.Parse(query.FilterValue)).ToList();
                break;

            case FilterOperators.GREATER_THAN_OR_EQUAL: // so sánh lớn hơn hoặc bằng
                result.Data = collections.Where(item => Double.Parse(item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()!) >= Double.Parse(query.FilterValue)).ToList();
                break;

            case FilterOperators.LESS_THAN: // so sánh nhỏ hơn
                result.Data = collections.Where(item => Double.Parse(item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()!) < Double.Parse(query.FilterValue)).ToList();
                break;

            case FilterOperators.LESS_THAN_OR_EQUAL: // so sánh nhỏ hơn hoặc bằng
                result.Data = collections.Where(item => Double.Parse(item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()!) <= Double.Parse(query.FilterValue)).ToList();
                break;

            case FilterOperators.CONTAINS: // so sánh chứa trong chuỗi
                result.Data = collections.Where(item => item.GetType().GetProperty(query.FilterField)!.GetValue(item)!.ToString()!.ToLower()!.Contains(query.FilterValue)).ToList();
                break;

            default:
                result.Data = collections.ToList();
                break;
        }

        return Paginate(result.Data, queryPaginate);
    }
    /// <summary>
    /// Hàm sắp xếp dữ liệu
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static PaginateResponse<T> Sort(IEnumerable<T> collections, SortDto query, PaginateDto queryPaginate)
    {
        var result = new PaginateResponse<T>();
        // kiểm tra dữ liệu đầu vào
        if (query.SortField is null || query.SortType is null)
        {
            result.Data = collections.ToList();
            return Paginate(result.Data, queryPaginate);
        }

        // kiểm tra xem trường sắp xếp có tồn tại trong đối tượng không
        if (!CheckField(query.SortField))
        {
            throw new Exception($"Trường lọc không tồn tại trong đối tượng. Trường lọc phải là một trong các trường sau: {string.Join(", ", _filters)}");
        }

        // thực hiện sắp xếp dữ liệu
        query.SortField = query.SortField.Trim();
        query.SortType = query.SortType.Trim();
        switch (query.SortType)
        {
            case SortTypes.ASC: // sắp xếp tăng dần
                result.Data = collections.OrderBy(item => item.GetType().GetProperty(query.SortField)!.GetValue(item)).ToList();
                break;
            case SortTypes.DESC: // sắp xếp giảm dần
                result.Data = collections.OrderByDescending(item => item.GetType().GetProperty(query.SortField
                )!.GetValue(item)).ToList();
                break;
            default:
                result.Data = collections.ToList();
                break;
        }
        return Paginate(result.Data, queryPaginate);
    }
    /// <summary>
    /// Hàm phân trang dữ liệu
    /// </summary>
    /// <param name="collections"></param>
    /// <param name="query"></param>
    /// <param name="queryFilter"></param>
    /// <param name="querySort"></param>
    /// <returns></returns>
    public static PaginateResponse<T> Paginate(IEnumerable<T> collections, PaginateDto query)
    {
        var result = new PaginateResponse<T>();

        // kiểm tra dữ liệu đầu vào
        if (query is null || query.Limit <= 0 || query.Page <= 0)
        {
            // lấy tất cả dữ liệu
            result.CurrentPage = 1;
            result.PerPage = 0;
            result.TotalPages = 1;
            result.From = (query!.Page - 1) * query!.Limit + 1;
            result.To = collections.Count();
            result.Data = collections.ToList();

            return result;
        }

        // thực hiện phân trang dữ liệu
        result.CurrentPage = query.Page;
        result.PerPage = query.Limit;
        result.TotalRows = collections.Count();
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRows / query.Limit);
        result.From = (query.Page - 1) * query.Limit;
        result.To = query.Page * query.Limit;
        result.Data = collections.Skip(result.From).Take(result.To).ToList();

        return result;
    }

    public static PaginateResponse<T> Query(IEnumerable<T> collections, FilterDto? queryFilter = null!, SortDto? querySort = null!, PaginateDto? queryPaginate = null!)
    {
        var result = new PaginateResponse<T>();

        // kiểm tra nếu có phân trang dữ liệu
        if (queryPaginate is null)
        {
            queryPaginate = new PaginateDto();
        }

        // kiểm tra nếu có lọc dữ liệu và có sắp xếp dữ liệu
        if (queryFilter != null && querySort != null)
        {
            result = Sort(Filter(collections, queryFilter, new PaginateDto()).Data, querySort, queryPaginate);
            return result;
        }

        // kiểm tra nếu có sắp xếp dữ liệu và không có lọc dữ liệu
        if (queryFilter is null && querySort != null)
        {
            result = Sort(collections, querySort, queryPaginate);
            return result;
        }

        // kiểm tra nếu có lọc dữ liệu và không có sắp xếp dữ liệu
        if (queryFilter != null && querySort is null)
        {
            result = Filter(collections, queryFilter, queryPaginate);
            return result;
        }


        result = Paginate(collections, queryPaginate);

        return result;
    }
}