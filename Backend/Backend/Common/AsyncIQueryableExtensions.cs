
using LinqToDB;

namespace Backend.Common;

public static class AsyncIQueryableExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(
       this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        pageSize = Math.Max(pageSize, 1);

        if (pageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index cannot be less than zero.");
        var totalCount = await source.CountAsync();

        if (getOnlyTotalCount)
        {
            return new PagedList<T>(new List<T>(), pageIndex, pageSize, totalCount);
        }

        var items = await source
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, pageIndex, pageSize, totalCount);
    }
}
