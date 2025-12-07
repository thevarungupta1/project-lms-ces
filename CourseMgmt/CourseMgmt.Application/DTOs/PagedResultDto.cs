namespace CourseMgmt.Application.DTOs;

/// <summary>
/// Paginated result DTO
/// </summary>
public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)Limit);
}

