namespace Acropolis.Application.Base.Pagination;

public interface IPagedList<T> where T : new()
{
    int PageIndex { get; }
    int PageSize { get; }
    long TotalPages { get; }
    long TotalItems { get; }
    IEnumerable<T> Data { get; }
}
