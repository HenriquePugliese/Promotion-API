namespace Acropolis.Application.Base.Models;

public abstract class Parameter
{
    // Constants

    private const int MaxPageSize = 50;

    // Fields

    private int _pageSize = 25;
    private string? _orderBy;

    // Properties

    public int PageIndex { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? OrderBy
    {
        get => _orderBy;
        set => _orderBy = !string.IsNullOrWhiteSpace(value) ? value.Trim().ToLowerInvariant() : null;
    }
}
