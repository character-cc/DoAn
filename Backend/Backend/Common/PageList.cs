namespace Backend.Common;

public class PagedList<T> : IPagedList<T>
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex + 1 < TotalPages;
    public List<T> Items { get; private set; }

    public PagedList(List<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
