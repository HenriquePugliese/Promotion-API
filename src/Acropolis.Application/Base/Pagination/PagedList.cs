namespace Acropolis.Application.Base.Pagination;

public class PagedList<T> : IPagedList<T> where T : new()
{
    public PagedList(IEnumerable<T> items, long count, int pageIndex, int pageSize)
    {
        TotalItems = count;
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalPages = (long)Math.Ceiling(count / (double)pageSize);
        Data = items;
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public long TotalPages { get; }
    public long TotalItems { get; }
    public bool HasPrevious => PageIndex > 1;
    public bool HasNext => PageIndex < TotalPages;
    public IEnumerable<T> Data { get; set; }
}
