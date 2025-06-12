namespace Backend.Common;

public interface IPagedList<T>
{
    int PageIndex { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
    List<T> Items { get; }
}
